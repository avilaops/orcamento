using roncav_budget.Models;
using roncav_budget.Services.Avila;

namespace roncav_budget.Services;

/// <summary>
/// Serviço de sincronização entre dados locais (SQLite) e nuvem (api.avila.inc)
/// Implementa estratégia offline-first com sync bidirecional e retry automático
/// </summary>
public class SyncService
{
    private readonly AvilaApiService _apiService;
    private readonly DatabaseService _databaseService;
    private readonly IPreferences _preferences;
    private readonly IConnectivity _connectivity;
    private readonly ErrorHandlingService _errorHandler;

    private bool _isSyncing = false;
    private DateTime? _lastSyncAt;
    
    // 🔄 Configurações de retry
    private const int MaxRetryAttempts = 3;
    private static readonly TimeSpan[] RetryDelays = new[]
    {
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10)
    };

    public event EventHandler<SyncEventArgs>? SyncStarted;
    public event EventHandler<SyncEventArgs>? SyncCompleted;
    public event EventHandler<SyncEventArgs>? SyncFailed;

    public SyncService(
        AvilaApiService apiService,
        DatabaseService databaseService,
        IPreferences preferences,
        IConnectivity connectivity,
        ErrorHandlingService errorHandler)
    {
        _apiService = apiService;
        _databaseService = databaseService;
        _preferences = preferences;
        _connectivity = connectivity;
        _errorHandler = errorHandler;

        // Carregar última sincronização
        var lastSyncStr = _preferences.Get("last_sync_at", string.Empty);
        if (DateTime.TryParse(lastSyncStr, out var lastSync))
        {
            _lastSyncAt = lastSync;
        }

        // Monitorar conectividade
        _connectivity.ConnectivityChanged += OnConnectivityChanged;
    }

    /// <summary>
    /// Sincroniza dados bidirecionalmente com retry automático
    /// </summary>
    public async Task<SyncResult> SyncAsync(bool force = false)
    {
        // Evitar sincronização concorrente
        if (_isSyncing)
            return SyncResult.Failed("Sincronização já em andamento");

        // Verificar conectividade
        if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            return SyncResult.Failed("Sem conexão com a internet");

        // Verificar se precisa sincronizar (skip se < 5 min desde última sync)
        if (!force && _lastSyncAt.HasValue && DateTime.UtcNow - _lastSyncAt.Value < TimeSpan.FromMinutes(5))
            return SyncResult.Success(new SyncResponse { Success = true, ItemsSynced = 0, SyncedAt = _lastSyncAt.Value });

        return await ExecuteWithRetryAsync(async () =>
        {
            try
            {
                _isSyncing = true;
                SyncStarted?.Invoke(this, new SyncEventArgs { StartedAt = DateTime.UtcNow });

                // 1. Upload dados locais modificados
                var uploadResult = await UploadLocalChangesAsync();
                if (!uploadResult.IsSuccess)
                    return uploadResult;

                // 2. Download dados do servidor
                var downloadResult = await DownloadServerDataAsync();
                if (!downloadResult.IsSuccess)
                    return SyncResult.Failed(downloadResult.ErrorMessage ?? "Erro ao baixar dados");

                // 3. Resolver conflitos (se houver)
                if (uploadResult.Data?.Conflicts?.Any() == true)
                {
                    await ResolveConflictsAsync(uploadResult.Data.Conflicts);
                }

                // Atualizar última sincronização
                _lastSyncAt = DateTime.UtcNow;
                _preferences.Set("last_sync_at", _lastSyncAt.Value.ToString("O"));

                var result = SyncResult.Success(uploadResult.Data!);
                SyncCompleted?.Invoke(this, new SyncEventArgs 
                { 
                    CompletedAt = DateTime.UtcNow, 
                    ItemsSynced = uploadResult.Data!.ItemsSynced 
                });

                return result;
            }
            catch (Exception ex)
            {
                await _apiService.LogErrorAsync("Sync", ex);
                var result = SyncResult.Failed($"Erro na sincronização: {ex.Message}");
                SyncFailed?.Invoke(this, new SyncEventArgs { Error = ex.Message });
                throw; // Re-throw para o retry handler
            }
            finally
            {
                _isSyncing = false;
            }
        });
    }

    /// <summary>
    /// 🔄 Executa ação com retry exponencial automático
    /// </summary>
    private async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> action)
    {
        Exception? lastException = null;

        for (int attempt = 0; attempt < MaxRetryAttempts; attempt++)
        {
            try
            {
                return await action();
            }
            catch (HttpRequestException ex) when (attempt < MaxRetryAttempts - 1)
            {
                lastException = ex;
                System.Diagnostics.Debug.WriteLine($"🔄 Tentativa {attempt + 1}/{MaxRetryAttempts} falhou. Retentando em {RetryDelays[attempt].TotalSeconds}s...");
                
                await Task.Delay(RetryDelays[attempt]);
                continue;
            }
            catch (TimeoutException ex) when (attempt < MaxRetryAttempts - 1)
            {
                lastException = ex;
                System.Diagnostics.Debug.WriteLine($"⏱️ Timeout na tentativa {attempt + 1}/{MaxRetryAttempts}. Retentando...");
                
                await Task.Delay(RetryDelays[attempt]);
                continue;
            }
            catch (Exception ex)
            {
                // Outros erros não devem fazer retry
                await _errorHandler.HandleErrorAsync(ex, "SyncRetry");
                throw;
            }
        }

        // Todas as tentativas falharam
        var finalError = lastException ?? new Exception("Sync falhou após múltiplas tentativas");
        await _errorHandler.HandleErrorAsync(finalError, "SyncRetryExhausted");
        throw finalError;
    }

    /// <summary>
    /// Envia dados locais modificados para o servidor
    /// </summary>
    private async Task<SyncResult> UploadLocalChangesAsync()
    {
        try
        {
            // Obter itens pendentes de sincronização
            var contas = await _databaseService.ObterContasAsync();
            var transacoes = await _databaseService.ObterTransacoesAsync();
            var orcamentos = await _databaseService.ObterOrcamentosMesAsync(DateTime.Now.Month, DateTime.Now.Year);
            var metas = await _databaseService.ObterMetasAsync();

            // Filtrar apenas itens não sincronizados ou modificados após última sync
            var pendingItems = new List<object>();

            foreach (var conta in contas.Where(c => !c.IsSynced || (c.DataAtualizacao > _lastSyncAt)))
                pendingItems.Add(conta);

            foreach (var transacao in transacoes.Where(t => !t.IsSynced || (t.DataAtualizacao > _lastSyncAt)))
                pendingItems.Add(transacao);

            // Orçamentos e Metas ainda não têm campo IsSynced, então sincroniza todos
            foreach (var orcamento in orcamentos)
                pendingItems.Add(orcamento);

            foreach (var meta in metas)
                pendingItems.Add(meta);

            if (!pendingItems.Any())
                return SyncResult.Success(new SyncResponse { Success = true, ItemsSynced = 0, SyncedAt = DateTime.UtcNow });

            // Enviar para servidor
            var payload = new SyncPayload
            {
                CreatedItems = pendingItems,
                LastSyncAt = _lastSyncAt ?? DateTime.MinValue
            };

            var result = await _apiService.SyncDataAsync(payload);

            // Marcar itens como sincronizados
            if (result.IsSuccess)
            {
                await MarkItemsAsSyncedAsync(pendingItems);
            }

            return result;
        }
        catch (Exception ex)
        {
            return SyncResult.Failed($"Erro ao enviar dados: {ex.Message}");
        }
    }

    /// <summary>
    /// Baixa dados atualizados do servidor
    /// </summary>
    private async Task<DownloadResult> DownloadServerDataAsync()
    {
        try
        {
            var result = await _apiService.DownloadDataAsync(_lastSyncAt);

            if (result.IsSuccess && result.Data?.Items != null)
            {
                // Processar e salvar dados no banco local
                await ProcessServerDataAsync(result.Data.Items);
            }

            return result;
        }
        catch (Exception ex)
        {
            return DownloadResult.Failed($"Erro ao baixar dados: {ex.Message}");
        }
    }

    /// <summary>
    /// Processa dados recebidos do servidor e salva localmente
    /// </summary>
    private async Task ProcessServerDataAsync(List<object> items)
    {
        foreach (var item in items)
        {
            // Aqui você processaria cada tipo de item
            // Por exemplo, usando pattern matching ou reflection

            switch (item)
            {
                case Conta conta:
                    conta.IsSynced = true;
                    await _databaseService.SalvarContaAsync(conta);
                    break;

                case Transacao transacao:
                    transacao.IsSynced = true;
                    await _databaseService.SalvarTransacaoAsync(transacao);
                    break;

                    // Adicionar outros tipos conforme necessário
            }
        }
    }

    /// <summary>
    /// Resolve conflitos de sincronização
    /// </summary>
    private async Task ResolveConflictsAsync(List<ConflictInfo> conflicts)
    {
        var resolutions = new List<ConflictResolution>();

        foreach (var conflict in conflicts)
        {
            // Estratégia padrão: server-wins para dados críticos
            // Pode ser customizado por tipo de dado
            resolutions.Add(new ConflictResolution
            {
                ItemId = conflict.ItemId,
                Resolution = "server-wins" // ou "client-wins", "merge"
            });
        }

        await _apiService.ResolveConflictsAsync(resolutions);
    }

    /// <summary>
    /// Marca itens como sincronizados no banco local
    /// </summary>
    private async Task MarkItemsAsSyncedAsync(List<object> items)
    {
        foreach (var item in items)
        {
            switch (item)
            {
                case Conta conta:
                    conta.IsSynced = true;
                    await _databaseService.SalvarContaAsync(conta);
                    break;

                case Transacao transacao:
                    transacao.IsSynced = true;
                    await _databaseService.SalvarTransacaoAsync(transacao);
                    break;

                    // Adicionar outros tipos
            }
        }
    }

    /// <summary>
    /// Handler para mudanças de conectividade - sincroniza automaticamente quando voltar online
    /// </summary>
    private async void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        if (e.NetworkAccess == NetworkAccess.Internet)
        {
            // Aguardar 2 segundos para estabilizar conexão
            await Task.Delay(2000);

            // Tentar sincronizar automaticamente
            _ = SyncAsync();
        }
    }

    /// <summary>
    /// Obtém status atual da sincronização
    /// </summary>
    public SyncStatusInfo GetStatus()
    {
        return new SyncStatusInfo
        {
            IsSyncing = _isSyncing,
            LastSyncAt = _lastSyncAt,
            HasInternet = _connectivity.NetworkAccess == NetworkAccess.Internet
        };
    }

    /// <summary>
    /// Limpa cache de sincronização (forçar full sync)
    /// </summary>
    public void ClearSyncCache()
    {
        _lastSyncAt = null;
        _preferences.Remove("last_sync_at");
    }
}

public class SyncEventArgs : EventArgs
{
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int ItemsSynced { get; set; }
    public string? Error { get; set; }
}

public class SyncStatusInfo
{
    public bool IsSyncing { get; set; }
    public DateTime? LastSyncAt { get; set; }
    public bool HasInternet { get; set; }

    public string StatusText => IsSyncing ? "Sincronizando..." :
                               !HasInternet ? "Sem conexão" :
                               LastSyncAt.HasValue ? $"Última sync: {GetRelativeTime(LastSyncAt.Value)}" :
                               "Nunca sincronizado";

    private string GetRelativeTime(DateTime dateTime)
    {
        var diff = DateTime.UtcNow - dateTime;

        if (diff.TotalMinutes < 1) return "agora";
        if (diff.TotalMinutes < 60) return $"há {(int)diff.TotalMinutes} min";
        if (diff.TotalHours < 24) return $"há {(int)diff.TotalHours}h";
        return $"há {(int)diff.TotalDays} dias";
    }
}

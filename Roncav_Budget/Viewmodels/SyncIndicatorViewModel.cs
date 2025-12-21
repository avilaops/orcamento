using CommunityToolkit.Mvvm.ComponentModel;
using roncav_budget.Services;

namespace Roncav_Budget.ViewModels;

/// <summary>
/// ViewModel global para indicador de sincronização (usado em toda a app)
/// </summary>
public partial class SyncIndicatorViewModel : ObservableObject
{
    private readonly SyncService _syncService;

    [ObservableProperty]
    private bool _isSyncing;

    [ObservableProperty]
    private string _syncStatusText = "";

    [ObservableProperty]
    private string _syncStatusIcon = "✅";

    [ObservableProperty]
    private Color _syncStatusColor = Colors.Green;

    [ObservableProperty]
    private DateTime? _lastSyncTime;

    public SyncIndicatorViewModel(SyncService syncService)
    {
        _syncService = syncService;

        // Inscrever nos eventos de sincronização
        _syncService.SyncStarted += OnSyncStarted;
        _syncService.SyncCompleted += OnSyncCompleted;
        _syncService.SyncFailed += OnSyncFailed;

        UpdateStatus();
    }

    private void OnSyncStarted(object? sender, SyncEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            IsSyncing = true;
            SyncStatusText = "Sincronizando...";
            SyncStatusIcon = "🔄";
            SyncStatusColor = Colors.Orange;
        });
    }

    private void OnSyncCompleted(object? sender, SyncEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            IsSyncing = false;
            LastSyncTime = e.CompletedAt;
            SyncStatusText = $"Sincronizado • {e.ItemsSynced} itens";
            SyncStatusIcon = "✅";
            SyncStatusColor = Colors.Green;

            // Voltar ao status normal após 3 segundos
            Task.Delay(3000).ContinueWith(_ =>
            {
                MainThread.BeginInvokeOnMainThread(UpdateStatus);
            });
        });
    }

    private void OnSyncFailed(object? sender, SyncEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            IsSyncing = false;
            SyncStatusText = "Erro na sincronização";
            SyncStatusIcon = "❌";
            SyncStatusColor = Colors.Red;

            // Voltar ao status normal após 5 segundos
            Task.Delay(5000).ContinueWith(_ =>
            {
                MainThread.BeginInvokeOnMainThread(UpdateStatus);
            });
        });
    }

    private void UpdateStatus()
    {
        var status = _syncService.GetStatus();

        if (status.IsSyncing)
        {
            SyncStatusText = "Sincronizando...";
            SyncStatusIcon = "🔄";
            SyncStatusColor = Colors.Orange;
        }
        else if (!status.HasInternet)
        {
            SyncStatusText = "Sem conexão";
            SyncStatusIcon = "📡";
            SyncStatusColor = Colors.Gray;
        }
        else if (status.LastSyncAt.HasValue)
        {
            var timeAgo = DateTime.UtcNow - status.LastSyncAt.Value;
            SyncStatusText = timeAgo.TotalMinutes < 1 ? "Sincronizado agora" :
                            timeAgo.TotalHours < 1 ? $"Sincronizado há {(int)timeAgo.TotalMinutes}min" :
                            timeAgo.TotalDays < 1 ? $"Sincronizado há {(int)timeAgo.TotalHours}h" :
                            $"Sincronizado há {(int)timeAgo.TotalDays}d";
            SyncStatusIcon = "✅";
            SyncStatusColor = Colors.Green;
        }
        else
        {
            SyncStatusText = "Nunca sincronizado";
            SyncStatusIcon = "⚠️";
            SyncStatusColor = Colors.Orange;
        }
    }
}

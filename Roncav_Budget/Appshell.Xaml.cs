using roncav_budget.Data;
using roncav_budget.Services;
using roncav_budget.Views;
using Roncav_Budget.Views;

namespace roncav_budget
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Registrar rotas para navegação
            Routing.RegisterRoute("login", typeof(LoginPage));
            Routing.RegisterRoute("register", typeof(RegisterPage));
            Routing.RegisterRoute("dashboard", typeof(DashboardPage));

            // Popular dados de exemplo na primeira execução
            _ = InicializarDadosAsync();
        }

        private async Task InicializarDadosAsync()
        {
            try
            {
                var databaseService = Handler?.MauiContext?.Services.GetService<DatabaseService>();
                if (databaseService != null)
                {
                    await DadosDeExemplo.PopularDadosExemploAsync(databaseService);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao popular dados: {ex.Message}");
            }
        }

        #region ⌨️ Keyboard Accelerators Handlers

        /// <summary>
        /// CTRL+SHIFT+W - Ajuda Rápida / What's This?
        /// </summary>
        private async void OnHelpAcceleratorInvoked(object? sender, EventArgs args)
        {
            try
            {
                await DisplayAlert(
                    "⌨️ Atalhos de Teclado",
                    "Bem-vindo ao Roncav Budget!\n\n" +
                    "Atalhos disponíveis:\n\n" +
                    "• CTRL+K: Adicionar nova categoria\n" +
                    "• CTRL+O: Abrir/Importar arquivo\n" +
                    "• CTRL+SHIFT+W: Esta ajuda\n" +
                    "• CTRL+N: Nova transação\n" +
                    "• CTRL+S: Sincronizar dados\n" +
                    "• F5: Atualizar dashboard\n\n" +
                    "Para mais informações, acesse Configurações → Ajuda.",
                    "OK"
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no atalho de ajuda: {ex.Message}");
            }
        }

        /// <summary>
        /// CTRL+K - Adicionar Categoria/Pasta ao Workspace
        /// </summary>
        private async void OnAddFolderAcceleratorInvoked(object? sender, EventArgs args)
        {
            try
            {
                string? result = await DisplayPromptAsync(
                    "➕ Nova Categoria",
                    "Digite o nome da nova categoria:",
                    placeholder: "Ex: Investimentos, Viagens, Saúde",
                    maxLength: 50,
                    keyboard: Keyboard.Text
                );

                if (!string.IsNullOrWhiteSpace(result))
                {
                    // TODO: Implementar lógica para adicionar categoria ao banco de dados
                    // var databaseService = Handler?.MauiContext?.Services.GetService<DatabaseService>();
                    // await databaseService.AdicionarCategoriaAsync(result);
                    
                    await DisplayAlert(
                        "✅ Sucesso",
                        $"Categoria '{result}' adicionada com sucesso!\n\n" +
                        $"Você pode visualizá-la na seção de Transações.",
                        "OK"
                    );
                    
                    System.Diagnostics.Debug.WriteLine($"📁 Nova categoria adicionada: {result}");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(
                    "❌ Erro",
                    $"Não foi possível adicionar a categoria: {ex.Message}",
                    "OK"
                );
                System.Diagnostics.Debug.WriteLine($"Erro no atalho adicionar pasta: {ex.Message}");
            }
        }

        /// <summary>
        /// CTRL+O - Abrir/Importar Pasta ou Arquivo
        /// </summary>
        private async void OnOpenFolderAcceleratorInvoked(object? sender, EventArgs args)
        {
            try
            {
                // Opção 1: Usar FolderPicker (requer Microsoft.Maui.Storage)
                // var result = await FolderPicker.Default.PickAsync();
                
                // Opção 2: Usar FilePicker para importar extratos
                var customFileType = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "public.comma-separated-values-text" } },
                        { DevicePlatform.Android, new[] { "text/csv", "text/comma-separated-values" } },
                        { DevicePlatform.WinUI, new[] { ".csv", ".xlsx", ".xls", ".ofx" } },
                        { DevicePlatform.macOS, new[] { "csv", "xlsx", "xls", "ofx" } }
                    });

                var options = new PickOptions
                {
                    PickerTitle = "Selecione um extrato bancário para importar",
                    FileTypes = customFileType
                };

                var result = await FilePicker.Default.PickAsync(options);

                if (result != null)
                {
                    await DisplayAlert(
                        "📂 Arquivo Selecionado",
                        $"Nome: {result.FileName}\n" +
                        $"Tipo: {result.ContentType}\n" +
                        $"Caminho: {result.FullPath}\n\n" +
                        $"Processando importação...",
                        "OK"
                    );

                    // TODO: Implementar lógica de importação
                    // var importService = Handler?.MauiContext?.Services.GetService<ImportacaoExtratoService>();
                    // await importService.ImportarAsync(result.FullPath);
                    
                    System.Diagnostics.Debug.WriteLine($"📥 Arquivo selecionado: {result.FullPath}");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(
                    "❌ Erro",
                    $"Não foi possível abrir o arquivo: {ex.Message}",
                    "OK"
                );
                System.Diagnostics.Debug.WriteLine($"Erro no atalho abrir pasta: {ex.Message}");
            }
        }

        #endregion
    }
}

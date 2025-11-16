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
    }
}

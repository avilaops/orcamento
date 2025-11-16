using Roncav_Budget.Views;

namespace roncav_budget
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Verificar se o usuário está autenticado
            var isAuthenticated = SecureStorage.Default.GetAsync("access_token").Result != null;

            if (isAuthenticated)
            {
                return new Window(new AppShell());
            }
            else
            {
                // Se não estiver autenticado, mostrar tela de login
                return new Window(new NavigationPage(Handler!.MauiContext!.Services.GetService<LoginPage>()!));
            }
        }
    }
}

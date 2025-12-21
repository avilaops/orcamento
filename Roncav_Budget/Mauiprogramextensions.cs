using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Plugin.LocalNotification;
using roncav_budget.Services;
using roncav_budget.Services.Avila;
using roncav_budget.ViewModels;
using Roncav_Budget.ViewModels;
using roncav_budget.Views;
using Roncav_Budget.Views;

namespace roncav_budget
{
    public static class MauiProgramExtensions
    {
        public static MauiAppBuilder UseSharedMauiApp(this MauiAppBuilder builder)
        {
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseSkiaSharp()
                .UseLocalNotification()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Configurar LiveCharts
            LiveCharts.Configure(config =>
                config
                    .AddSkiaSharp()
                    .AddDefaultMappers()
                    .AddLightTheme());

            // Registrar serviços
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<ImportacaoExtratoService>();
            builder.Services.AddSingleton<RelatorioService>();
            builder.Services.AddSingleton<CacheService>();
            builder.Services.AddSingleton<LoggingService>();
            builder.Services.AddSingleton<ExportService>();
            builder.Services.AddSingleton<ThemeService>();
            builder.Services.AddSingleton<NotificationService>();
            builder.Services.AddSingleton<BackupService>();

            // Serviços de integração Avila
            builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
            builder.Services.AddSingleton<ISecureStorage>(SecureStorage.Default);
            builder.Services.AddSingleton<IPreferences>(Preferences.Default);

            builder.Services.AddHttpClient<AvilaApiService>();
            builder.Services.AddSingleton<SyncService>();
            
            // Serviço de tratamento de erros global
            builder.Services.AddSingleton<ErrorHandlingService>();

            // Registrar ViewModels
            builder.Services.AddSingleton<SyncIndicatorViewModel>();
            builder.Services.AddTransient<ChartDataViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<DashboardEnhancedViewModel>();
            builder.Services.AddTransient<TransacoesViewModel>();
            builder.Services.AddTransient<ContasViewModel>();
            builder.Services.AddTransient<OrcamentosViewModel>();
            builder.Services.AddTransient<MetasViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();

            // Registrar Pages
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<DashboardEnhancedPage>();
            builder.Services.AddTransient<TransacoesPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<SettingsPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder;
        }
    }
}

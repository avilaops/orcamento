using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.iOSOption;

namespace roncav_budget.Services;

/// <summary>
/// Serviço de notificações locais e agendadas
/// </summary>
public class NotificationService
{
    private readonly IPreferences _preferences;
    private readonly LoggingService _logger;

    public NotificationService(IPreferences preferences, LoggingService logger)
    {
        _preferences = preferences;
        _logger = logger;
    }

    /// <summary>
    /// Inicializa o serviço de notificações
    /// </summary>
    public async Task InitializeAsync()
    {
        // Solicitar permissão
        if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
        {
            await LocalNotificationCenter.Current.RequestNotificationPermission();
        }
    }

    /// <summary>
    /// Envia notificação imediata
    /// </summary>
    public async Task ShowNotificationAsync(string title, string message, int notificationId = 0)
    {
        var notification = new NotificationRequest
        {
            NotificationId = notificationId,
            Title = title,
            Description = message,
            BadgeNumber = 1,
            CategoryType = NotificationCategoryType.Status
        };

        await LocalNotificationCenter.Current.Show(notification);
        await _logger.LogInfoAsync($"Notificação enviada: {title}", "NotificationService");
    }

    /// <summary>
    /// Agenda notificação para o futuro
    /// </summary>
    public async Task ScheduleNotificationAsync(
        string title, 
        string message, 
        DateTime scheduleDate,
        int notificationId)
    {
        var notification = new NotificationRequest
        {
            NotificationId = notificationId,
            Title = title,
            Description = message,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = scheduleDate,
                RepeatType = NotificationRepeat.No
            },
            BadgeNumber = 1,
            CategoryType = NotificationCategoryType.Alarm
        };

        await LocalNotificationCenter.Current.Show(notification);
        await _logger.LogInfoAsync($"Notificação agendada para: {scheduleDate}", "NotificationService");
    }

    /// <summary>
    /// Cancela notificação específica
    /// </summary>
    public void CancelNotification(int notificationId)
    {
        LocalNotificationCenter.Current.Cancel(notificationId);
    }

    /// <summary>
    /// Cancela todas as notificações
    /// </summary>
    public void CancelAllNotifications()
    {
        LocalNotificationCenter.Current.CancelAll();
    }

    #region Notificações Específicas do App

    /// <summary>
    /// Notificação de orçamento excedido
    /// </summary>
    public async Task NotifyBudgetExceededAsync(string categoryName, decimal amount, decimal limit)
    {
        if (!_preferences.Get("BudgetAlertsEnabled", true))
            return;

        await ShowNotificationAsync(
            "⚠️ Orçamento Excedido!",
            $"Categoria {categoryName}: R$ {amount:N2} de R$ {limit:N2}",
            1001
        );
    }

    /// <summary>
    /// Notificação de meta atingida
    /// </summary>
    public async Task NotifyGoalAchievedAsync(string goalName, decimal amount)
    {
        if (!_preferences.Get("GoalRemindersEnabled", true))
            return;

        await ShowNotificationAsync(
            "🎉 Meta Atingida!",
            $"Parabéns! Você alcançou a meta '{goalName}' de R$ {amount:N2}",
            1002
        );
    }

    /// <summary>
    /// Notificação de lembrete de transação recorrente
    /// </summary>
    public async Task NotifyRecurringTransactionAsync(string description, decimal amount, DateTime dueDate)
    {
        await ScheduleNotificationAsync(
            "📅 Transação Recorrente",
            $"{description} - R$ {amount:N2}",
            dueDate.AddHours(-2), // 2 horas antes
            2000 + dueDate.Day
        );
    }

    /// <summary>
    /// Notificação diária de resumo
    /// </summary>
    public async Task ScheduleDailySummaryAsync()
    {
        var tomorrow = DateTime.Today.AddDays(1).AddHours(20); // 20h do dia seguinte

        await ScheduleNotificationAsync(
            "📊 Resumo do Dia",
            "Veja como foi seu dia financeiro!",
            tomorrow,
            3000
        );
    }

    /// <summary>
    /// Notificação de sincronização pendente
    /// </summary>
    public async Task NotifySyncPendingAsync(int pendingItems)
    {
        await ShowNotificationAsync(
            "🔄 Sincronização Pendente",
            $"Você tem {pendingItems} itens não sincronizados. Conecte-se à internet!",
            4000
        );
    }

    /// <summary>
    /// Notificação de backup recomendado
    /// </summary>
    public async Task NotifyBackupRecommendedAsync()
    {
        await ShowNotificationAsync(
            "💾 Faça um Backup!",
            "Faz tempo que você não faz backup dos seus dados.",
            5000
        );
    }

    #endregion
}

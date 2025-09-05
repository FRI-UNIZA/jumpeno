namespace Jumpeno.Client.Components;

#pragma warning disable CS8618

using AntDesign;

public partial class Notification {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    // Class:
    public const string CLASS_ARIA = "ant-notification-aria-label";
    public const string CLASS_SPACE = "ant-notification-message-space";
    // Delay:
    public const int ARIA_DELAY_START = 0;
    public const int ARIA_DELAY_INCREMENT = 1000;

    // Settings ---------------------------------------------------------------------------------------------------------------------------
    private readonly INotificationService Service;
    private const int DEFAULT_DURATION = 3000;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly List<NotificationData> AriaNotifications = [];
    private readonly LockerSlim DisplayLock = new();
    private Delay? AriaDelay;
    private int AriaDelayTime = ARIA_DELAY_START;

    // Server notifications ---------------------------------------------------------------------------------------------------------------
    [Inject]
    public required PersistentComponentState ApplicationState { private get; set; }
    private PersistingComponentStateSubscription PersistingSubscription;
    private Task PersistData() {
        ApplicationState.PersistAsJson(nameof(GetServerNotifications), new { ServerNotifications = GetServerNotifications() });
        return Task.CompletedTask;
    }

    public static List<NotificationData> GetServerNotifications() {
        var notifications = RequestStorage.Get<List<NotificationData>>(nameof(GetServerNotifications));
        return notifications is null ? [] : notifications;
    }
    public static void SetServerNotifications(List<NotificationData> notifications) {
        RequestStorage.Set(nameof(GetServerNotifications), notifications);
    }
    public static void AddServerNotification(NotificationData notification) {
        List<NotificationData> notifications = GetServerNotifications();
        notifications.Add(notification);
        SetServerNotifications(notifications);
    }

    protected sealed override void OnComponentInitialized() {
        PersistingSubscription = ApplicationState.RegisterOnPersisting(PersistData);
        if (ApplicationState.TryTakeFromJson<NotificationState>(nameof(GetServerNotifications), out var restored)) {
            SetServerNotifications(restored!.ServerNotifications);
        }
    }
    protected override void OnComponentAfterRender(bool firstRender) {
        if (!firstRender) return;
        foreach (var notification in GetServerNotifications()) {
            Display(notification);
        }
    }
    protected override void OnComponentDispose() {
        DisplayLock.Dispose();
        PersistingSubscription.Dispose();
    }

    // Instance ---------------------------------------------------------------------------------------------------------------------------
    public Notification() : base() {
        if (AppEnvironment.IsServer) return;
        Service = AppEnvironment.GetService<INotificationService>();
    }

    private void StartAriaDisposer() {
        Delay.Clear(AriaDelay);
        if (AriaDelayTime <= int.MaxValue - ARIA_DELAY_INCREMENT) AriaDelayTime += ARIA_DELAY_INCREMENT;
        AriaDelay = Delay.Set(async () => {
            await DisplayLock.TryExclusive(() => {
                AriaNotifications.Clear();
                StateHasChanged();
                AriaDelayTime = ARIA_DELAY_START;
            });
        }, AriaDelayTime);
    }

    private async void DisplayAriaNotification(
        NotificationType type,
        string message,
        string description,
        int? duration
    ) {
        await DisplayLock.TryExclusive(() => {
            AriaNotifications.Add(new NotificationData(
                type, message, description, duration
            ));
            StartAriaDisposer();
            StateHasChanged();
        });
    }

    // Open -------------------------------------------------------------------------------------------------------------------------------
    public static void Display(NotificationData notification) {
        switch (notification.Type) {
            case NotificationType.None:
                None(notification.Title, notification.Message, notification.Duration);
            break;
            case NotificationType.Success:
                Success(notification.Title, notification.Message, notification.Duration);
            break;
            case NotificationType.Error:
                Error(notification.Title, notification.Message, notification.Duration);
            break;
            case NotificationType.Warning:
                Warning(notification.Title, notification.Message, notification.Duration);
            break;
            case NotificationType.Info:
                Info(notification.Title, notification.Message, notification.Duration);
            break;
        } 
    }
    private static RenderFragment CreateDescriptionFragment(string description) => builder => {
        var sequence = 0;
        builder.OpenElement(sequence++, "div");
        builder.AddAttribute(sequence++, "class", CLASS_SPACE);
        builder.AddContent(sequence++, description);
        builder.CloseElement();
    };
    private static async void OpenNotification(NotificationConfig config, int? duration) {
        if (AppEnvironment.IsServer) {
            AddServerNotification(
                new NotificationData(
                    config.NotificationType,
                    config.Message.AsT0,
                    config.Description.AsT0,
                    duration
                )
            );
            return;
        }
        if (duration is not null && duration < 0) {
            throw new Exception("Minimal duration is 0ms (infinite)!");
        }
        config.Duration = 0;
        config.Key = Guid.NewGuid().ToString();
        config.ClassName = SURFACE.FLOATING.CSSClass();
        var instance = Instance();
        instance.DisplayAriaNotification(
            config.NotificationType, config.Message.AsT0.Trim(), config.Description.AsT0.Trim(), (int?) config.Duration
        );
        if (config.Description.IsT0 && config.Description.AsT0.Trim() != "") {
            config.Description = CreateDescriptionFragment(config.Description.AsT0);
        } else {
            config.Description = "";
        }
        await instance.Service.Open(config);
        await JS.InvokeVoidAsync(JSNotification.Open, config.Key, duration is null ? DEFAULT_DURATION : duration);
    }
    // Close ------------------------------------------------------------------------------------------------------------------------------
    [JSInvokable]
    public static void JS_Close(string key) => Instance().Service.Close(key);

    // Display notification ---------------------------------------------------------------------------------------------------------------
    public static void None(string title, string message, int? duration) {
        OpenNotification(new NotificationConfig() {
            Message = title,
            Description = message,
            NotificationType = NotificationType.None
        }, duration);
    }
    public static void None(string title, string message) => None(title, message, null);
    public static void None(string message, int? duration) => None(I18N.T("Notification"), message, duration);
    public static void None(string message) => None(message, (int?) null);

    public static void Success(string title, string message, int? duration) {
        OpenNotification(new NotificationConfig() {
            Message = title,
            Description = message,
            NotificationType = NotificationType.Success
        }, duration);
    }
    public static void Success(string title, string message) => Success(title, message, null);
    public static void Success(string message, int? duration) => Success(I18N.T("Success"), message, duration);
    public static void Success(string message) => Success(message, (int?) null);
    
    public static void Error(string title, string message, int? duration) {
        OpenNotification(new NotificationConfig() {
            Message = title,
            Description = message,
            NotificationType = NotificationType.Error
        }, duration);
    }
    public static void Error(string title, string message) => Error(title, message, null);
    public static void Error(string message, int? duration) => Error(I18N.T("Error occurred"), message, duration);
    public static void Error(string message) => Error(message, (int?) null);

    public static void Warning(string title, string message, int? duration) {
        OpenNotification(new NotificationConfig() {
            Message = title,
            Description = message,
            NotificationType = NotificationType.Warning
        }, duration);
    }
    public static void Warning(string title, string message) => Warning(title, message, null);
    public static void Warning(string message, int? duration) => Warning(I18N.T("Warning"), message, duration);
    public static void Warning(string message) => Warning(message, (int?) null);

    public static void Info(string title, string message, int? duration) {
        OpenNotification(new NotificationConfig() {
            Message = title,
            Description = message,
            NotificationType = NotificationType.Info
        }, duration);
    }
    public static void Info(string title, string message) => Info(title, message, null);
    public static void Info(string message, int? duration) => Info(I18N.T("Information"), message, duration);
    public static void Info(string message) => Info(message, (int?) null);
}

namespace Jumpeno.Client.Components;

#pragma warning disable CS8618

public partial class Notification {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    // Duration:
    public const int DEFAULT_DURATION = 3000;
    // ARIA:
    private const int ARIA_DELAY_START = 0;
    private const int ARIA_DELAY_INCREMENT = 1000;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    // Notifications:
    private readonly Dictionary<string, NotificationData> Notifications = [];
    private readonly List<NotificationData> NotificationList = [];
    private readonly Dictionary<string, NotificationData> Closing = [];
    private readonly Dictionary<string, Delay> Delays = [];
    private readonly LockerSlim DisplayLock = new();
    // ARIA:
    private readonly List<NotificationData> AriaNotifications = [];
    private int AriaDelayTime = ARIA_DELAY_START;
    private Delay? AriaDelay = null;
    private readonly LockerSlim AriaLock = new();

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    private CSSClass GridClass(NotificationData notification) {
        return new CSSClass("notification-grid")
        .Set("closing", Closing.ContainsKey(notification.Key));
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentAfterRender(bool firstRender) {
        if (!firstRender) return;
        foreach (var notification in SSRStorage.State.ServerNotifications) {
            Open(notification);
        }
        SSRStorage.State.ServerNotifications.Clear();
    }

    protected override void OnComponentDispose() {
        DisplayLock.Dispose();
        AriaLock.Dispose();
    }

    // ARIA -------------------------------------------------------------------------------------------------------------------------------
    private void StartAriaDisposer() {
        Delay.Clear(AriaDelay);
        if (AriaDelayTime <= int.MaxValue - ARIA_DELAY_INCREMENT) AriaDelayTime += ARIA_DELAY_INCREMENT;
        AriaDelay = Delay.Set(async () => {
            await AriaLock.TryExclusive(() => {
                AriaNotifications.Clear();
                StateHasChanged();
                AriaDelayTime = ARIA_DELAY_START;
            });
        }, AriaDelayTime);
    }

    private async void DisplayAriaNotification(NotificationData notification) {
        await AriaLock.TryExclusive(() => {
            AriaNotifications.Add(notification);
            StartAriaDisposer();
        });
    }

    // Delay ------------------------------------------------------------------------------------------------------------------------------
    private void SetDelay(NotificationData notification) {
        if (Delays.ContainsKey(notification.Key)) return;
        Delays[notification.Key] = Delay.Set(
            async () => await Close(notification), notification.Duration ?? DEFAULT_DURATION
        );
    }

    private void ClearDelay(NotificationData notification) {
        if (!Delays.TryGetValue(notification.Key, out Delay? delay)) return;
        Delay.Clear(delay);
        Delays.Remove(notification.Key);
    }

    // Display ----------------------------------------------------------------------------------------------------------------------------
    private static async void Open(NotificationData notification) {
        var instance = Instance();
        if (AppEnvironment.IsServer) {
            SSRStorage.State.ServerNotifications.Add(notification);
            return;
        }
        if (notification.Duration is not null && notification.Duration < 1) {
            throw new Exception("Minimal duration is 1ms!");
        }
        await instance.DisplayLock.Exclusive(() => {
            instance.Notifications[notification.Key] = notification;
            instance.NotificationList.Add(notification);
            instance.SetDelay(notification);
            instance.DisplayAriaNotification(notification);
            instance.Notify();
        });
    }

    private async Task StopDelay(NotificationData notification) {
        await DisplayLock.Exclusive(() => {
            if (!Notifications.ContainsKey(notification.Key)) return;
            if (Closing.ContainsKey(notification.Key)) return;
            ClearDelay(notification);
        });
    }

    private async Task RestartDelay(NotificationData notification) {
        await DisplayLock.Exclusive(() => {
            if (!Notifications.ContainsKey(notification.Key)) return;
            if (Closing.ContainsKey(notification.Key)) return;
            SetDelay(notification);
        });
    }

    private async Task Close(NotificationData notification) {
        await DisplayLock.Exclusive(() => {
            if (!Notifications.ContainsKey(notification.Key)) return;
            if (Closing.ContainsKey(notification.Key)) return;
            ClearDelay(notification);
            Closing[notification.Key] = notification;
            Notify();
        });
    }

    private async Task OnClose(NotificationData notification) {
        await DisplayLock.Exclusive(() => {
            if (!Closing.ContainsKey(notification.Key)) return;
            Notifications.Remove(notification.Key);
            NotificationList.Remove(notification);
            Closing.Remove(notification.Key);
            Notify();
        });
    }

    // Open -------------------------------------------------------------------------------------------------------------------------------
    public static void Basic(string title, string message, int? duration = null) => Open(new(NOTIFICATION_TYPE.BASIC, title, message, duration));
    public static void Basic(string message, int? duration = null) => Basic(I18N.T("Notification"), message, duration);

    public static void Success(string title, string message, int? duration = null) => Open(new(NOTIFICATION_TYPE.SUCCESS, title, message, duration));
    public static void Success(string message, int? duration = null) => Success(I18N.T("Success"), message, duration);
    
    public static void Error(string title, string message, int? duration = null) => Open(new(NOTIFICATION_TYPE.ERROR, title, message, duration));
    public static void Error(string message, int? duration = null) => Error(I18N.T("Error occurred"), message, duration);

    public static void Warning(string title, string message, int? duration = null) => Open(new(NOTIFICATION_TYPE.WARNING, title, message, duration));
    public static void Warning(string message, int? duration = null) => Warning(I18N.T("Warning"), message, duration);

    public static void Info(string title, string message, int? duration = null) => Open(new(NOTIFICATION_TYPE.INFO, title, message, duration));
    public static void Info(string message, int? duration = null) => Info(I18N.T("Information"), message, duration);
}

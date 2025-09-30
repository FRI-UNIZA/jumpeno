namespace Jumpeno.Client.Models;

[method: JsonConstructor]
public class NotificationData(string key, NOTIFICATION_TYPE type, string title, string message, int? duration) {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public string Key { get; private set; } = key;
    public NOTIFICATION_TYPE Type { get; private set; } = type;
    public string Title { get; private set; } = title;
    public string Message { get; private set; } = message;
    public int? Duration { get; private set; } = duration;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public NotificationData(NOTIFICATION_TYPE type, string title, string message, int? duration) : this(
        Guid.NewGuid().ToString(), type, title, message, duration
    ) {}
}

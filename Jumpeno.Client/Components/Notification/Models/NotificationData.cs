namespace Jumpeno.Client.Models;

using AntDesign;

public class NotificationData(NotificationType type, string title, string message, int? duration) {
    public NotificationType Type { get; private set; } = type;
    public string Title { get; private set; } = title;
    public string Message { get; private set; } = message;
    public int? Duration { get; private set; } = duration;
}

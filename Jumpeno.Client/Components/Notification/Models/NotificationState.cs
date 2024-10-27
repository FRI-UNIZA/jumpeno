namespace Jumpeno.Client.Models;

public class NotificationState(List<NotificationData> serverNotifications) {
    public List<NotificationData> ServerNotifications { get; private set; } = serverNotifications;
}

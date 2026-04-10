namespace Jumpeno.Client.Constants;

public enum NotificationType {
    [CSSClass("notification-basic")] BASIC,
    [CSSClass("notification-success")] SUCCESS,
    [CSSClass("notification-error")] ERROR,
    [CSSClass("notification-warning")] WARNING,
    [CSSClass("notification-info")] INFO
}

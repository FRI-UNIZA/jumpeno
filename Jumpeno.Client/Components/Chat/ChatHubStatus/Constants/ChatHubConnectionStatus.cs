namespace Jumpeno.Client.Constants;

public enum ChatHubConnectionStatus {
    [CSSClass("status-connecting")] Connecting,
    [CSSClass("status-connected")] Connected,
    [CSSClass("status-reconnecting")] Reconnecting,
    [CSSClass("status-disconnected")] Disconnected,
    [CSSClass("status-error")] Error
}

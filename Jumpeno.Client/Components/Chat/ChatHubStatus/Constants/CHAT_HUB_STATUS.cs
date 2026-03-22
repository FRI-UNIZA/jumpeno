namespace Jumpeno.Client.Constants;

public enum CHAT_HUB_STATUS {
    [CSSClass("status-connecting")] CONNECTING,
    [CSSClass("status-connected")] CONNECTED,
    [CSSClass("status-reconnecting")] RECONNECTING,
    [CSSClass("status-disconnected")] DISCONNECTED,
    [CSSClass("status-error")] ERROR
}

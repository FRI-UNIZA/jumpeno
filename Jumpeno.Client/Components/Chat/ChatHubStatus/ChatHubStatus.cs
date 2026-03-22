namespace Jumpeno.Client.Components;

public partial class ChatHubStatus {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "chat-hub-status";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public CHAT_HUB_STATUS Status { get; set; } = CHAT_HUB_STATUS.CONNECTING;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass()
        .Set(CLASS, Base)
        .SetVariant(Status);

    private string Icon => Status switch {
        CHAT_HUB_STATUS.CONNECTED    => "check-circle",
        CHAT_HUB_STATUS.RECONNECTING => "loading",
        CHAT_HUB_STATUS.DISCONNECTED => "disconnect",
        _                            => "loading"
    };

    private string Label => Status switch {
        CHAT_HUB_STATUS.CONNECTED    => I18N.T("Connected"),
        CHAT_HUB_STATUS.RECONNECTING => I18N.T("Reconnecting"),
        CHAT_HUB_STATUS.DISCONNECTED => I18N.T("Disconnected"),
        _                            => I18N.T("Connecting")
    };
}

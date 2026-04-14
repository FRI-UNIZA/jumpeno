namespace Jumpeno.Client.Components;

public partial class ChatHubStatus {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string Class = "chat-hub-status";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public ChatHubConnectionStatus Status { get; set; } = ChatHubConnectionStatus.Connecting;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass()
        .Set(Class, Base)
        .SetVariant(Status);

    private string Icon => Status switch {
        ChatHubConnectionStatus.Connected    => "check-circle",
        ChatHubConnectionStatus.Reconnecting => "loading",
        ChatHubConnectionStatus.Disconnected => "disconnect",
        _                                    => "loading"
    };

    private string Label => Status switch {
        ChatHubConnectionStatus.Connected    => I18N.T("Connected"),
        ChatHubConnectionStatus.Reconnecting => I18N.T("Reconnecting"),
        ChatHubConnectionStatus.Disconnected => I18N.T("Disconnected"),
        _                                    => I18N.T("Connecting")
    };
}

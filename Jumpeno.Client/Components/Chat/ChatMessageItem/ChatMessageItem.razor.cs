namespace Jumpeno.Client.Components;

public partial class ChatMessageItem {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string Class = "chat-message-item";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required ChatMessage Message { get; set; }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(Class, Base);

    private string FormattedTime => Message.SentAt.ToLocalTime().ToString("HH:mm");
}

namespace Jumpeno.Client.Components;

public partial class GlobalChat : IAsyncDisposable {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string Class = "global-chat";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required GlobalChatViewModel VM { get; set; }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(Class, Base);
}

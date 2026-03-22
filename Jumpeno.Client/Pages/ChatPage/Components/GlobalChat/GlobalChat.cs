namespace Jumpeno.Client.Components;

public partial class GlobalChat : IAsyncDisposable {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "global-chat";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required GlobalChatViewModel VM { get; set; }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);
}

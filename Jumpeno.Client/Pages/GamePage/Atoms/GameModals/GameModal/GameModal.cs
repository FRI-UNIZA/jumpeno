namespace Jumpeno.Client.Components;

public partial class GameModal {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string CLASS = "game-modal";
    
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required GameViewModel VM { get; set; }

    // Events -----------------------------------------------------------------------------------------------------------------------------
    public override async Task CallOnOpenStart() { await base.CallOnOpenStart(); await VM.SwitchToWebInput(); }
    public override async Task CallOnCloseFinish() { await base.CallOnCloseFinish(); await VM.SwitchToGameInput(); }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override async ValueTask OnComponentDisposeAsync() => await Close();
}

namespace Jumpeno.Client.Components;

public partial class ModalClose {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASSNAME = "modal-close";
    public const string CLASSNAME_ICON = "modal-close-icon";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required Modal Modal { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private bool? LastUnclosable;
    private string ComputeID() => $"{Modal.ID}-{CLASSNAME}";
    private CSSClass ComputeClass() {
        var c = new CSSClass(CLASSNAME);
        if (Modal.Unclosable) c.Set("unclosable");
        return c;
    }
    private bool ComputeInert() => Modal.Unclosable;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentParametersSet(bool firstTime) {
        if (Modal.Unclosable && LastUnclosable == false) {
            JS.InvokeVoid(JSModal.AdaptCloseFocus, Modal.ID, ComputeID());
        }
        LastUnclosable = Modal.Unclosable;
    }
}

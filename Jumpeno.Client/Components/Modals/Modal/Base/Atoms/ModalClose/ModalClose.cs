namespace Jumpeno.Client.Components;

public partial class ModalClose {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "modal-close";
    public const string CLASS_ICON = "modal-close-icon";
    public static string CLASS_UNCLOSABLE => ModalElement.CLASS_UNCLOSABLE;

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required Modal Modal { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private bool? LastUnclosable;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    private string ComputeID() => $"{Modal.ID}-{CLASS}";
    private bool ComputeInert() => Modal.Unclosable;
    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(CLASS, Base)
        .Set(CLASS_UNCLOSABLE, Modal.Unclosable);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentParametersSet(bool firstTime) {
        if (Modal.Unclosable && LastUnclosable == false) {
            JS.InvokeVoid(JSModal.AdaptCloseFocus, Modal.ID, ComputeID());
        }
        LastUnclosable = Modal.Unclosable;
    }
}

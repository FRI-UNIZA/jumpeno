namespace Jumpeno.Client.Components;

public partial class ModalClose {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "modal-close";
    public const string ClassIcon = "modal-close-icon";
    public static string ClassUnclosable => Modal.ClassUnclosable;

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required Modal Modal { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private bool? LastUnclosable;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    private string ComputeID() => $"{Modal.Id}-{ClassName}";
    private bool ComputeInert() => Modal.Unclosable;
    public override CssClass ComputeClass() {
        return base.ComputeClass()
        .Set(ClassName, Base)
        .Set(ClassUnclosable, Modal.Unclosable);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentParametersSet(bool firstTime) {
        if (Modal.Unclosable && LastUnclosable == false) {
            JS.InvokeVoid(JSModal.AdaptCloseFocus, Modal.Id, ComputeID());
        }
        LastUnclosable = Modal.Unclosable;
    }
}

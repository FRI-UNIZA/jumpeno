namespace Jumpeno.Client.Atoms;

public partial class ModalHeading {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASSNAME = "modal-heading";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}

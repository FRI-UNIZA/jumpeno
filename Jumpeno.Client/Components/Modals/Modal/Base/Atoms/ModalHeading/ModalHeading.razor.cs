namespace Jumpeno.Client.Components;

public partial class ModalHeading {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "modal-heading";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}

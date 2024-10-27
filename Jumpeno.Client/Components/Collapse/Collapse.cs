
namespace Jumpeno.Client.Components;

public partial class Collapse {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS_COLLAPSE = "collapse";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    protected CSSClass ComputeClass() {
        return ComputeClass(CLASS_COLLAPSE);
    }
}

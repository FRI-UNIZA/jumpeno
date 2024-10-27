
namespace Jumpeno.Client.Atoms;

public partial class InputLabel {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS_INPUT_LABEL = "input-label";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required string For { get; set; }
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private CSSClass ComputeClass() {
        return ComputeClass(CLASS_INPUT_LABEL);
    }
}


namespace Jumpeno.Client.Components;

public partial class InputLabel {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS_INPUT_LABEL = "input-label";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required string For { get; set; }
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private CSSClass ComputeClass() => ComputeClass(CLASS_INPUT_LABEL);
}

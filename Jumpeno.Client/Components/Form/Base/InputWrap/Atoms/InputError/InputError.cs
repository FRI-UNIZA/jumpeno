namespace Jumpeno.Client.Atoms;

public partial class InputError {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS_INPUT_ERROR = "input-error";
    public const string CLASS_INPUT_ERROR_OUTLINE = "input-error-outline";
    public const string CLASS_INPUT_ERROR_MESSAGE = "input-error-message";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required InputErrorViewModel ViewModel { get; set; }
    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }
    [Parameter]
    public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private CSSClass ComputeClass() {
        return ComputeClass(CLASS_INPUT_ERROR);
    }
}

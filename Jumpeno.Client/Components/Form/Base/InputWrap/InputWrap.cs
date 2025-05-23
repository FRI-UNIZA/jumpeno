namespace Jumpeno.Client.Components;

using System.Reflection;

public partial class InputWrap {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS_INPUT_WRAP = "input-wrap";
    public const string CLASS_ERROR = "error";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required InputErrorViewModel ErrorVM { get; set; } 
    [Parameter]
    public required string FormID { get; set; }
    [Parameter]
    public required string Label { get; set; }
    [Parameter]
    public bool HideLabel { get; set; } = false;
    [Parameter]
    public INPUT_ALIGN Align { get; set; } = INPUT_ALIGN.LEFT;
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    protected CSSClass ComputeClass() {
        var c = new CSSClass(CLASS_INPUT_WRAP);
        if (ErrorVM.HasError) c.Set(CLASS_ERROR);
        c.Set($"align-{Align.StringLower()}");
        return c;
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnParametersSet() {
        ErrorVM.GetType().GetField("Notify", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(ErrorVM, () => {
            StateHasChanged();
        });
    }

    // Events -----------------------------------------------------------------------------------------------------------------------------
    private void HandleErrorClick(MouseEventArgs e) {
        ActionHandler.SetFocus(FormID);
    }

    private void HandleErrorKeyDown(KeyboardEventArgs e) {
        if (e.Key == KEYBOARD.ENTER) {
            ActionHandler.SetFocus(FormID);
        }
    }
}

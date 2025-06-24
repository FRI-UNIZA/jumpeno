namespace Jumpeno.Client.Components;

public partial class FormWrap {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS_INPUT_WRAP = "form-wrap";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    // Type:
    [Parameter]
    public required ERROR_TYPE Type { get; set; }
    // References:
    [Parameter]
    public required FormViewModel ViewModel { get; set; }
    [Parameter]
    public required dynamic Component { get; set; }
    // Cursor:
    [Parameter]
    public required RenderFragment? DisabledCursor { get; set; } = null;
    // Content:
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    // CSS --------------------------------------------------------------------------------------------------------------------------------
    protected CSSClass ComputeClass() {
        var c = new CSSClass(CLASS_INPUT_WRAP);
        if (ViewModel.Error.HasError) c.Set(FormError.CLASS_ERROR);
        if (ViewModel.Disabled) c.Set(FormError.CLASS_DISABLED);
        c.Set(((Enum)Component.Align).String());
        return c;
    }
}

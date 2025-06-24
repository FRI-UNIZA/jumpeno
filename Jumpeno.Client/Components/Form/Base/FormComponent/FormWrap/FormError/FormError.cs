namespace Jumpeno.Client.Components;

public partial class FormError {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "form-error";

    public const string CLASS_CONTENT = "form-content";
    public const string CLASS_OUTLINE = "form-error-outline";
    public const string CLASS_MESSAGE = "form-error-message";

    public const string CLASS_ERROR = "error";

    public const string CLASS_DISABLED = "disabled";
    public const string CLASS_DISABLED_TRANSITION = "form-error-disabled-transition";
    public const string CLASS_DISABLED_CURSOR = "form-disabled-cursor";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required ERROR_TYPE Type { get; set; }
    [Parameter]
    public required FormErrorViewModel ViewModel { get; set; }
    [Parameter]
    public bool Disabled { get; set; } = false;
    [Parameter]
    public required RenderFragment? DisabledCursor { get; set; } = null;
    [Parameter]
    public required RenderFragment ChildContent { get; set; }
    
    // CSS --------------------------------------------------------------------------------------------------------------------------------
    private CSSClass ComputeClass() {
        var c = ComputeClass(CLASS);
        if (ViewModel.HasError) c.Set(CLASS_ERROR);
        if (Disabled) c.Set(CLASS_DISABLED);
        if (Disabled || WasDisabled) c.Set(CLASS_DISABLED_TRANSITION);
        c.Set(Type.String());
        return c;
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    private bool WasDisabled = false;
    protected override async Task OnComponentAfterRenderAsync(bool firstTime) {
        await Task.Yield();
        if (WasDisabled != Disabled) { WasDisabled = Disabled; StateHasChanged(); }
    }
    protected override void OnComponentDispose() => FormManager.Remove(ViewModel.FormViewModel.FormID);
    
    // Events -----------------------------------------------------------------------------------------------------------------------------
    private void Activate() {
        if (Type == ERROR_TYPE.PASSIVE) return;
        ActionHandler.SetFocus(ViewModel.FormViewModel.FormID);
        ActionHandler.Click($"#{ViewModel.FormViewModel.FormID}");
    }

    private void OnClick(MouseEventArgs e) => Activate();

    private async Task OnKeyDown(KeyboardEventArgs e) {
        if (e.Key != KEYBOARD.ENTER) return;
        await Task.Yield();
        Activate();
    }
}

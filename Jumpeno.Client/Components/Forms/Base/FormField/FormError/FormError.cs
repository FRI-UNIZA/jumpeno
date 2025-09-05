namespace Jumpeno.Client.Components;

public partial class FormError {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "form-error";
    public const string CLASS_OUTLINE = "form-error-outline";
    public const string CLASS_MESSAGE = "form-error-message";
    // Error class:
    public const string CLASS_ERROR = "error";
    // Display classes:
    public const string CLASS_NO_ERROR = "no-error";
    public const string CLASS_NO_MESSAGE = "no-message";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    // Type:
    [Parameter]
    public required FORM_ERROR_TYPE? Type { get; set; }
    // References:
    [Parameter]
    public required FormErrorViewModel ViewModel { get; set; }
    // Style:
    [Parameter]
    public FORM_ALIGN? Align { get; set; }
    [Parameter]
    public FORM_ALIGN? ErrorAlign { get; set; }
    // Display:
    [Parameter]
    public bool? NoError { get; set; } = false;
    [Parameter]
    public bool? NoMessage { get; set; } = false;
    
    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() {
        var c = base.ComputeClass()
        .Set(CLASS, Base)
        .Set(Type);
        if (ErrorAlign != null) c.Set(ErrorAlign);
        else c.Set(Align);
        c.Set(CLASS_NO_ERROR, NoError);
        c.Set(CLASS_NO_MESSAGE, NoMessage);
        return c;
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentDispose() => FormManager.Remove(ViewModel.FormViewModel.FormID);
    
    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private void Activate() {
        if (Type == FORM_ERROR_TYPE.PASSIVE) return;
        ActionHandler.SetFocus(ViewModel.FormViewModel.FormID);
        ActionHandler.Click($"#{ViewModel.FormViewModel.FormID}");
    }

    // Events -----------------------------------------------------------------------------------------------------------------------------
    private void OnClick(MouseEventArgs e) => Activate();

    private async Task OnKeyDown(KeyboardEventArgs e) {
        if (e.Key != KEYBOARD.ENTER) return;
        await Task.Yield();
        Activate();
    }
}

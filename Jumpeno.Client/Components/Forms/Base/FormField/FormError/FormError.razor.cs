namespace Jumpeno.Client.Components;

public partial class FormError {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "form-error";
    public const string ClassOutline = "form-error-outline";
    public const string ClassMessage = "form-error-message";
    // Error class:
    public const string ClassError = "error";
    // Display classes:
    public const string ClassNoError = "no-error";
    public const string ClassNoMessage = "no-message";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    // Type:
    [Parameter]
    public required FormErrorType? Type { get; set; }
    // References:
    [Parameter]
    public required FormErrorViewModel ViewModel { get; set; }
    // Style:
    [Parameter]
    public FormAlign? Align { get; set; }
    [Parameter]
    public FormAlign? ErrorAlign { get; set; }
    // Display:
    [Parameter]
    public bool? NoError { get; set; } = false;
    [Parameter]
    public bool? NoMessage { get; set; } = false;
    
    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() {
        var c = base.ComputeClass()
        .Set(ClassName, Base)
        .Set(Type);
        if (ErrorAlign != null) c.Set(ErrorAlign);
        else c.Set(Align);
        c.Set(ClassNoError, NoError);
        c.Set(ClassNoMessage, NoMessage);
        return c;
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentDispose() => ViewModel.Detach();
    
    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private void Activate() {
        if (Type == FormErrorType.Passive) return;
        ActionHandler.SetFocus(ViewModel.FormViewModel.FormID);
        ActionHandler.Click($"#{ViewModel.FormViewModel.FormID}");
    }

    // Events -----------------------------------------------------------------------------------------------------------------------------
    private void OnClick(MouseEventArgs e) => Activate();

    private async Task OnKeyDown(KeyboardEventArgs e) {
        if (e.Key != KeyBoard.Enter) return;
        await Task.Yield();
        Activate();
    }
}

namespace Jumpeno.Client.Utils;

public class Disabler(IDisabledComponent view, string? @class = null, string? @classTransition = null) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "disabled";
    public const string CLASS_TRANSITION = "disabled-transition";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly IDisabledComponent View = view;
    // State:
    private bool WasDisabled = false;
    // Classes:
    public string Class { get; private set; } = @class ?? CLASS;
    public string ClassTransition { get; private set; } = @classTransition ?? CLASS_TRANSITION;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public string CSSClass { get {
        return new CSSClass()
        .Set(Class, View.Disabled)
        .Set(ClassTransition, View.Disabled || WasDisabled);
    }}

    // Events -----------------------------------------------------------------------------------------------------------------------------
    public async Task OnViewRender() {
        if (WasDisabled == View.Disabled) return;
        await Task.Yield();
        WasDisabled = View.Disabled;
        View.Notify();
    }
}

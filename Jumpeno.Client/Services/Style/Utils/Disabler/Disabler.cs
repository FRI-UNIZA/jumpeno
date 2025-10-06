namespace Jumpeno.Client.Utils;

public class Disabler(IDisabledComponent view, string? @class = null, string? classDisabledAnimation = null) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "disabled";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly IDisabledComponent View = view;
    // State:
    private bool WasDisabled = false;
    // Classes:
    public string Class { get; private set; } = @class ?? CLASS;
    public string ClassDisabledAnimation { get; private set; } = classDisabledAnimation ?? AnimationHandler.CLASS_DISABLED_ANIMATION;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public string CSSClass { get {
        return new CSSClass()
        .Set(Class, View.Disabled)
        .Set(ClassDisabledAnimation, View.Disabled || WasDisabled);
    }}

    // Events -----------------------------------------------------------------------------------------------------------------------------
    public async Task OnViewRender() {
        if (WasDisabled == View.Disabled) return;
        await Task.Yield();
        WasDisabled = View.Disabled;
        View.Notify();
    }
}

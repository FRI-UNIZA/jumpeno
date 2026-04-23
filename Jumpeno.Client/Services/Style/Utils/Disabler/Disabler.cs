namespace Jumpeno.Client.Utils;

public class Disabler(IDisabledComponent view, string? @class = null, string? classDisabledAnimation = null) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "disabled";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly IDisabledComponent View = view;
    // State:
    private bool WasDisabled = false;
    // Classes:
    public string Class { get; private set; } = @class ?? ClassName;
    public string ClassDisabledAnimation { get; private set; } = classDisabledAnimation ?? AnimationHandler.ClassDisabledAnimation;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public string CSSClass { get {
        return new CssClass()
        .Set(Class, View.Disabled)
        .Set(ClassDisabledAnimation, View.Disabled || WasDisabled);
    }}

    // Events -----------------------------------------------------------------------------------------------------------------------------
    public async Task OnViewRender() {
        if (WasDisabled == View.Disabled) return;
        WasDisabled = View.Disabled;
        await Task.Yield();
        View.Notify();
    }
}

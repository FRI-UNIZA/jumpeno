namespace Jumpeno.Client.Components;

public partial class MenuControls {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "menu-controls";
    public const string CLASS_MOBILE = "mobile";
    public const string FIRST_LINK_ID = "menu-first-link";
    public const string FIRST_LINK_ID_MOBILE = "menu-first-link-mobile";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public bool Mobile { get; set; } = false;
    [Parameter]
    public Func<Task> Close { get; set; } = () => Task.CompletedTask;
    [Parameter]
    public Action OnFocusIn { get; set; } = () => {};
    [Parameter]
    public Action OnFocusOut { get; set; } = () => {};

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private CSSClass ComputeClass() {
        var c = new CSSClass(CLASS);
        if (Mobile) c.Set(CLASS_MOBILE);
        return c;
    }
}

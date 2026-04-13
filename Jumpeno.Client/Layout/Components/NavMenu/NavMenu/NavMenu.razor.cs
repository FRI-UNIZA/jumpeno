namespace Jumpeno.Client.Layouts;

public partial class NavMenu {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "nav-menu";
    public const string HIDDEN_CLASS = "hidden";
    public const string DISPLAY_CLASS = "display";
    public const string CLASS_CONTAINER = "nav-menu-container";
    public const string CLASS_NAVIGATION = "navigation";
    public const string MOBILE_MENU_BUTTON_ID = "mobile-menu-button";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public NAV_MENU_SURFACE Surface { get; set; } = NAV_MENU_SURFACE.SECONDARY;
    [Parameter]
    public required NavMenuMobile MobileRef { get; set; }
    [Parameter]
    public bool Display { get; set; } = true;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private bool Hidden = false;

    public bool MobileMenuButtonFocused { get; private set; }
    private void OnMobileMenuButtonFocusIn() => MobileMenuButtonFocused = true;
    private void OnMobileMenuButtonFocusOut() => MobileMenuButtonFocused = false;

    public bool ControlsFocused { get; private set; }
    private void OnControlsFocusIn() => ControlsFocused = true;
    private void OnControlsFocusOut() => ControlsFocused = false;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(CLASS, Base)
        .SetSurface(Surface)
        .Set(DISPLAY_CLASS, Display)
        .Set(HIDDEN_CLASS, Hidden);
    }
    
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentInitialized() => ScrollArea.AddScrollListener(SCROLLAREA_ID.PAGE, OnScroll);

    protected override void OnComponentDispose() => ScrollArea.RemoveScrollListener(SCROLLAREA_ID.PAGE, OnScroll);

    // Events -----------------------------------------------------------------------------------------------------------------------------
    private double TopPosition = 0;
    private void OnScroll(ScrollAreaPosition position) {
        if (position.ScrollTop > TopPosition) {
            if (!Hidden && position.ScrollTop > 75) ToggleHidden();
        } else if (position.ScrollTop < TopPosition) {
            if (Hidden) ToggleHidden();
        }
        TopPosition = position.ScrollTop;
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private void ToggleHidden() {
        Hidden = !Hidden;
        StateHasChanged();
    }
}

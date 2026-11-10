namespace Jumpeno.Client.Layouts;

public partial class NavMenu {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "nav-menu";
    public const string HiddenClass = "hidden";
    public const string DisplayClass = "display";
    public const string ClassContainer = "nav-menu-container";
    public const string ClassNavigation = "navigation";
    public const string MobileMenuButtonId = "mobile-menu-button";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public NavMenuSurface Surface { get; set; } = NavMenuSurface.Secondary;
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
    public override CssClass ComputeClass() {
        return base.ComputeClass()
        .Set(ClassName, Base)
        .SetSurface(Surface)
        .Set(DisplayClass, Display)
        .Set(HiddenClass, Hidden);
    }
    
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentInitialized() => ScrollArea.AddScrollListener(ScrollAreaId.Page, OnScroll);

    protected override void OnComponentDispose() => ScrollArea.RemoveScrollListener(ScrollAreaId.Page, OnScroll);

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

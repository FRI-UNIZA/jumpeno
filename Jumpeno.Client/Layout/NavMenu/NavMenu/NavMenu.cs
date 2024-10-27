namespace Jumpeno.Client.Layout;

public partial class NavMenu: IDisposable {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "nav-menu";
    public const string HIDDEN_CLASS = "hidden";
    public const string DISPLAY_CLASS = "display";
    public const string MOBILE_MENU_BUTTON_ID = "mobile-menu-button";
    public const string FIRST_LINK_ID = "menu-first-link";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required NavMenuMobile MobileRef { get; set; }
    [Parameter]
    public bool Display { get; set; } = true;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private bool Hidden = false;
    public bool MobileMenuButtonFocused { get; private set; }
    public bool ControlsFocused { get; private set; }
    private string ComputeClass() {
        var c = new CSSClass(CLASS);
        if (Display) c.Set(DISPLAY_CLASS);
        if (Hidden) c.Set(HIDDEN_CLASS);
        return c;
    }
    
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnInitialized() {
        ScrollArea.AddScrollListener(SCROLLAREA_ID.PAGE, OnScroll);
    }

    public void Dispose() {
        ScrollArea.RemoveScrollListener(SCROLLAREA_ID.PAGE, OnScroll);
    }

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
    private void OnControlsFocusIn() {
        ControlsFocused = true;
    }
    private void OnControlsFocusOut() {
        ControlsFocused = false;
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private void ToggleHidden() {
        Hidden = !Hidden;
        StateHasChanged();
    }
}

namespace Jumpeno.Client.Components;

public partial class NavMenuCloseButton {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "nav-menu-close-button";
    public const string CLASS_LINE = "nav-menu-close-button-line";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public EventCallback OnClick { get; set; }
}

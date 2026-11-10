namespace Jumpeno.Client.Components;

public partial class NavMenuCloseButton {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "nav-menu-close-button";
    public const string ClassLine = "nav-menu-close-button-line";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public EventCallback OnClick { get; set; }
}

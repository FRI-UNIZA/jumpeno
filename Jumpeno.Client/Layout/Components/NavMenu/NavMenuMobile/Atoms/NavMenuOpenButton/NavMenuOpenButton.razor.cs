namespace Jumpeno.Client.Components;

public partial class NavMenuOpenButton {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "nav-menu-open-button";
    public const string CLASS_LINE = "nav-menu-open-button-line";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required string ID { get; set; }
    [Parameter]
    public required EventCallback OnClick { get; set; }
    [Parameter]
    public Action OnFocus { get; set; } = () => {};
    [Parameter]
    public Action OnBlur { get; set; } = () => {};
}

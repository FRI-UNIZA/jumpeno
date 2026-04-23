namespace Jumpeno.Client.Components;

public partial class MenuControls {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "menu-controls";
    public const string ClassMobile = "mobile";
    public const string FirstLinkId = "menu-first-link";
    public const string FirstLinkIdMobile = "menu-first-link-mobile";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public bool Mobile { get; set; } = false;
    [Parameter]
    public Func<Task> Close { get; set; } = () => Task.CompletedTask;
    [Parameter]
    public Action OnFocusIn { get; set; } = () => {};
    [Parameter]
    public Action OnFocusOut { get; set; } = () => {};

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() {
        return base.ComputeClass()
        .Set(ClassName, Base)
        .Set(ClassMobile, Mobile);
    }
}

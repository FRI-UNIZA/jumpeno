namespace Jumpeno.Client.Components;

public partial class MenuButton {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string ClassName = "menu-button";

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);
}

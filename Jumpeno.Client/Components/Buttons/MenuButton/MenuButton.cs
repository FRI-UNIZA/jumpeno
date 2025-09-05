namespace Jumpeno.Client.Components;

public partial class MenuButton {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string CLASS = "menu-button";

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);
}

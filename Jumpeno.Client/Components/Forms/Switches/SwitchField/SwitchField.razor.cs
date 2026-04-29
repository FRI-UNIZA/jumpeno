namespace Jumpeno.Client.Components;

public partial class SwitchField {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string ClassName = "switch-field";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);
}

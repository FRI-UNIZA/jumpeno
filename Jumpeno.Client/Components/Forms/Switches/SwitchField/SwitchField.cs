namespace Jumpeno.Client.Components;

public partial class SwitchField {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string CLASS = "switch-field";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);
}

namespace Jumpeno.Client.Components;

public partial class CheckBoxField {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string CLASS = "checkbox-field";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);
}

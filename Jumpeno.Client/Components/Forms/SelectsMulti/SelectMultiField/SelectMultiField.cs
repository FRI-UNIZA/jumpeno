namespace Jumpeno.Client.Components;

public partial class SelectMultiField {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string CLASS = "select-multi-field";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);
}

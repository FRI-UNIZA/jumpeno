namespace Jumpeno.Client.Components;

public partial class SelectField<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string CLASS = "select-field";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);
}

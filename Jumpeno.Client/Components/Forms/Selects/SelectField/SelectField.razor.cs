namespace Jumpeno.Client.Components;

public partial class SelectField<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string ClassName = "select-field";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);
}

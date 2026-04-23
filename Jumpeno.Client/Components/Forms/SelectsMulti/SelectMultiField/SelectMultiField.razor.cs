namespace Jumpeno.Client.Components;

public partial class SelectMultiField<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string ClassName = "select-multi-field";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);
}

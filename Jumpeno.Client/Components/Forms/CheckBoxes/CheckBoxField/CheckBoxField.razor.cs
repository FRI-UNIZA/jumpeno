namespace Jumpeno.Client.Components;

public partial class CheckBoxField {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string ClassName = "checkbox-field";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);
}

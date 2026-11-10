namespace Jumpeno.Client.Components;

public partial class RadioField<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string ClassName = "radio-field";

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);
}

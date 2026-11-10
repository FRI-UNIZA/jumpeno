namespace Jumpeno.Client.Components;

public partial class InputField<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string ClassName = "input-field";

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);
}

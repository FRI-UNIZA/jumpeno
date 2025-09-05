namespace Jumpeno.Client.Components;

public partial class InputField<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string CLASS = "input-field";

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);
}

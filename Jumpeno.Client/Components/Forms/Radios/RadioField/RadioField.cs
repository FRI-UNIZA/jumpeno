namespace Jumpeno.Client.Components;

public partial class RadioField<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string CLASS = "radio-field";

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);
}

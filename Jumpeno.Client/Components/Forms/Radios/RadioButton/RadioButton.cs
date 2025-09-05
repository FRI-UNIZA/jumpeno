namespace Jumpeno.Client.Components;

public partial class RadioButton<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string CLASS = "radio-button";

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);
}

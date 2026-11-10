namespace Jumpeno.Client.Components;

public partial class RadioButton<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string ClassName = "radio-button";

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);
}

namespace Jumpeno.Client.Components;

public partial class TextAreaField {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string CLASS = "textarea-field";

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);
}

namespace Jumpeno.Client.Components;

public partial class TextAreaField {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string Class = "textarea-field";

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(Class, Base);
}

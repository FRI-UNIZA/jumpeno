namespace Jumpeno.Client.Components;

public partial class Paragraph {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string CLASS = "paragraph";

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);
}

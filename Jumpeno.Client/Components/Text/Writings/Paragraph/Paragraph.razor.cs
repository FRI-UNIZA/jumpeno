namespace Jumpeno.Client.Components;

public partial class Paragraph {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string ClassName = "paragraph";

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);
}

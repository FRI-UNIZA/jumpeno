namespace Jumpeno.Client.Components;

public partial class TextSpan {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string ClassName = "text-span";

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);
}

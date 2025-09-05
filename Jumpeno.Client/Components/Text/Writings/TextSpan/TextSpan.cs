namespace Jumpeno.Client.Components;

public partial class TextSpan {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string CLASS = "text-span";

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);
}

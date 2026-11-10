namespace Jumpeno.Client.Components;

public partial class ThumbButton {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "thumb-button";

    // Markup -------------------------------------------------------------------- --------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);
}

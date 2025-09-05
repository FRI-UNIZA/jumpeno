namespace Jumpeno.Client.Components;

public partial class ThumbButton {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "thumb-button";

    // Markup -------------------------------------------------------------------- --------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);
}

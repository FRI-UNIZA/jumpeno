namespace Jumpeno.Client.Components;

public abstract partial class WritingComponent {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string CLASS = "writing";

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);
}

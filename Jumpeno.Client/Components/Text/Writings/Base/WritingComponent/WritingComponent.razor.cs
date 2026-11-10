namespace Jumpeno.Client.Components;

public abstract partial class WritingComponent {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string ClassName = "writing";

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);
}

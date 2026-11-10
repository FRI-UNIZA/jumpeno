namespace Jumpeno.Client.Components;

public abstract partial class WritingSpacingComponent {
    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(WritingComponent.ClassName, Base);
}

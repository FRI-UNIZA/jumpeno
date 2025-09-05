namespace Jumpeno.Client.Components;

public abstract partial class WritingSpacingComponent {
    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(WritingComponent.CLASS, Base);
}

namespace Jumpeno.Client.Components;

public partial class HeadingComponent {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string ClassName = "heading";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required HeadingType Type { get; set; }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);
}

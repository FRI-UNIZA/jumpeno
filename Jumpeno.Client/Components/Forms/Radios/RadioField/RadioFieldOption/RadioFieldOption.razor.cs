namespace Jumpeno.Client.Components;

public partial class RadioFieldOption<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string ClassName = "radio-field-option";
    public const string ClassElement = "radio-field-option-element";
    public const string ClassDescription = "radio-field-option-description";
    public const string ClassActiveDescription = "active-description";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public RenderFragment? Description { get; set; } = null;
    [Parameter]
    public bool ActiveDescription { get; set; } = false;
    [Parameter]
    public RadioPosition? Position { get; set; } = RadioPosition.Start;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() {
        return base.ComputeClass()
        .Set(ClassName, Base)
        .Set(ClassActiveDescription, ActiveDescription)
        .Set(Position);
    }
}

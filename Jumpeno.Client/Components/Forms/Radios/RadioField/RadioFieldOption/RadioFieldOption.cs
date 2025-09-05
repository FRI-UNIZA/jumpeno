namespace Jumpeno.Client.Components;

public partial class RadioFieldOption<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string CLASS = "radio-field-option";
    public const string CLASS_ELEMENT = "radio-field-option-element";
    public const string CLASS_DESCRIPTION = "radio-field-option-description";
    public const string CLASS_ACTIVE_DESCRIPTION = "active-description";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public RenderFragment? Description { get; set; } = null;
    [Parameter]
    public bool ActiveDescription { get; set; } = false;
    [Parameter]
    public RADIO_POSITION? Position { get; set; } = RADIO_POSITION.START;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(CLASS, Base)
        .Set(CLASS_ACTIVE_DESCRIPTION, ActiveDescription)
        .Set(Position);
    }
}

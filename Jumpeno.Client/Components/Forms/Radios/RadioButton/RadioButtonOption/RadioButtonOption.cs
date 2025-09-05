namespace Jumpeno.Client.Components;

public partial class RadioButtonOption<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string CLASS = "radio-button-option";
    public const string CLASS_ELEMENT = "radio-button-element";
    public new const string CLASS_CONTENT = "radio-button-content";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);
}

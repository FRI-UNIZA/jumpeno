namespace Jumpeno.Client.Components;

public partial class RadioButtonOption<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string ClassName = "radio-button-option";
    public const string ClassElement = "radio-button-element";
    public new const string ClassContent = "radio-button-content";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);
}

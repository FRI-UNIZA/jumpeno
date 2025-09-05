namespace Jumpeno.Client.Components;

public partial class RadioComponent<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string CLASS = "radio";
    public const string CLASS_GROUP = "radio-group";
    // Cascade:
    public const string CASCADE_REF = $"{nameof(RadioComponent<T>)}.{nameof(CASCADE_REF)}";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public RenderFragment? ChildContent { get; set; } = null;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);
}

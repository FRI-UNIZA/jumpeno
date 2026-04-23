namespace Jumpeno.Client.Components;

public partial class RadioComponent<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string ClassName = "radio";
    public const string ClassGroup = "radio-group";
    // Cascade:
    public const string CascadeRef = $"{nameof(RadioComponent<T>)}.{nameof(CascadeRef)}";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public RenderFragment? ChildContent { get; set; } = null;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);
}

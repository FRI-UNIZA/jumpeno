namespace Jumpeno.Client.Components;

public partial class FormLabel {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "form-label";
    public const string ClassElement = "form-label-element";
    public const string ClassText = "form-label-text";
    public const string ClassContent = "form-label-content";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    // Label:
    [Parameter]
    public required OneOf<string, List<string>> Label { get; set; }
    // Style:
    [Parameter]
    public FormVariant? Variant { get; set; } = FormVariant.Primary;
    [Parameter]
    public FormSize? Size { get; set; } = FormSize.M;
    [Parameter]
    public FormAlign? Align { get; set; } = null;
    // Content:
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base).SetVariant(Variant).SetSize(Size).Set(Align);
}

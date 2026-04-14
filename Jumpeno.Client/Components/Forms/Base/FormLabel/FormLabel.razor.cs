namespace Jumpeno.Client.Components;

public partial class FormLabel {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "form-label";
    public const string CLASS_ELEMENT = "form-label-element";
    public const string CLASS_TEXT = "form-label-text";
    public const string CLASS_CONTENT = "form-label-content";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    // Label:
    [Parameter]
    public required OneOf<string, List<string>> Label { get; set; }
    // Style:
    [Parameter]
    public FORM_VARIANT? Variant { get; set; } = FORM_VARIANT.PRIMARY;
    [Parameter]
    public FORM_SIZE? Size { get; set; } = FORM_SIZE.M;
    [Parameter]
    public FORM_ALIGN? Align { get; set; } = null;
    // Content:
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base).SetVariant(Variant).SetSize(Size).Set(Align);
}

namespace Jumpeno.Client.Components;

public abstract partial class TextComponent {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "text";
    public const string CLASS_NO_WRAP = "no-wrap";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public TextVariant? Variant { get; set; } = TextVariant.PRIMARY;
    [Parameter]
    public TextSize? Size { get; set; } = TextSize.M;
    [Parameter]
    public TextAlignTypes? Align { get; set; } = null;
    [Parameter]
    public bool NoWrap { get; set; }
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(CLASS, Base)
        .SetVariant(Variant)
        .SetSize(Size)
        .Set(Align)
        .Set(CLASS_NO_WRAP, NoWrap);
    }
}

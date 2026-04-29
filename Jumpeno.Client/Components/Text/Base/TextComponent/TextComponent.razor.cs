namespace Jumpeno.Client.Components;

public abstract partial class TextComponent {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "text";
    public const string ClassNoWrap = "no-wrap";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public TextVariant? Variant { get; set; } = TextVariant.Primary;
    [Parameter]
    public TextSize? Size { get; set; } = TextSize.M;
    [Parameter]
    public TextAlignTypes? Align { get; set; } = null;
    [Parameter]
    public bool NoWrap { get; set; }
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() {
        return base.ComputeClass()
        .Set(ClassName, Base)
        .SetVariant(Variant)
        .SetSize(Size)
        .Set(Align)
        .Set(ClassNoWrap, NoWrap);
    }
}

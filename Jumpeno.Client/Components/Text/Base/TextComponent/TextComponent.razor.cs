namespace Jumpeno.Client.Components;

public abstract partial class TextComponent {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "text";
    public const string CLASS_NO_WRAP = "no-wrap";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public TEXT_VARIANT? Variant { get; set; } = TEXT_VARIANT.PRIMARY;
    [Parameter]
    public TEXT_SIZE? Size { get; set; } = TEXT_SIZE.M;
    [Parameter]
    public TEXT_ALIGN? Align { get; set; } = null;
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

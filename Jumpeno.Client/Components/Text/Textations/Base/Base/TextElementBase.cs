namespace Jumpeno.Client.Components;

public partial class TextElementBase: SurfaceComponent {
    public const string CLASSNAME = "text";
    public const string CLASSNAME_NOWRAP = "no-wrap";
    public const string CLASSNAME_SPACING = "spacing";

    [Parameter]
    public required TEXT_VARIANT Variant { get; set; }
    [Parameter]
    public required TEXT_SIZE Size { get; set; }
    [Parameter]
    public bool NoWrap { get; set; } = false;
    [Parameter]
    public required bool Spacing { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected CSSClass ComputeClass() {
        var c = ComputeClass(CLASSNAME);
        c.Set(Class);
        c.Set(Variant.StringLower());
        c.Set(Size.StringLower());
        if (NoWrap) c.Set(CLASSNAME_NOWRAP);
        if (Spacing) c.Set(CLASSNAME_SPACING);
        return c;
    }
}

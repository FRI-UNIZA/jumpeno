namespace Jumpeno.Client.Layouts;

public partial class Container {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "container";
    public const string CLASS_BOUNDARY = "boundary";
    public const string CLASS_PADDING_VERTICAL = "padding-vertical";
    public const string CLASS_PADDING_HORIZONTAL = "padding-horizontal";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public string Tag { get; set; } = "div";
    [Parameter]
    public string? ID { get; set; }
    [Parameter]
    public CONTAINER_TYPE Type { get; set; } = CONTAINER_TYPE.NONE;
    [Parameter]
    public bool Boundary { get; set; } = false;
    [Parameter]
    public bool PaddingVertical { get; set; } = false;
    [Parameter]
    public bool PaddingHorizontal { get; set; } = false;
    [Parameter]
    public bool Inert { get; set; } = false;
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? Attributes { get; set; }
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(CLASS, Base)
        .Set(Type)
        .Set(CLASS_BOUNDARY, Boundary)
        .Set(CLASS_PADDING_VERTICAL, PaddingVertical)
        .Set(CLASS_PADDING_HORIZONTAL, PaddingHorizontal);
    }

    // Render -----------------------------------------------------------------------------------------------------------------------------
    private RenderFragment Render() => builder => {
        var sequence = 0;
        builder.OpenElement(sequence++, Tag);
        builder.AddAttribute(sequence++, SCOPE.GLOBAL);
        builder.AddAttribute(sequence++, "class", ComputeClass());
        builder.AddAttribute(sequence++, "style", Style);
        if (ID is not null) builder.AddAttribute(sequence++, "id", ID);
        if (Attributes is not null) {
            foreach (var attribute in Attributes) {
                builder.AddAttribute(sequence++, attribute.Key, attribute.Value);
            }
        }
        if (Inert) builder.AddAttribute(sequence++, "inert", true);
        builder.AddContent(sequence++, ChildContent);
        builder.CloseElement();
    };
}

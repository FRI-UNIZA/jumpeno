namespace Jumpeno.Client.Layouts;

public partial class Container {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASSNAME = "container";
    public const string CLASSNAME_TYPE_INLINE = "container-inline";
    public const string CLASSNAME_TYPE_SIZE = "container-size";
    public const string CLASSNAME_BOUNDARY = "boundary";
    public const string CLASSNAME_PADDING_VERTICAL = "padding-vertical";
    public const string CLASSNAME_PADDING_HORIZONTAL = "padding-horizontal";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [CascadingParameter]
    public SURFACE? _ParentContainerSurface { get; set; }

    [Parameter]
    public string Tag { get; set; } = "div";
    [Parameter]
    public string? ID { get; set; }
    [Parameter]
    public CONTAINER_TYPE Type { get; set; } = CONTAINER_TYPE.NONE;
    [Parameter]
    public SURFACE? Surface { get; set; }
    [Parameter]
    public bool Boundary { get; set; } = false;
    [Parameter]
    public bool PaddingVertical { get; set; } = false;
    [Parameter]
    public bool PaddingHorizontal { get; set; } = false;
    [Parameter]
    public bool Inert { get; set; } = false;
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private CSSClass ComputeClass() {
        var c = new CSSClass(CLASSNAME);
        switch (Type) {
            case CONTAINER_TYPE.INLINE:
                c.Set(CLASSNAME_TYPE_INLINE);
            break;
            case CONTAINER_TYPE.SIZE:
                c.Set(CLASSNAME_TYPE_SIZE);
            break;
        }
        if (Boundary) c.Set(CLASSNAME_BOUNDARY);
        if (PaddingVertical) c.Set(CLASSNAME_PADDING_VERTICAL);
        if (PaddingHorizontal) c.Set(CLASSNAME_PADDING_HORIZONTAL);
        c.Set(Class);
        return c;
    }

    private Dictionary<string, object> ComputeParameters() {
        return new Dictionary<string, object> {
            { "ChildContent", BuildRenderTree(this) }
        };
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentParametersSet(bool firstTime) {
        if (Surface is not null) return;
        if (_ParentContainerSurface is not null) Surface = _ParentContainerSurface;
        else Surface = SURFACE.PRIMARY;
    }

    // Render -----------------------------------------------------------------------------------------------------------------------------
    private static RenderFragment BuildRenderTree(Container container) => (RenderTreeBuilder builder) => {
        var sequence = 0;
        builder.OpenElement(sequence++, container.Tag);
        builder.AddAttribute(sequence++, "class", container.ComputeClass());
        if (container.ID is not null) builder.AddAttribute(sequence++, "id", container.ID);
        if (container.AdditionalAttributes is not null) {
            foreach (var attribute in container.AdditionalAttributes) {
                builder.AddAttribute(sequence++, attribute.Key, attribute.Value);
            }
        }
        if (container.Inert) builder.AddAttribute(sequence++, "inert", true);
        builder.AddContent(sequence++, container.ChildContent);
        builder.CloseElement();
    };
}

namespace Jumpeno.Client.Layouts;

public partial class Container {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "container";
    public const string ClassBoundary = "boundary";
    public const string ClassPaddingVertical = "padding-vertical";
    public const string ClassPaddingHorizontal = "padding-horizontal";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public string Tag { get; set; } = "div";
    [Parameter]
    public string? Id { get; set; }
    [Parameter]
    public ContainerType Type { get; set; } = ContainerType.None;
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
    public override CssClass ComputeClass() {
        return base.ComputeClass()
        .Set(ClassName, Base)
        .Set(Type)
        .Set(ClassBoundary, Boundary)
        .Set(ClassPaddingVertical, PaddingVertical)
        .Set(ClassPaddingHorizontal, PaddingHorizontal);
    }

    // Render -----------------------------------------------------------------------------------------------------------------------------
    private RenderFragment Render() => builder => {
        var sequence = 0;
        builder.OpenElement(sequence++, Tag);
        builder.AddAttribute(sequence++, Scope.Global);
        builder.AddAttribute(sequence++, "class", ComputeClass());
        builder.AddAttribute(sequence++, "style", Style);
        if (Id is not null) builder.AddAttribute(sequence++, "id", Id);
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

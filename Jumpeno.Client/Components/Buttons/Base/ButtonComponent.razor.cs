namespace Jumpeno.Client.Components;

public partial class ButtonComponent : IDisabledComponent {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string IdPrefix = "button";
    // Classes:
    public const string ClassName = "button-component";
    public const string ClassContent = "button-content";
    public const string ClassIconBefore = "button-icon-before";
    public const string ClassText = "button-text";
    public const string ClassIconAfter = "button-icon-after";
    public const string ClassHasIconBefore = "has-icon-before";
    public const string ClassHasIconAfter = "has-icon-after";
    public const string ClassIconOnly = "icon-only";
    // Params:
    public static readonly ButtonParams DefaultParams = new();

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public string Id { get; set; } = "";
    [Parameter]
    public required OneOf<ButtonParams, ButtonLinkParams> Params { get; set; } = DefaultParams;
    [Parameter]
    public EventCallback<ButtonClickEvent> OnClick { get; set; } = EventCallback<ButtonClickEvent>.Empty;
    [Parameter]
    public RenderFragment? Icon { get; set; }
    [Parameter]
    public RenderFragment? Text { get; set; }
    [Parameter]
    public RenderFragment? IconAfter { get; set; }
    [Parameter]
    public bool Disabled { get; set; } = false;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private Dictionary<string, object> Attributes { get; set; } = [];

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    public bool IconOnly() => Text == null && ((Icon != null && IconAfter == null) || (Icon == null && IconAfter != null));

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base).Set(ClassIconOnly, IconOnly());

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override sealed void OnInitialized() => base.OnInitialized();
    protected override sealed async Task OnInitializedAsync() => await base.OnInitializedAsync();
    protected override sealed void OnParametersSet() {
        if (Id == "") Id = IDGenerator.Generate(IdPrefix);
        var label = Params.IsT0 ? Params.AsT0.Label : Params.AsT1.Label;
        if (label != "") Attributes["aria-label"] = label;
        base.OnParametersSet();
    }
    protected override sealed async Task OnParametersSetAsync() => await base.OnParametersSetAsync();
    protected override sealed bool ShouldRender() => base.ShouldRender();
    protected override sealed void OnAfterRender(bool firstRender) => base.OnAfterRender(firstRender);
    protected override sealed async Task OnAfterRenderAsync(bool firstRender) => await base.OnAfterRenderAsync(firstRender);
    public override sealed async ValueTask DisposeAsync() => await base.DisposeAsync();

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    private RenderFragment RenderIconBefore() => builder => {
        var sequence = 0;
        builder.OpenElement(sequence++, "span");
        builder.AddAttribute(sequence++, "class", ClassIconBefore);
        builder.AddContent(sequence++, Icon);
        builder.CloseElement();
    };

    private RenderFragment RenderText() => builder => {
        var sequence = 0;
        builder.OpenElement(sequence++, "span");
        var c = new CssClass(ClassText);
        if (Icon != null) c.Set(ClassHasIconBefore);
        if (IconAfter != null) c.Set(ClassHasIconAfter);
        builder.AddAttribute(sequence++, "class", c);
        builder.AddContent(sequence++, Text);
        builder.CloseElement();
    };

    private RenderFragment RenderIconAfter() => builder => {
        var sequence = 0;
        builder.OpenElement(sequence++, "span");
        builder.AddAttribute(sequence++, "class", ClassIconAfter);
        builder.AddContent(sequence++, IconAfter);
        builder.CloseElement();
    };

    private RenderFragment RenderChildContent() => builder => {
        var sequence = 0;
        builder.OpenElement(sequence++, "span");
        builder.AddAttribute(sequence++, "class", ClassContent);
        if (Icon is not null) builder.AddContent(sequence++, RenderIconBefore()); 
        if (Text is not null) builder.AddContent(sequence++, RenderText()); 
        if (IconAfter is not null) builder.AddContent(sequence++, RenderIconAfter()); 
        builder.CloseElement();
    };
}

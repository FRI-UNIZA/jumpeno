namespace Jumpeno.Client.Components;

public partial class ButtonComponent : IDisabledComponent {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID_PREFIX = "button";
    // Classes:
    public const string CLASS = "button-component";
    public const string CLASS_CONTENT = "button-content";
    public const string CLASS_ICON_BEFORE = "button-icon-before";
    public const string CLASS_TEXT = "button-text";
    public const string CLASS_ICON_AFTER = "button-icon-after";
    public const string CLASS_HAS_ICON_BEFORE = "has-icon-before";
    public const string CLASS_HAS_ICON_AFTER = "has-icon-after";
    // Params:
    public static readonly ButtonParams DEFAULT_PARAMS = new();

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public string ID { get; set; } = "";
    [Parameter]
    public required OneOf<ButtonParams, ButtonLinkParams> Params { get; set; } = DEFAULT_PARAMS;
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

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override sealed void OnInitialized() => base.OnInitialized();
    protected override sealed async Task OnInitializedAsync() => await base.OnInitializedAsync();
    protected override sealed void OnParametersSet() {
        if (ID == "") ID = IDGenerator.Generate(ID_PREFIX);
        var Label = Params.IsT0 ? Params.AsT0.Label : Params.AsT1.Label;
        if (Label != "") Attributes["aria-label"] = Label;
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
        builder.AddAttribute(sequence++, "class", CLASS_ICON_BEFORE);
        builder.AddContent(sequence++, Icon);
        builder.CloseElement();
    };

    private RenderFragment RenderText() => builder => {
        var sequence = 0;
        builder.OpenElement(sequence++, "span");
        var c = new CSSClass(CLASS_TEXT);
        if (Icon != null) c.Set(CLASS_HAS_ICON_BEFORE);
        if (IconAfter != null) c.Set(CLASS_HAS_ICON_AFTER);
        builder.AddAttribute(sequence++, "class", c);
        builder.AddContent(sequence++, Text);
        builder.CloseElement();
    };

    private RenderFragment RenderIconAfter() => builder => {
        var sequence = 0;
        builder.OpenElement(sequence++, "span");
        builder.AddAttribute(sequence++, "class", CLASS_ICON_AFTER);
        builder.AddContent(sequence++, IconAfter);
        builder.CloseElement();
    };

    private RenderFragment RenderChildContent() => builder => {
        var sequence = 0;
        builder.OpenElement(sequence++, "span");
        builder.AddAttribute(sequence++, "class", CLASS_CONTENT);
        if (Icon is not null) builder.AddContent(sequence++, RenderIconBefore()); 
        if (Text is not null) builder.AddContent(sequence++, RenderText()); 
        if (IconAfter is not null) builder.AddContent(sequence++, RenderIconAfter()); 
        builder.CloseElement();
    };
}

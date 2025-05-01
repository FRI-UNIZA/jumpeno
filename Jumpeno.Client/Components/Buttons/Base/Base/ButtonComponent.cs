namespace Jumpeno.Client.Components;

public class ButtonComponent : SurfaceComponent {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID_PREFIX = "button";
    public static readonly ButtonParams DEFAULT_PARAMS = new();

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public string ID { get; set; } = "";
    [Parameter]
    public required OneOf<ButtonParams, ButtonLinkParams> Params { get; set; } = DEFAULT_PARAMS;
    [Parameter]
    public EventCallback<ButtonComponent> OnClick { get; set; } = EventCallback<ButtonComponent>.Empty;
    [Parameter]
    public RenderFragment? Icon { get; set; }
    [Parameter]
    public RenderFragment? Text { get; set; }
    [Parameter]
    public RenderFragment? IconAfter { get; set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentParametersSet(bool firstTime) {
        if (ID == "") ID = IDGenerator.Generate(ID_PREFIX);
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    protected RenderFragment Render() => builder => {
        var sequence = 0;
        builder.OpenComponent<ButtonElement>(sequence++);
        builder.AddAttribute(sequence++, nameof(ID), ID);
        builder.AddAttribute(sequence++, nameof(Params), Params);
        builder.AddAttribute(sequence++, nameof(OnClick), OnClick);
        if (Icon is not null) builder.AddAttribute(sequence++, nameof(Icon), Icon); 
        if (Text is not null) builder.AddAttribute(sequence++, nameof(Text), Text); 
        if (IconAfter is not null) builder.AddAttribute(sequence++, nameof(IconAfter), IconAfter); 
        builder.CloseComponent();
    };
}

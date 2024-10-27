namespace Jumpeno.Client.Components;

public class ButtonComponent: SurfaceComponent {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID_PREFIX = "button";
    public static readonly ButtonParameters DEFAULT_PARAMETERS = new();

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public string ID { get; set; } = "";
    [Parameter]
    public required OneOf<ButtonParameters, ButtonLinkParameters> Parameters { get; set; } = DEFAULT_PARAMETERS;
    [Parameter]
    public EventCallback<ButtonComponent> OnClick { get; set; } = EventCallback<ButtonComponent>.Empty;
    [Parameter]
    public RenderFragment? Icon { get; set; }
    [Parameter]
    public RenderFragment? Text { get; set; }
    [Parameter]
    public RenderFragment? IconAfter { get; set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnParametersSet() {
        if (ID == "") ID = ComponentService.GenerateID(ID_PREFIX);
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    protected RenderFragment Render() => (RenderTreeBuilder builder) => {
        var sequence = 0;
        builder.OpenComponent<ButtonBase>(sequence++);
        builder.AddAttribute(sequence++, "ID", ID);
        builder.AddAttribute(sequence++, "Parameters", Parameters);
        builder.AddAttribute(sequence++, "OnClick", OnClick);
        if (Icon is not null) builder.AddAttribute(sequence++, "Icon", Icon); 
        if (Text is not null) builder.AddAttribute(sequence++, "Text", Text); 
        if (IconAfter is not null) builder.AddAttribute(sequence++, "IconAfter", IconAfter); 
        builder.CloseComponent();
    };
}

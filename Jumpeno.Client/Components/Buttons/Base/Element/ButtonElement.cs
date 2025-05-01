namespace Jumpeno.Client.Components;

public partial class ButtonElement {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASSNAME = "button-component";
    public const string CLASSNAME_ELEMENT = "button-element";
    public const string CLASSNAME_CONTENT = "button-content";
    public const string CLASSNAME_ICON_BEFORE = "button-icon-before";
    public const string CLASSNAME_TEXT = "button-text";
    public const string CLASSNAME_ICON_AFTER = "button-icon-after";
    public const string CLASSNAME_HAS_ICON_BEFORE = "has-icon-before";
    public const string CLASSNAME_HAS_ICON_AFTER = "has-icon-after";

    // Attributes -------------------------------------------------------------------------------------------------------------------------    
    private readonly Dictionary<string, object> AdditionalAttributes = [];
    protected CSSClass ComputeClass() => ComputeClass(CLASSNAME);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentParametersSet(bool firstTime) {
        var Label = Params.IsT0 ? Params.AsT0.Label : Params.AsT1.Label;
        if (Label != "") AdditionalAttributes["aria-label"] = Label;
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private RenderFragment RenderIconBefore() => builder => {
        var sequence = 0;
        builder.OpenElement(sequence++, "span");
        builder.AddAttribute(sequence++, "class", CLASSNAME_ICON_BEFORE);
        builder.AddContent(sequence++, Icon);
        builder.CloseElement();
    };

    private RenderFragment RenderText() => builder => {
        var sequence = 0;
        builder.OpenElement(sequence++, "span");
        var @class = new CSSClass(CLASSNAME_TEXT);
        if (Icon != null) @class.Set(CLASSNAME_HAS_ICON_BEFORE);
        if (IconAfter != null) @class.Set(CLASSNAME_HAS_ICON_AFTER);
        builder.AddAttribute(sequence++, "class", @class);
        builder.AddContent(sequence++, Text);
        builder.CloseElement();
    };

    private RenderFragment RenderIconAfter() => builder => {
        var sequence = 0;
        builder.OpenElement(sequence++, "span");
        builder.AddAttribute(sequence++, "class", CLASSNAME_ICON_AFTER);
        builder.AddContent(sequence++, IconAfter);
        builder.CloseElement();
    };

    private RenderFragment RenderChildContent() => builder => {
        var sequence = 0;
        builder.OpenElement(sequence++, "span");
        builder.AddAttribute(sequence++, "class", CLASSNAME_CONTENT);
        if (Icon is not null) builder.AddContent(sequence++, RenderIconBefore()); 
        if (Text is not null) builder.AddContent(sequence++, RenderText()); 
        if (IconAfter is not null) builder.AddContent(sequence++, RenderIconAfter()); 
        builder.CloseElement();
    };
}

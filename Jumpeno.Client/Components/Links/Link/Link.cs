namespace Jumpeno.Client.Components;

public partial class Link {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string UNDERLINE_CLASS = "underline";
    public const string DEFAULT_ACTIVE_CLASS = "active";
    public const string ID_PREFIX = "link";
    public const string ROLE = "link";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public string ID { get; set; } = "";
    [Parameter]
    public string? Href { get; set; } = null;
    [Parameter]
    public bool HrefPrevent { get; set; } = false;
    [Parameter]
    public string Label { get; set; } = "";
    [Parameter]
    public EventCallback<LinkClickEvent> OnClick { get; set; } = EventCallback<LinkClickEvent>.Empty;
    [Parameter]
    public EventCallback<LinkKeyEvent> OnKeyPress { get; set; } = EventCallback<LinkKeyEvent>.Empty;
    [Parameter]
    public EventCallback<LinkKeyEvent> OnEnter { get; set; } = EventCallback<LinkKeyEvent>.Empty;
    [Parameter]
    public OneOf<LINK_TARGET, string> Target { get; set; } = LINK_TARGET.SELF;
    [Parameter]
    public LINK_MATCH Match { get; set; } = LINK_MATCH.PREFIX;
    [Parameter]
    public bool Underline { get; set; } = false;
    [Parameter]
    public string ActiveClass { get; set; } = DEFAULT_ACTIVE_CLASS;
    [Parameter]
    public int TabIndex { get; set; } = 0;
    private readonly Dictionary<string, object> AdditionalAttributes = [];

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private bool IsActive() {
        if (Href == null) return false;
        if (URL.Schema(Href) != "") return false;
        return Match == LINK_MATCH.ALL ? IsExactMatch() : IsPrefixMatch();
    }

    private string? ComputeTarget() {
        if (Href == null) return null;
        return Target.IsT0
            ? $"_{Target.AsT0.ToString().ToLower()}"
            : Target.AsT1;
    }

    protected CSSClass ComputeLinkClass() {
        var c = ComputeClass();
        if (Underline) c.Set(UNDERLINE_CLASS);
        if (IsActive()) c.Set(ActiveClass);
        return c;
    }

    private bool IsExactMatch() {
        if (Href == null) return false;
        var uri = URL.Path().ToLower();
        var hrefUri = URL.Path(Href).ToLower();
        // Compare the path ignoring query parameters
        return uri.Split('?')[0] == hrefUri.Split('?')[0];
    }

    private bool IsPrefixMatch() {
        if (Href == null) return false;
        var uri = URL.Path().ToLower();
        var hrefUri = URL.Path(Href).ToLower();
        // Compare the prefix of the path ignoring query parameters
        return uri.Split('?')[0].StartsWith(hrefUri.Split('?')[0], StringComparison.OrdinalIgnoreCase);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentParametersSet(bool firstTime) {
        if (!firstTime) return;
        if (ID == "") ID = IDGenerator.Generate(ID_PREFIX);
        if (Label != "") AdditionalAttributes["aria-label"] = Label;
    }

    protected override async Task OnComponentInitializedAsync() => await Navigator.AddAfterEventListener(Notify);

    protected override async ValueTask OnComponentDisposeAsync() => await Navigator.RemoveAfterEventListener(Notify);

    private async Task OnClickEvent(MouseEventArgs e) => await OnClick.InvokeAsync(new(this, e));
    private async Task OnKeyPressEvent(KeyboardEventArgs e) {
        if (e.Key == KEYBOARD.ENTER) await OnEnter.InvokeAsync(new(this, e));
        await OnKeyPress.InvokeAsync(new(this, e));
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private void Notify(NavigationEvent e) => StateHasChanged();
}

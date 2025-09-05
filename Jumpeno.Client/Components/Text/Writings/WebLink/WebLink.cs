namespace Jumpeno.Client.Components;

public partial class WebLink : IDisabledComponent {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string CLASS = "web-link";
    public const string CLASS_UNDERLINE = "underline";
    public const string CLASS_DEFAULT_ACTIVE = "active";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public string ID { get; set; } = "";
    [Parameter]
    public WEBLINK_ROLE Role { get; set; } = WEBLINK_ROLE.LINK;
    [Parameter]
    public string? Href { get; set; } = null;
    [Parameter]
    public bool HrefPrevent { get; set; } = false;
    [Parameter]
    public string Label { get; set; } = "";
    [Parameter]
    public EventCallback<WebLinkClickEvent> OnClick { get; set; } = EventCallback<WebLinkClickEvent>.Empty;
    [Parameter]
    public EventCallback<WebLinkKeyEvent> OnKeyPress { get; set; } = EventCallback<WebLinkKeyEvent>.Empty;
    [Parameter]
    public EventCallback<WebLinkKeyEvent> OnEnter { get; set; } = EventCallback<WebLinkKeyEvent>.Empty;
    [Parameter]
    public OneOf<WEBLINK_TARGET, string> Target { get; set; } = WEBLINK_TARGET.SELF;
    [Parameter]
    public WEBLINK_MATCH Match { get; set; } = WEBLINK_MATCH.PREFIX;
    [Parameter]
    public bool Underline { get; set; } = false;
    [Parameter]
    public bool Disabled { get; set; } = false;
    [Parameter]
    public string ActiveClass { get; set; } = CLASS_DEFAULT_ACTIVE;
    [Parameter]
    public int TabIndex { get; set; } = 0;
    private readonly Dictionary<string, object> Attributes = [];

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    private bool IsActive() {
        if (Href == null) return false;
        if (URL.Schema(Href) != "") return false;
        return Match == WEBLINK_MATCH.ALL ? IsExactMatch() : IsPrefixMatch();
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
        var path = uri.Split('?')[0];
        var hrefPath = hrefUri.Split('?')[0];
        if (path == hrefPath) return true;
        return path.StartsWith(hrefPath + '/', StringComparison.OrdinalIgnoreCase);
    }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    private string? ComputeTarget() {
        if (Href == null) return null;
        return Target.IsT0 ? Target.AsT0.String() : Target.AsT1;
    }

    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(CLASS, Base)
        .Set(CLASS_UNDERLINE, Underline)
        .Set(ActiveClass, IsActive());
    }

    protected override async Task OnComponentInitializedAsync() => await Navigator.AddAfterEventListener(Notify);

    protected override void OnComponentParametersSet(bool firstTime) {
        if (!firstTime) return;
        if (ID == "") ID = IDGenerator.Generate(nameof(WebLink));
        if (Label != "") Attributes["aria-label"] = Label;
    }

    protected override async ValueTask OnComponentDisposeAsync() => await Navigator.RemoveAfterEventListener(Notify);

    private async Task OnClickEvent(MouseEventArgs e) {
        if (Disabled) return;
        await OnClick.InvokeAsync(new(this, e));
    }
    private async Task OnKeyPressEvent(KeyboardEventArgs e) {
        if (Disabled) return;
        if (e.Key == KEYBOARD.ENTER) await OnEnter.InvokeAsync(new(this, e));
        await OnKeyPress.InvokeAsync(new(this, e));
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private void Notify(NavigationEvent e) => StateHasChanged();
}

namespace Jumpeno.Client.Components;

public partial class Link : IAsyncDisposable {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string UNDERLINE_CLASS = "underline";
    public const string DEFAULT_ACTIVE_CLASS = "active";
    public const string ID_PREFIX = "link";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public string ID { get; set; } = "";
    [Parameter]
    public string? Href { get; set; } = null;
    [Parameter]
    public string Label { get; set; } = "";
    [Parameter]
    public EventCallback<Link> OnClick { get; set; } = EventCallback<Link>.Empty;
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
    public Link() {
        ID = ComponentService.GenerateID(ID_PREFIX);
    }

    protected override void OnParametersSet(bool firstTime) {
        if (ID == "") ID = ComponentService.GenerateID(ID_PREFIX);
        if (Label != "") AdditionalAttributes["aria-label"] = Label;
    }

    protected override async Task OnInitializedAsync() {
        await Navigator.AddAfterEventListener(ChangeState);
    }

    public virtual async ValueTask DisposeAsync() {
        await Navigator.RemoveAfterEventListener(ChangeState);
    }

    private async Task OnClickEvent(MouseEventArgs e) {
        await OnClick.InvokeAsync(this);
    }

    private async Task OnEnterEvent(KeyboardEventArgs e) {
        if (e.Key != KEYBOARD.ENTER) return;
        await OnClick.InvokeAsync(this);
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private void ChangeState(NavigationEvent e) {
        StateHasChanged();
    }
}

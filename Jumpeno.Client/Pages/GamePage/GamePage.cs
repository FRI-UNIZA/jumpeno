namespace Jumpeno.Client.Pages;

public partial class GamePage {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ROUTE_EN = "/en/game/{URLCode?}";
    public const string ROUTE_SK = "/sk/hra/{URLCode?}";
    public static string Link(string url, string code) => URL.ReplaceSegments(url, new() {{ 1, $"{code}" }});

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public string? URLCode { get; set; }

    [CascadingParameter]
    public required MainLayoutViewModel LayoutVM { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly ConnectViewModel ConnectVM;
    private GameViewModel? GameVM;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public GamePage() {
        ConnectVM = new(new(
            URLCode: () => URLCode,
            OnConnect: new(OnConnect),
            OnDisconnect: new(OnDisconnect),
            Notify: new(Notify)
        ));
        GameVM = null;
    }

    protected override async Task OnPageInitializedAsync() => await ConnectVM.OnPageInitializedAsync();
    protected override async Task OnPageParametersSetAsync(bool firstTime) => await ConnectVM.OnPageParametersSetAsync();
    protected override async ValueTask OnPageDisposeAsync() => await ConnectVM.OnPageDisposeAsync();

    // Events -----------------------------------------------------------------------------------------------------------------------------
    private void OnConnect(GameViewModel vm) {
        // 1) Control actions:
        Window.BlockUserSelect();
        Window.TouchActionPanOn();
        Window.OverscrollNoneOn();
        Window.PreventTouchStart();
        Window.PreventTouchEnd();
        // 2) Set model:
        GameVM = vm;
        GameVM.InitControls();
        // 3) Update layout:
        LayoutVM?.HideNavigation(false);
        ScrollArea.ScrollTo(SCROLLAREA_ID.PAGE, 0, 0);
    }

    private void OnDisconnect() {
        // 1) Control actions:
        Window.AllowUserSelect();
        Window.TouchActionPanOff();
        Window.OverscrollNoneOff();
        Window.DefaultTouchStart();
        Window.DefaultTouchEnd();
        // 2) Set model:
        GameVM = null;
        // 3) Update layout:
        LayoutVM?.ShowNavigation();
        ScrollArea.ScrollTo(SCROLLAREA_ID.PAGE, 0, 0);
    }

    private void Notify() => StateHasChanged();
}

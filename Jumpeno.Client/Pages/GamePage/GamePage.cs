namespace Jumpeno.Client.Pages;

public partial class GamePage {
    public const string ROUTE_EN = "/en/game/{URLCode?}";
    public const string ROUTE_SK = "/sk/hra/{URLCode?}";

    // Navigation -------------------------------------------------------------------------------------------------------------------------
    public static string Link(string url, string code) => URL.ReplaceSegments(url, new() {{ 1, $"{code}" }});
    // Navigator data:
    public record NavData(bool Create);
    public static readonly NavData DEFAULT_NAV_DATA = new(false);
    // Navigator state:
    public record HistoryState(bool WasRedirect, bool WasCreate);
    public static readonly HistoryState DEFAULT_HISTORY_STATE = new(false, false);
    public static class NavState {
        public static (string Key, HistoryState? Data)? New(HistoryState? state) => new(HISTORY_STATE.GAME_PAGE, state);
        public static HistoryState Get() => Navigator.State(HISTORY_STATE.GAME_PAGE, DEFAULT_HISTORY_STATE);
        public static void Set(HistoryState state) => Navigator.SetState(HISTORY_STATE.GAME_PAGE, state);
    }
    // Navigation init:
    public static void InitNavigation() {
        InitOnce.Check($"{nameof(GamePage)}.{nameof(InitNavigation)}");
        if (!URL.AppPathMatch(I18N.Link<GamePage>()) || !URL.GetQueryParams().IsEmpty())
            CreateBox.InitialValues.Delete();
        if (!CreateBox.InitialValues.AreSet()) return;
        NavState.Set(new HistoryState(false, true));
    }
    public static bool ShouldOpenCreateBox() {
        var value = Navigator.Data<NavData>()?.Create;
        if (value is bool v) return v;
        return NavState.Get().WasCreate;
    }
    // Navigation:
    private static async Task NavigateTo(bool create) {
        Navigator.AllowOne();
        await PageLoader.Show(() =>
            Navigator.NavigateTo(
                I18N.Link<GamePage>(),
                data: new NavData(create),
                state: NavState.New(new HistoryState(false, create))
            )
        , PAGE_LOADER_TASK.ANIMATION);
        Navigator.AllowAny();
    }
    public static async Task NavigateToConnect() => await NavigateTo(false);
    public static async Task NavigateToCreate() => await NavigateTo(true);

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public string? URLCode { get; set; }

    [CascadingParameter(Name = AppLayout.CASCADE_APP_LAYOUT)]
    public required AppLayoutVM LayoutVM { get; set; }

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly ConnectViewModel ConnectVM;
    private GameViewModel? GameVM;
    private GameChat? ChatRef;

    // Views ------------------------------------------------------------------------------------------------------------------------------
    public static readonly List<Type?> CONNECT_VIEWS = [typeof(ConnectBox), typeof(CreateBox)];
    public static readonly List<Type?> GAME_VIEWS = [typeof(Lobby), typeof(GameScreen)];
    public Component? View { get; private set; } = null;

    // Layout -----------------------------------------------------------------------------------------------------------------------------
    private void ShowWebLayout() {
        LayoutVM?.ShowNavigation();
        ScrollArea.ScrollTo(SCROLLAREA_ID.PAGE, 0, 0);
    }

    private void ShowGameLayout() {
        LayoutVM?.HideNavigation(false);
        ScrollArea.ScrollTo(SCROLLAREA_ID.PAGE, 0, 0);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public GamePage() {
        ConnectVM = new(new(
            Create: ShouldOpenCreateBox(),
            URLCode: () => URLCode,
            Chat: () => ChatRef,
            OnConnect: new(OnConnect),
            OnDisconnect: new(OnDisconnect),
            Notify: new(Notify)
        ));
        GameVM = null;
    }

    protected override async Task OnPageInitializedAsync() { ShowWebLayout(); await ConnectVM.OnPageInitializedAsync(); }
    protected override async Task OnPageParametersSetAsync(bool firstTime) => await ConnectVM.OnPageParametersSetAsync();
    protected override async ValueTask OnPageDisposeAsync() => await ConnectVM.OnPageDisposeAsync();

    // Events -----------------------------------------------------------------------------------------------------------------------------
    private void OnConnect(GameViewModel vm) {
        // 1) ViewModel:
        GameVM = vm;
        GameVM.InitUI();
        // 2) Layout:
        ShowGameLayout();
    }

    private void OnDisconnect() {
        // 1) ViewModel:
        GameVM?.DisposeUI();
        GameVM = null;
        // 2) Layout:
        ShowWebLayout();
    }
}

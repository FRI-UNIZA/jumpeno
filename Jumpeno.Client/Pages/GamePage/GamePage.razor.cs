namespace Jumpeno.Client.Pages;

public partial class GamePage {
    public const string RouteEn = "/en/game/{URLCode?}";
    public const string RouteSk = "/sk/hra/{URLCode?}";

    // Navigation -------------------------------------------------------------------------------------------------------------------------
    public static string Link(string url, string code) => URL.ReplaceSegments(url, new() {{ 1, $"{code}" }});
    // Navigator data:
    public record NavData(bool Create);
    public static readonly NavData DefaultNavData = new(false);
    // Navigator state:
    public record HistoryState(bool WasRedirect, bool WasCreate);
    public static readonly HistoryState DefaultHistoryState = new(false, false);
    public static class NavState {
        public static (string Key, HistoryState? Data)? New(HistoryState? state) => new(Constants.HistoryState.GamePage, state);
        public static HistoryState Get() => Navigator.State(Constants.HistoryState.GamePage, DefaultHistoryState);
        public static void Set(HistoryState state) => Navigator.SetState(Constants.HistoryState.GamePage, state);
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
        , PageLoaderTask.Animation);
        Navigator.AllowAny();
    }
    public static async Task NavigateToConnect() => await NavigateTo(false);
    public static async Task NavigateToCreate() => await NavigateTo(true);

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public string? UrlCode { get; set; }

    [CascadingParameter(Name = AppLayout.CascadeAppLayout)]
    public required AppLayoutVM LayoutVM { get; set; }

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly ConnectViewModel _connectVm;
    private GameViewModel? GameVM;
    private GameChat? ChatRef;

    // Views ------------------------------------------------------------------------------------------------------------------------------
    public static readonly List<Type?> ConnectViews = [typeof(ConnectBox), typeof(CreateBox)];
    public static readonly List<Type?> GameViews = [typeof(Lobby), typeof(GameScreen)];
    public Component? View { get; private set; } = null;

    // Layout -----------------------------------------------------------------------------------------------------------------------------
    private void ShowWebLayout() {
        LayoutVM?.ShowNavigation();
        ScrollArea.ScrollTo(ScrollAreaId.Page, 0, 0);
    }

    private void ShowGameLayout() {
        LayoutVM?.HideNavigation(false);
        ScrollArea.ScrollTo(ScrollAreaId.Page, 0, 0);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public GamePage() {
        _connectVm = new(new(
            Create: ShouldOpenCreateBox(),
            URLCode: () => UrlCode,
            Chat: () => ChatRef,
            OnConnect: new(OnConnect),
            OnDisconnect: new(OnDisconnect),
            Notify: new(Notify)
        ));
        GameVM = null;
    }

    protected override async Task OnPageInitializedAsync() { ShowWebLayout(); await _connectVm.OnPageInitializedAsync(); }
    protected override async Task OnPageParametersSetAsync(bool firstTime) => await _connectVm.OnPageParametersSetAsync();
    protected override async ValueTask OnPageDisposeAsync() => await _connectVm.OnPageDisposeAsync();

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

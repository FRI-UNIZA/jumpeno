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

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public GamePage() {
        ConnectVM = new(new(
            URLCode: () => URLCode,
            LayoutVM: () => LayoutVM,
            OnConnect: new(OnConnect),
            OnDisconnect: new(OnDisconnect),
            Notify: new(Notify)
        ));
        GameVM = null;
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override async Task OnPageInitializedAsync() => await ConnectVM.OnPageInitializedAsync();
    protected override async Task OnPageParametersSetAsync(bool firstTime) => await ConnectVM.OnPageParametersSetAsync();
    protected override async ValueTask OnPageDisposeAsync() => await ConnectVM.OnPageDisposeAsync();

    // Events -----------------------------------------------------------------------------------------------------------------------------
    private void OnConnect(GameViewModel vm) => GameVM = vm;
    private void OnDisconnect() => GameVM = null;
    private void Notify() => StateHasChanged();
}

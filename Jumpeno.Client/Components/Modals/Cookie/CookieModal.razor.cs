namespace Jumpeno.Client.Components;

public partial class CookieModal {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "cookie-modal";
    public const string ClassCookieTitle = "cookie-title";

    // Injections -------------------------------------------------------------------------------------------------------------------------
    [Inject]
    private CookieStorage CookieStorage { get; set; } = null!;
    [Inject]
    private MemoryStorage MemoryStorage { get; set; } = null!;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private Modal ModalRef = null!;
    private bool Unclosable = false;
    private Dictionary<Type, bool> Initial = [];
    private Dictionary<Type, bool> Selected = [];
    public string GetDialogID() => ModalRef.IdDialog;
    public bool IsOpened { get; set; } = false;

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    public readonly string Form = Utils.Form.Of<CookieModal>();
    private readonly SwitchViewModel SwitchMandatoryVM;
    private readonly SwitchViewModel SwitchFunctionalVM;
    private readonly SwitchViewModel SwitchSecurityVM;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public CookieModal() {
        SwitchMandatoryVM = new(new(
            Form: Form,
            ID: nameof(SwitchMandatoryVM),
            DefaultValue: true
        ));
        SwitchFunctionalVM = new(new(
            Form: Form,
            ID: nameof(SwitchFunctionalVM),
            DefaultValue: true,
            OnChange: new(e => UpdateSelection(typeof(Cookies.Preference), e.Value))
        ));
        SwitchSecurityVM = new(new(
            Form: Form,
            ID: nameof(SwitchSecurityVM),
            DefaultValue: true,
            OnChange: new(e => UpdateSelection(typeof(Cookies.Security), e.Value))
        ));
    }
    protected override void OnComponentInitialized() => MemoryStorage.Set(MemoryStorageKeys.CookieModal, this);

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    private static Dictionary<Type, bool> ToDictionary(List<Type> list) => list.ToDictionary(c => c, c => true);

    private bool IsSelected(Type cookieType) => Selected.ContainsKey(cookieType);

    private void UpdateSelection(Type cookieType, bool accept) {
        if (accept) Selected[cookieType] = true;
        else Selected.Remove(cookieType);
    }

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    private async Task InitSelected(bool unclosable, bool sync = true) {
        void Init()
        {
            // Get cookies:
            var acceptedCookies = CookieStorage.GetAcceptedCookies();
            // Init:
            if (unclosable && acceptedCookies.Count <= 0)
            {
                acceptedCookies = Cookies.Types;
                Initial = [];
            }
            else
            {
                Initial = ToDictionary(acceptedCookies);
            }
            // Select:
            Selected = ToDictionary(acceptedCookies);
            SwitchFunctionalVM.SetValue(IsSelected(typeof(Cookies.Preference)));
            SwitchSecurityVM.SetValue(IsSelected(typeof(Cookies.Security)));
        }
        if (sync) await HTTP.Sync(Init);
        else Init();
    }

    private bool IsStateInitial(Dictionary<Type, bool> accept) {
        return Initial.Count == accept.Count
        && !Initial.Except(accept).Any()
        && !accept.Except(Initial).Any();
    }

    public static async Task Open(bool unclosable = false, bool sync = true) 
    {
        var modal = AppEnvironment.MemoryStorage.Get<CookieModal>(MemoryStorageKeys.CookieModal);
        if (modal is not null) await modal.OpenModal(unclosable, sync);
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private async Task OpenModal(bool unclosable, bool sync = true) {
        await ModalRef.OpenLoading();
        Unclosable = unclosable;
        await InitSelected(unclosable, sync);
        StateHasChanged();
        await ModalRef.FinishLoading();
        IsOpened = true;
    }

    private async Task AcceptCookies(List<Type> accept) {
        await PageLoader.Show(PageLoaderTask.CookieConsent);
        await HTTP.Try(async() => {
            var newSelected = ToDictionary(accept);
            Selected = ToDictionary(accept);

            SwitchFunctionalVM.SetValue(IsSelected(typeof(Cookies.Preference)));
            SwitchSecurityVM.SetValue(IsSelected(typeof(Cookies.Security)));
            StateHasChanged();
           
            await Task.Delay(AppTheme.TransitionFast); // NOTE: Switch transition
            if (IsStateInitial(newSelected)) {
                await ModalRef.Close();
                return;
            }
            var body = new CookieSetDTO(
                AcceptedNames: [.. accept.Select(x => x.Name)]
            );
            await HTTP.Patch(API.Base.CookieSet, body: body);
            await ModalRef.Close();

            if ((Initial.TryGetValue(typeof(Cookies.Security), out var initialValue) && initialValue) != SwitchSecurityVM.Value)
                Navigator.Refresh();
        });
        await PageLoader.Hide(PageLoaderTask.CookieConsent);
    }

    public void OnClose() 
    {
        IsOpened = false;
    }
}

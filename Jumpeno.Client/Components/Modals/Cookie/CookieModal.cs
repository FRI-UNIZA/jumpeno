namespace Jumpeno.Client.Components;

public partial class CookieModal {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "cookie-modal";
    public const string CLASS_COOKIE_TITLE = "cookie-title";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private Modal ModalRef = null!;
    private bool Unclosable = false;
    private Dictionary<Type, bool> Initial = [];
    private Dictionary<Type, bool> Selected = [];
    public string GetDialogID() => ModalRef.ID_DIALOG;

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    public readonly string FORM = Form.Of<CookieModal>();
    private readonly SwitchViewModel SwitchMandatoryVM;
    private readonly SwitchViewModel SwitchFunctionalVM;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public CookieModal() {
        SwitchMandatoryVM = new(new(
            Form: FORM,
            ID: nameof(SwitchMandatoryVM),
            DefaultValue: true
        ));
        SwitchFunctionalVM = new(new(
            Form: FORM,
            ID: nameof(SwitchFunctionalVM),
            DefaultValue: true,
            OnChange: new(e => UpdateSelection(typeof(COOKIE.PREFERENCES), e.Value))
        ));
    }
    protected override void OnComponentInitialized() => RequestStorage.Set(nameof(CookieModal), this);

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    private static Dictionary<Type, bool> ToDictionary(List<Type> list) => list.ToDictionary(c => c, c => true);

    private bool IsSelected(Type cookieType) => Selected.ContainsKey(cookieType);

    private void UpdateSelection(Type cookieType, bool accept) {
        if (accept) Selected[cookieType] = true;
        else Selected.Remove(cookieType);
    }

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    private async Task InitSelected(bool unclosable) {
        await HTTP.Sync(() => {
            // Get cookies:
            var acceptedCookies = CookieStorage.GetAcceptedCookies();
            // Init:
            if (unclosable && acceptedCookies.Count <= 0) {
                acceptedCookies = COOKIE.TYPES;
                Initial = [];
            } else {
                Initial = ToDictionary(acceptedCookies);
            }
            // Select:
            Selected = ToDictionary(acceptedCookies);
            SwitchFunctionalVM.SetValue(IsSelected(typeof(COOKIE.PREFERENCES)));
        });
    }

    private bool IsStateInitial(Dictionary<Type, bool> accept) {
        return Initial.Count == accept.Count
        && !Initial.Except(accept).Any()
        && !accept.Except(Initial).Any();
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task OpenModal(bool unclosable) {
        Unclosable = unclosable;
        await InitSelected(unclosable);
        StateHasChanged();
        await ModalRef.Open();
    }

    private async Task AcceptCookies(List<Type> accept) {
        await PageLoader.Show(PAGE_LOADER_TASK.COOKIE_CONSENT);
        await HTTP.Try(async() => {
            var newSelected = ToDictionary(accept);
            Selected = ToDictionary(accept);
            SwitchFunctionalVM.SetValue(IsSelected(typeof(COOKIE.PREFERENCES)));
            StateHasChanged();
            await Task.Delay(AppTheme.TRANSITION_FAST); // NOTE: Switch transition
            if (IsStateInitial(newSelected)) {
                await ModalRef.Close();
                return;
            }
            var body = new CookieSetDTO(
                AcceptedNames: [.. accept.Select(x => x.Name)]
            );
            await HTTP.Patch(API.BASE.COOKIE_SET, body: body);
            await ModalRef.Close();
        });
        await PageLoader.Hide(PAGE_LOADER_TASK.COOKIE_CONSENT);
    }
}

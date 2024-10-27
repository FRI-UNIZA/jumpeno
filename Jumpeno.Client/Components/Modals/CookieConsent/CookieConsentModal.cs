namespace Jumpeno.Client.Components;

public partial class CookieConsentModal {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string COOKIE_TITLE_ID = "cookie-title";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [CascadingParameter]
    public required BaseTheme Theme { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private Modal ModalRef = null!;
    private bool Unclosable = false;
    private Dictionary<Type, bool> Initial = [];
    private Dictionary<Type, bool> Selected = [];
    public string GetDialogID() { return ModalRef.ID_DIALOG; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnInitialized() {
        RequestStorage.Set(REQUEST_STORAGE_KEYS.COOKIE_CONSENT_MODAL, this);
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private static Dictionary<Type, bool> ToDictionary(List<Type> list) {
        return list.ToDictionary(c => c, c => true);
    }

    private void InitSelected() {
        var acceptedCookies = CookieStorage.GetAcceptedCookies();
        if (acceptedCookies.Count <= 0) {
            acceptedCookies = COOKIE.TYPES;
            Initial = [];
        } else {
            Initial = ToDictionary(acceptedCookies);
        }
        Selected = ToDictionary(acceptedCookies);
    }

    private bool IsStateInitial(Dictionary<Type, bool> accept) {
        return Initial.Count == accept.Count
               && !Initial.Except(accept).Any()
               && !accept.Except(Initial).Any();
    }
    
    public async Task OpenModal(bool unclosable) {
        Unclosable = unclosable;
        InitSelected();
        StateHasChanged();
        await ModalRef.Open();
    }

    private bool IsSelected(Type cookieType) {
        return Selected.ContainsKey(cookieType);
    }

    private void UpdateSelection(Type cookieType, bool accept) {
        if (accept) Selected[cookieType] = true;
        else Selected.Remove(cookieType);
    }

    private async Task AcceptCookies(List<Type> accept) {
        try {
            await PageLoader.Show(PAGE_LOADER_TASK.COOKIE_CONSENT);
            var newSelected = ToDictionary(accept);
            Selected = ToDictionary(accept);
            StateHasChanged();
            await Task.Delay(Theme.TRANSITION_FAST);
            if (IsStateInitial(newSelected)) {
                await ModalRef.Close();
                return;
            }
            await HTTP.Patch(API.BASE.COOKIE(), body: accept.Select(x => x.Name));
            CookieStorage.CacheAcceptedCookies(accept);
            await ModalRef.Close();
        } finally {
            await PageLoader.Hide(PAGE_LOADER_TASK.COOKIE_CONSENT);
        }
    }
}

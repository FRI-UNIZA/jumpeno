namespace Jumpeno.Client.Components;

public partial class CookieModal {
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
    public string GetDialogID() => ModalRef.ID_DIALOG;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentInitialized() => RequestStorage.Set(nameof(CookieModal), this);

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
        await PageLoader.Show(PAGE_LOADER_TASK.COOKIE_CONSENT);
        await HTTP.Try(async() => {
            var newSelected = ToDictionary(accept);
            Selected = ToDictionary(accept);
            StateHasChanged();
            await Task.Delay(Theme.TRANSITION_FAST);
            if (IsStateInitial(newSelected)) {
                await ModalRef.Close();
                return;
            }
            var body = new CookieSetDTO(
                AcceptedNames: [.. accept.Select(x => x.Name)]
            );
            await HTTP.Patch(API.BASE.COOKIE_SET, body: body);
            CookieStorage.CacheAcceptedCookies(accept);
            await ModalRef.Close();
        });
        await PageLoader.Hide(PAGE_LOADER_TASK.COOKIE_CONSENT);
    }
}

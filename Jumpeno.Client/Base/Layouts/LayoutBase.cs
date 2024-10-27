namespace Jumpeno.Client.Base;

public class LayoutBase: LayoutComponentBase {
    // Static methods ---------------------------------------------------------------------------------------------------------------------
    public static LayoutBase CurrentLayout() {
        return RequestStorage.Get<LayoutBase>(REQUEST_STORAGE_KEYS.CURRENT_LAYOUT) ?? new LayoutBase();
    }
    private static void SetCurrentLayout(LayoutBase layout) {
        RequestStorage.Set(REQUEST_STORAGE_KEYS.CURRENT_LAYOUT, layout);
    }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    protected bool Key { get; private set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public LayoutBase() {
        Key = false;
        SetCurrentLayout(this);
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public void UpdateContent() {
        Key = !Key;
        StateHasChanged();
    }
}

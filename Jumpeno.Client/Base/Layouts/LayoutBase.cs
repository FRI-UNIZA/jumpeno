namespace Jumpeno.Client.Base;

public class LayoutBase : LayoutComponentBase {
    // Static methods ---------------------------------------------------------------------------------------------------------------------
    public static LayoutBase Current => RequestStorage.Get<LayoutBase>(nameof(LayoutBase)) ?? new LayoutBase();
    private static void SetCurrent(LayoutBase layout) => RequestStorage.Set(nameof(LayoutBase), layout);

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    protected bool Key { get; private set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public LayoutBase() {
        Key = false;
        SetCurrent(this);
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public void UpdateContent() {
        Key = !Key;
        StateHasChanged();
    }
}

namespace Jumpeno.Client.Base;

public class LayoutBase : LayoutComponentBase {
    // Static methods ---------------------------------------------------------------------------------------------------------------------
    public static LayoutBase Current => RequestStorage.Get<LayoutBase>(nameof(LayoutBase)) ?? new LayoutBase();
    private static void SetCurrent(LayoutBase layout) => RequestStorage.Set(nameof(LayoutBase), layout);

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    protected bool Key { get; private set; }
    protected bool ContentKey { get; private set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public LayoutBase() {
        Key = false;
        ContentKey = false;
        SetCurrent(this);
    }

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    public virtual void Notify() {
        Key = !Key;
        ContentKey = !ContentKey;
        StateHasChanged();
    }

    public void NotifyContent() {
        ContentKey = !ContentKey;
        StateHasChanged();
    }

    public void ChangeState() => StateHasChanged();
}

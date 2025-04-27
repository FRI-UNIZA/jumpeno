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
    public virtual void NotifyAll() {
        Key = !Key;
        ContentKey = !ContentKey;
        StateHasChanged();
    }

    public virtual void NotifyPage() {
        ContentKey = !ContentKey;
        StateHasChanged();
    }

    public virtual void NotifyState() => StateHasChanged();

    public void Notify(NOTIFY notify) {
        switch (notify) {
            case NOTIFY.ALL: NotifyAll(); break;
            case NOTIFY.PAGE: NotifyPage(); break;
            case NOTIFY.STATE: NotifyState(); break;
        }
    }
}

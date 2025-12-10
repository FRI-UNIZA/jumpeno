namespace Jumpeno.Client.Components;

public partial class ConfirmModal {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "confirm-modal";
    public const string CLASS_DANGER = "danger";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [CascadingParameter(Name = ThemeProvider.CASCADE_APP_THEME)]
    public required BaseTheme Theme { get; set; }
    // Variant:
    [Parameter]
    public bool Danger { get; set; } = false;
    // Content:
    [Parameter]
    public OneOf<string, List<string>>? Label { get; set; }
    [Parameter]
    public RenderFragment? Icon { get; set; }
    [Parameter]
    public RenderFragment? Message { get; set; }
    [Parameter]
    public RenderFragment? TextCancel { get; set; }
    [Parameter]
    public RenderFragment? TextOK { get; set; }
    // Events:
    [Parameter]
    public EventCallback<ConfirmModal> OnBeforeOpen { get; set; } = EventCallback<ConfirmModal>.Empty;
    [Parameter]
    public EventCallback<ConfirmModal> OnAfterOpen { get; set; } = EventCallback<ConfirmModal>.Empty;
    [Parameter]
    public EventCallback<ConfirmModal> OnBeforeClose { get; set; } = EventCallback<ConfirmModal>.Empty;
    [Parameter]
    public EventCallback<ConfirmModal> OnAfterClose { get; set; } = EventCallback<ConfirmModal>.Empty;

    // Events -----------------------------------------------------------------------------------------------------------------------------
    public virtual async Task CallOnBeforeOpen() => await OnBeforeOpen.InvokeAsync(this);
    public virtual async Task CallOnAfterOpen() => await OnAfterOpen.InvokeAsync(this);
    public virtual async Task CallOnBeforeClose() => await OnBeforeClose.InvokeAsync(this);
    public virtual async Task CallOnAfterClose() => await OnAfterClose.InvokeAsync(this);

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private Modal ModalRef = null!;
    // Action:
    private EmptyDelegate Action = EmptyDelegate.EMPTY;
    private bool Loader = true;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(CLASS, Base)
        .Set(CLASS_DANGER, Danger);
    }

    // Open -------------------------------------------------------------------------------------------------------------------------------
    private async Task Open(EmptyDelegate action, bool loader) {
        Action = action;
        Loader = loader;
        await ModalRef.Open();
    }
    public async Task Open(Func<Task> action, bool loader = true) => await Open(new EmptyDelegate(action), loader);
    public async Task Open(Action action, bool loader = true) => await Open(new EmptyDelegate(action), loader);

    // Close ------------------------------------------------------------------------------------------------------------------------------
    public async Task Close() => await ModalRef.Close();

    // Confirm ----------------------------------------------------------------------------------------------------------------------------
    private async Task Confirm() {
        try {
            await PageLoader.Show(PAGE_LOADER_TASK.CONFIRM, !Loader);
            if (Loader) await Task.Delay(Theme.TRANSITION_FAST);
            await ModalRef.Close();
            await Action.Invoke();
        } finally {
            await PageLoader.Hide(PAGE_LOADER_TASK.CONFIRM, Loader);
        }
    }
}

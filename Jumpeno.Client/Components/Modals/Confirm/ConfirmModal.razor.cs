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
    // Events [open]:
    [Parameter]
    public EventCallback<ConfirmModal> OnBeforeOpenStart { get; set; } = EventCallback<ConfirmModal>.Empty;
    [Parameter]
    public EventCallback<ConfirmModal> OnOpenStart { get; set; } = EventCallback<ConfirmModal>.Empty;
    [Parameter]
    public EventCallback<ConfirmModal> OnOpenFinish { get; set; } = EventCallback<ConfirmModal>.Empty;
    [Parameter]
    public EventCallback<ConfirmModal> OnAfterOpenFinish { get; set; } = EventCallback<ConfirmModal>.Empty;
    // Events [close]:
    [Parameter]
    public EventCallback<ConfirmModal> OnBeforeCloseStart { get; set; } = EventCallback<ConfirmModal>.Empty;
    [Parameter]
    public EventCallback<ConfirmModal> OnCloseStart { get; set; } = EventCallback<ConfirmModal>.Empty;
    [Parameter]
    public EventCallback<ConfirmModal> OnCloseFinish { get; set; } = EventCallback<ConfirmModal>.Empty;
    [Parameter]
    public EventCallback<ConfirmModal> OnAfterCloseFinish { get; set; } = EventCallback<ConfirmModal>.Empty;

    // Events -----------------------------------------------------------------------------------------------------------------------------
    public virtual async Task CallOnBeforeOpenStart() => await OnBeforeOpenStart.InvokeAsync(this);
    public virtual async Task CallOnOpenStart() => await OnOpenStart.InvokeAsync(this);
    public virtual async Task CallOnOpenFinish() => await OnOpenFinish.InvokeAsync(this);
    public virtual async Task CallOnAfterOpenFinish() => await OnAfterOpenFinish.InvokeAsync(this);
    // Close:
    public virtual async Task CallOnBeforeCloseStart() => await OnBeforeCloseStart.InvokeAsync(this);
    public virtual async Task CallOnCloseStart() => await OnCloseStart.InvokeAsync(this);
    public virtual async Task CallOnCloseFinish() => await OnCloseFinish.InvokeAsync(this);
    public virtual async Task CallOnAfterCloseFinish() => await OnAfterCloseFinish.InvokeAsync(this);

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
    private Task Open(EmptyDelegate? init, EmptyDelegate action, bool loader) => ModalRef.Open(async () => {
        Action = action;
        Loader = loader;
        if (init != null) await init.Invoke();
    });
    public Task Open(Func<Task> action, bool loader = true) => Open(null, new EmptyDelegate(action), loader);
    public Task Open(Action init, Func<Task> action, bool loader = true) => Open(new EmptyDelegate(init), new EmptyDelegate(action), loader);
    public Task Open(Func<Task> init, Func<Task> action, bool loader = true) => Open(new EmptyDelegate(init), new EmptyDelegate(action), loader);
    public Task Open(Action action, bool loader = true) => Open(null, new EmptyDelegate(action), loader);
    public Task Open(Action init, Action action, bool loader = true) => Open(new EmptyDelegate(init), new EmptyDelegate(action), loader);
    public Task Open(Func<Task> init, Action action, bool loader = true) => Open(new EmptyDelegate(init), new EmptyDelegate(action), loader);

    // Close ------------------------------------------------------------------------------------------------------------------------------
    public Task Close() => ModalRef.Close();
    public Task Close(Action dispose) => ModalRef.Close(dispose);
    public Task Close(Func<Task> dispose) => ModalRef.Close(dispose);

    // Confirm ----------------------------------------------------------------------------------------------------------------------------
    private async Task Confirm() {
        try {
            await PageLoader.Show(PageLoaderTask.CONFIRM, !Loader);
            if (Loader) await Task.Delay(Theme.TRANSITION_FAST);
            await ModalRef.Close();
            await Action.Invoke();
        } finally {
            await PageLoader.Hide(PageLoaderTask.CONFIRM, Loader);
        }
    }
}

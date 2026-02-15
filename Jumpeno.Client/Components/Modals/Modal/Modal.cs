namespace Jumpeno.Client.Components;

public partial class Modal {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID_PREFIX = "modal";
    public const string ID_DIALOG_INIT_PREFIX = "modal-dialog-init";
    public const string ID_DIALOG_START_PREFIX = "modal-dialog-start";
    public const string ID_DIALOG_PREFIX = "modal-dialog";
    public const string ID_DIALOG_END_PREFIX = "modal-dialog-end";
    // Class:
    public const string CLASS = ID_PREFIX;
    public const string CLASS_BACKDROP = "modal-backdrop";
    public const string CLASS_INIT = "modal-init";
    public const string CLASS_START = "modal-start";
    public const string CLASS_CONTAINER = "modal-container";
    public const string CLASS_LOADING_INDICATOR = "modal-loading-indicator";
    public const string CLASS_DIALOG = "modal-dialog";
    public const string CLASS_HEADER = "modal-header";
    public const string CLASS_SUB_HEADER = "modal-sub-header";
    public const string CLASS_SCROLL = "modal-scroll";
    public const string CLASS_CONTENT = "modal-content";
    /// <summary>Important wrapper for proper padding on mobile devices.</summary>
    public const string CLASS_CONTENT_BOX = "modal-content-box";
    public const string CLASS_FOOTER = "modal-footer";
    public const string CLASS_END = "modal-end";
    // Settings:
    public const string CLASS_NO_HEADER = "no-header";
    public const string CLASS_NO_FOOTER = "no-footer";
    public const string CLASS_UNCLOSABLE = "unclosable";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    // Surface:
    [Parameter]
    public MODAL_SURFACE? Surface { get; set; } = MODAL_SURFACE.FLOATING;
    // Content:
    [Parameter]
    public required OneOf<string, List<string>> Label { get; set; }
    [Parameter]
    public bool NoHeader { get; set; }
    [Parameter]
    public RenderFragment? Header { get; set; }
    [Parameter]
    public RenderFragment? SubHeader { get; set; }
    [Parameter]
    public RenderFragment? Content { get; set; }
    [Parameter]
    public bool NoFooter { get; set; }
    [Parameter]
    public RenderFragment? Footer { get; set; }
    // Scrollbars:
    [Parameter]
    public SCROLLAREA_AUTOHIDE ScrollAutoHide { get; set; } = SCROLLAREA_AUTOHIDE.MOVE;
    [Parameter]
    public bool NoInitScroll { get; set; } = false;
    // Loading (ms):
    [Parameter]
    public uint MinLoading { get; set; } = 300;
    // Abilities:
    [Parameter]
    public bool Unclosable { get; set; } = false;
    // Events [open]:
    [Parameter]
    public EventCallback<Modal> OnBeforeOpenStart { get; set; } = EventCallback<Modal>.Empty;
    [Parameter]
    public EventCallback<Modal> OnOpenStart { get; set; } = EventCallback<Modal>.Empty;
    [Parameter]
    public EventCallback<Modal> OnOpenFinish { get; set; } = EventCallback<Modal>.Empty;
    [Parameter]
    public EventCallback<Modal> OnAfterOpenFinish { get; set; } = EventCallback<Modal>.Empty;
    // Events [close]:
    [Parameter]
    public EventCallback<Modal> OnBeforeCloseStart { get; set; } = EventCallback<Modal>.Empty;
    [Parameter]
    public EventCallback<Modal> OnCloseStart { get; set; } = EventCallback<Modal>.Empty;
    [Parameter]
    public EventCallback<Modal> OnCloseFinish { get; set; } = EventCallback<Modal>.Empty;
    [Parameter]
    public EventCallback<Modal> OnAfterCloseFinish { get; set; } = EventCallback<Modal>.Empty;

    // Events -----------------------------------------------------------------------------------------------------------------------------
    public virtual async Task CallOnBeforeOpenStart() => await OnBeforeOpenStart.InvokeAsync(this);
    public virtual async Task CallOnOpenStart() => await OnOpenStart.InvokeAsync(this);
    public virtual async Task CallOnOpenFinish() { 
        if (!NoInitScroll && ScrollAutoHide != SCROLLAREA_AUTOHIDE.NEVER) ScrollAreaRef.InitScrollTo(0, 0);
        await OnOpenFinish.InvokeAsync(this);
    }
    public virtual async Task CallOnAfterOpenFinish() => await OnAfterOpenFinish.InvokeAsync(this);
    // Close:
    public virtual async Task CallOnBeforeCloseStart() => await OnBeforeCloseStart.InvokeAsync(this);
    public virtual async Task CallOnCloseStart() => await OnCloseStart.InvokeAsync(this);
    public virtual async Task CallOnCloseFinish() => await OnCloseFinish.InvokeAsync(this);
    public virtual async Task CallOnAfterCloseFinish() => await OnAfterCloseFinish.InvokeAsync(this);

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public readonly string ID;
    public readonly string ID_DIALOG_INIT;
    public readonly string ID_DIALOG_START;
    public readonly string ID_DIALOG;
    public readonly string ID_DIALOG_END;
    public bool CreatedLoading { get; private set; }
    public MODAL_STATE State { get; private set; }
    public required ScrollArea ScrollAreaRef { get; set; }
    
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public Modal() {
        ID = IDGenerator.Generate(ID_PREFIX);
        ID_DIALOG_INIT = $"{ID_DIALOG_INIT_PREFIX}-{ID}";
        ID_DIALOG_START = $"{ID_DIALOG_START_PREFIX}-{ID}";
        ID_DIALOG = $"{ID_DIALOG_PREFIX}-{ID}";
        ID_DIALOG_END = $"{ID_DIALOG_END_PREFIX}-{ID}";
        State = MODAL_STATE.CLOSED;
    }

    protected override async Task OnComponentParametersSetAsync(bool firstTime) {
        if (State == MODAL_STATE.OPEN) await ModalProvider.NotifyElement(this);
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private async Task Open(bool loading = false, EmptyDelegate? init = null) {
        AppEnvironment.AssertClient();
        await ModalProvider.CreateModal(this, new(async () => {
            CreatedLoading = loading;
            if (init != null) await init.Invoke();
        }));
    }
    public Task Open() => Open(false);
    public Task Open(Action init) => Open(false, new(init));
    public Task Open(Func<Task> init) => Open(false, new(init));

    // Loading:
    public Task OpenLoading() => Open(true);
    public Task OpenLoading(Action init) => Open(true, new(init));
    public Task OpenLoading(Func<Task> init) => Open(true, new(init));
    public async Task FinishLoading() {
        AppEnvironment.AssertClient();
        await ModalProvider.FinishLoading(this);
    }
    public async Task CloseLoading() {
        AppEnvironment.AssertClient();
        await ModalProvider.DestroyLoadingModal(this);
    }

    // Close:
    private async Task Close(EmptyDelegate? dispose = null) {
        AppEnvironment.AssertClient();
        await ModalProvider.DestroyModal(this, dispose);
    }
    public Task Close() => Close((EmptyDelegate?)null);
    public Task Close(Action dispose) => Close(new EmptyDelegate(dispose));
    public Task Close(Func<Task> dispose) => Close(new EmptyDelegate(dispose));
    public static async Task CloseAllAbove(Modal root) {
        AppEnvironment.AssertClient();
        await ModalProvider.CloseAllAbove(root);
    }
    public static async Task CloseAll() {
        AppEnvironment.AssertClient();
        await ModalProvider.CloseAllAbove();
    }
}

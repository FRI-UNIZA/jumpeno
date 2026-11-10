namespace Jumpeno.Client.Components;

public partial class Modal {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string IdPrefix = "modal";
    public const string IdDialogInitPrefix = "modal-dialog-init";
    public const string IdDialogStartPrefix = "modal-dialog-start";
    public const string IdDialogPrefix = "modal-dialog";
    public const string IdDialogEndPrefix = "modal-dialog-end";
    // Class:
    public const string ClassName = IdPrefix;
    public const string ClassBackdrop = "modal-backdrop";
    public const string ClassInit = "modal-init";
    public const string ClassStart = "modal-start";
    public const string ClassContainer = "modal-container";
    public const string ClassLoadingIndicator = "modal-loading-indicator";
    public const string ClassDialog = "modal-dialog";
    public const string ClassHeader = "modal-header";
    public const string ClassSubHeader = "modal-sub-header";
    public const string ClassScroll = "modal-scroll";
    public const string ClassContent = "modal-content";
    /// <summary>Important wrapper for proper padding on mobile devices.</summary>
    public const string ClassContentBox = "modal-content-box";
    public const string ClassFooter = "modal-footer";
    public const string ClassEnd = "modal-end";
    // Settings:
    public const string ClassNoHeader = "no-header";
    public const string ClassNoFooter = "no-footer";
    public const string ClassUnclosable = "unclosable";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    // Surface:
    [Parameter]
    public ModalSurface? Surface { get; set; } = ModalSurface.Floating;
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
    public ScrollAreaAutoHide ScrollAutoHide { get; set; } = ScrollAreaAutoHide.Move;
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
        if (!NoInitScroll && ScrollAutoHide != ScrollAreaAutoHide.Never) ScrollAreaRef.InitScrollTo(0, 0);
        await OnOpenFinish.InvokeAsync(this);
    }
    public virtual async Task CallOnAfterOpenFinish() => await OnAfterOpenFinish.InvokeAsync(this);
    // Close:
    public virtual async Task CallOnBeforeCloseStart() => await OnBeforeCloseStart.InvokeAsync(this);
    public virtual async Task CallOnCloseStart() => await OnCloseStart.InvokeAsync(this);
    public virtual async Task CallOnCloseFinish() => await OnCloseFinish.InvokeAsync(this);
    public virtual async Task CallOnAfterCloseFinish() => await OnAfterCloseFinish.InvokeAsync(this);

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public readonly string Id;
    public readonly string IdDialogInit;
    public readonly string IdDialogStart;
    public readonly string IdDialog;
    public readonly string IdDialogEnd;
    public bool CreatedLoading { get; private set; }
    public ModalStateType State { get; private set; }
    public required ScrollArea ScrollAreaRef { get; set; }
    
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public Modal() {
        Id = IDGenerator.Generate(IdPrefix);
        IdDialogInit = $"{IdDialogInitPrefix}-{Id}";
        IdDialogStart = $"{IdDialogStartPrefix}-{Id}";
        IdDialog = $"{IdDialogPrefix}-{Id}";
        IdDialogEnd = $"{IdDialogEndPrefix}-{Id}";
        State = ModalStateType.Closed;
    }

    protected override async Task OnComponentParametersSetAsync(bool firstTime) {
        if (State == ModalStateType.Open) await ModalProvider.NotifyElement(this);
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

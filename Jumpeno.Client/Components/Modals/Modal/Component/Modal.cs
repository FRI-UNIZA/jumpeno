namespace Jumpeno.Client.Components;

public partial class Modal {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID_PREFIX = "modal";
    public const string ID_DIALOG_START_PREFIX = "modal-dialog-start";
    public const string ID_DIALOG_PREFIX = "modal-dialog";
    public const string ID_DIALOG_END_PREFIX = "modal-dialog-end";

    public const string CLASS_MODAL = ID_PREFIX;
    public const string CLASS_BACKDROP = "modal-backdrop";
    public const string CLASS_START = "modal-start";
    public const string CLASS_CONTAINER = "modal-container";
    public const string CLASS_LOADING_INDICATOR = "modal-loading-indicator";
    public const string CLASS_DIALOG = "modal-dialog";
    public const string CLASS_HEADER = "modal-header";
    public const string CLASS_SCROLL = "modal-scroll";
    public const string CLASS_CONTENT = "modal-content";
    public const string CLASS_FOOTER = "modal-footer";
    public const string CLASS_END = "modal-end";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    // Content:
    [Parameter]
    public required string Label { get; set; }
    [Parameter]
    public bool NoHeader { get; set; }
    [Parameter]
    public RenderFragment? Header { get; set; }
    [Parameter]
    public RenderFragment? Content { get; set; }
    [Parameter]
    public bool NoFooter { get; set; }
    [Parameter]
    public RenderFragment? Footer { get; set; }

    // Dimensions:
    [Parameter]
    public string? MinWidth { get; set; }
    [Parameter]
    public string? MaxWidth { get; set; }
    [Parameter]
    public string? MinHeight { get; set; }
    [Parameter]
    public string? MaxHeight { get; set; }

    // Abilities:
    [Parameter]
    public bool Unclosable { get; set; } = false;

    // Callbacks:
    [Parameter]
    public EventCallback<Modal> OnBeforeOpen { get; set; } = EventCallback<Modal>.Empty;
    [Parameter]
    public EventCallback<Modal> OnAfterOpen { get; set; } = EventCallback<Modal>.Empty;
    [Parameter]
    public EventCallback<Modal> OnBeforeClose { get; set; } = EventCallback<Modal>.Empty;
    [Parameter]
    public EventCallback<Modal> OnAfterClose { get; set; } = EventCallback<Modal>.Empty;

    // Styling:
    [Parameter]
    public string Class { get; set; } = "";
    [Parameter]
    public string Style { get; set; } = "";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public readonly string ID;
    public readonly string ID_DIALOG_START;
    public readonly string ID_DIALOG;
    public readonly string ID_DIALOG_END;
    public bool CreatedLoading { get; private set; }
    public MODAL_STATE State { get; private set; }
    public required ScrollArea ScrollAreaRef { get; set; }
    
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public Modal() {
        ID = ComponentService.GenerateID(ID_PREFIX);
        ID_DIALOG_START = $"{ID_DIALOG_START_PREFIX}-{ID}";
        ID_DIALOG = $"{ID_DIALOG_PREFIX}-{ID}";
        ID_DIALOG_END = $"{ID_DIALOG_END_PREFIX}-{ID}";
        State = MODAL_STATE.CLOSED;
    }

    protected override async Task OnParametersSetAsync() {
        MaxWidth ??= "100vw";
        MaxHeight ??= "100vh";
        if (State == MODAL_STATE.OPEN) {
            await ModalProvider.NotifyElement(this);
        }
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private async Task Open(bool loading = false) {
        AppEnvironment.CheckClient();
        CreatedLoading = loading;
        await ModalProvider.CreateModal(this);
    }
    public async Task Open() { await Open(false); }
    public async Task OpenLoading() { await Open(true); }
    public async Task FinishLoading() {
        AppEnvironment.CheckClient();
        await ModalProvider.NotifyFinishLoading(this);
    }

    public async Task Close() {
        AppEnvironment.CheckClient();
        await ModalProvider.DestroyModal(this);
    }
    public static async Task CloseAllAbove(Modal root) {
        AppEnvironment.CheckClient();
        await ModalProvider.CloseAllAbove(root);
    }
    public static async Task CloseAll() {
        AppEnvironment.CheckClient();
        await ModalProvider.CloseAllAbove(null);
    }
}

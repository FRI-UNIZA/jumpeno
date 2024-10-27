namespace Jumpeno.Client.Components;

using System.Reflection;

public partial class ModalProvider {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASSNAME_MODAL_PROVIDER_CONTENT = "modal-provider-content";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly Dictionary<string, ModalElement> ElementDictionary = [];
    private readonly List<Modal> ModalList = [];
    private readonly SemaphoreSlim ModalLock = new(1, 1);
    private readonly SemaphoreSlim ElementLock = new(1, 1);
    private TaskCompletionSource TCSOpen = null!;
    private TaskCompletionSource TCSDispose = null!;

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [CascadingParameter]
    public bool PageLoaderDisplayed { get; set; }
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public ModalProvider(): base() {
        if (AppEnvironment.IsServer()) return;
        JS.InvokeVoid(JSModal.Init);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override async Task OnInitializedAsync() {
        await Navigator.AddBeforeEventListener(async e => {
            // Close all modals before navigation
            await CloseAllAbove(null);
        });
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    // Setting:
    private static void SetModalState(Modal modal, MODAL_STATE state) {
        Type type = typeof(Modal);
        PropertyInfo? prop = type.GetProperty("State");
        if (prop is null) return;
        MethodInfo? setMethod = prop.GetSetMethod(nonPublic: true);
        if (setMethod is null) return;
        setMethod.Invoke(modal, [state]);
    }

    // Opening:
    public static async Task CreateModal(Modal modal) {
        var instance = Instance();
        await instance.ModalLock.WaitAsync();
        ActionHandler.SaveActiveElement();
        await PageLoader.Show(PAGE_LOADER_TASK.MODAL, true);
        SetModalState(modal, MODAL_STATE.PRE_OPEN);
        instance.ModalList.Add(modal);
        instance.StateHasChanged();
    }

    public static async Task AddElement(ModalElement element) {
        var instance = Instance();
        await instance.ElementLock.WaitAsync();
        instance.ElementDictionary.Add(element.Modal.ID, element);
        instance.ElementLock.Release();
        JS.InvokeVoid(JSModal.PreOpen, element.Modal.ID);
    }

    private static async Task SetModalOpen(string id) {
        var instance = Instance();
        var element = instance.ElementDictionary[id];
        if (element is null) return;
        instance.TCSOpen = new TaskCompletionSource();
        SetModalState(element.Modal, MODAL_STATE.OPEN);
        element.TriggerStateChange();
        await instance.TCSOpen.Task;
        await PageLoader.Hide(PAGE_LOADER_TASK.MODAL);
        ActionHandler.SetFocus(element.Modal.ID_DIALOG);
        await element.Modal.OnAfterOpen.InvokeAsync(element.Modal);
    }

    public static void NotifyOpen() {
        Instance().TCSOpen.TrySetResult();
    }

    // Notification:
    public static async Task NotifyElement(Modal modal) {
        var instance = Instance();
        await instance.ElementLock.WaitAsync();
        instance.ElementDictionary.TryGetValue(modal.ID, out var element);
        element?.TriggerStateChange(); 
        instance.ElementLock.Release();
    }

    public static async Task NotifyFinishLoading(Modal modal) {
        await modal.OnBeforeOpen.InvokeAsync(modal);
        SetModalState(modal, MODAL_STATE.OPENING);
        await NotifyElement(modal);
    }

    // Closing:
    public static async Task DestroyModal(Modal modal, bool withLock = true) {
        var instance = Instance();
        if (withLock) await instance.ModalLock.WaitAsync();
        await PageLoader.Show(PAGE_LOADER_TASK.MODAL, true);
        await modal.OnBeforeClose.InvokeAsync(modal);

        instance.ElementDictionary.TryGetValue(modal.ID, out var element);
        instance.TCSDispose = new TaskCompletionSource();
        element?.StartClosing();
        await instance.TCSDispose.Task;
        
        await PageLoader.Hide(PAGE_LOADER_TASK.MODAL);
        await ActionHandler.RestoreFocusAsync();
        await modal.OnAfterClose.InvokeAsync(modal);
        if (withLock) instance.ModalLock.Release();
    }

    public static async Task RemoveElement(ModalElement element) {
        var instance = Instance();
        await instance.ElementLock.WaitAsync();
        instance.ElementDictionary.Remove(element.Modal.ID);
        instance.ModalList.Remove(element.Modal);
        instance.StateHasChanged();
        instance.ElementLock.Release();
    }

    public static void NotifyDispose(Modal modal) {
        SetModalState(modal, MODAL_STATE.CLOSED);
        Instance().TCSDispose.TrySetResult();
    }

    public static async Task CloseAllAbove(Modal? modal) {
        var instance = Instance();
        for (var i = instance.ModalList.Count - 1; i >= 0; i--) {
            var displayed = instance.ModalList[i];
            if (displayed == modal) break;
            await displayed.Close();
        }
    }

    // JS Interop -------------------------------------------------------------------------------------------------------------------------
    [JSInvokable]
    public static async Task JS_ModalPreOpened(string id) {
        var element = Instance().ElementDictionary[id];
        SetModalState(element.Modal, element.Modal.CreatedLoading ? MODAL_STATE.LOADING : MODAL_STATE.OPENING);
        if (!element.Modal.CreatedLoading) await element.Modal.OnBeforeOpen.InvokeAsync(element.Modal);
        element.TriggerStateChange();
    }
    [JSInvokable]
    public static async Task JS_ModalOpened(string id) {
        await SetModalOpen(id);
        Instance().ModalLock.Release();
    }
    [JSInvokable]
    public static async Task JS_ModalClosed(string id) {
        Instance().ElementDictionary.TryGetValue(id, out var element);
        if (element is not null) {
            await RemoveElement(element);
        }
    }
    [JSInvokable]
    public static async Task JS_ModalESCPressed() {
        var instance = Instance();
        await instance.ModalLock.WaitAsync();
        if (instance.ModalList.Count > 0) {
            var modal = instance.ModalList[^1];
            if (modal.State == MODAL_STATE.OPEN && !modal.Unclosable) {
                await DestroyModal(modal, withLock: false);
            }
        }
        instance.ModalLock.Release();
    }
}

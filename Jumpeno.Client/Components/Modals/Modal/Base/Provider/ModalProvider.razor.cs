namespace Jumpeno.Client.Components;

public partial class ModalProvider {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassContent = "modal-provider-content";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly Dictionary<string, ModalElement> ElementDictionary = [];
    private readonly List<Modal> ModalList = [];
    private readonly LockerSlim ElementLock = new();
    private TaskCompletionSource TCSLoading = null!;
    private TaskCompletionSource TCSOpen = null!;
    private TaskCompletionSource TCSOpened = null!;
    private TaskCompletionSource TCSDispose = null!;
    private readonly MinWatch MinLoadingWatch = new();

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [CascadingParameter(Name = PageLoader.CascadePageLoaderDisplayed)]
    public bool PageLoaderDisplayed { get; set; }
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public ModalProvider() : base() {
        if (AppEnvironment.IsServer) return;
        JS.InvokeVoid(JSModal.Init);
    }

    protected override async ValueTask OnComponentDisposeAsync() {
        await ElementLock.DisposeSafe();
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    // Setting:
    private static void SetModalState(Modal modal, ModalStateType state) {
        Type type = typeof(Modal);
        PropertyInfo? prop = type.GetProperty(nameof(Modal.State));
        if (prop is null) return;
        MethodInfo? setMethod = prop.GetSetMethod(nonPublic: true);
        if (setMethod is null) return;
        setMethod.Invoke(modal, [state]);
    }

    // Opening:
    public static async Task CreateModal(Modal modal, EmptyDelegate? init = null) {
        var instance = Instance();

        // 1) Lock:
        await UI.Lock.TryLock();
        if (modal.State != ModalStateType.Closed) {
            UI.Lock.TryUnlock(); return;
        }

        // 2) Before start events:
        if (init != null) await init.Invoke();
        await modal.CallOnBeforeOpenStart();

        // 3) Block input:
        ActionHandler.SaveActiveElement();
        await PageLoader.Show(PageLoaderTask.Modal, true);

        // 4) Min loading:
        if (modal.CreatedLoading) instance.MinLoadingWatch.Start(modal.MinLoading);
        instance.TCSLoading = new();

        // 5) Start events:
        await modal.CallOnOpenStart();

        // 6) Set state:
        SetModalState(modal, ModalStateType.PreOpen);
        instance.TCSOpened = new();
        instance.ModalList.Add(modal);
        instance.StateHasChanged();

        // 7) Await until opened:
        if (!modal.CreatedLoading) await instance.TCSOpened.Task;
    }

    public static async Task AddElement(ModalElement element) {
        var instance = Instance(); await instance.ElementLock.TryExclusive(() => {
            instance.ElementDictionary.Add(element.Modal.Id, element);
        });
        JS.InvokeVoid(JSModal.PreOpen, element.Modal.Id);
    }

    private static async Task SetModalOpen(string id) {
        var instance = Instance();

        // 1) Check element:
        var element = instance.ElementDictionary[id];
        if (element is null) return;

        // 2) Set state:
        instance.TCSOpen = new TaskCompletionSource();
        SetModalState(element.Modal, ModalStateType.Open);
        element.Notify();
        await instance.TCSOpen.Task;

        // 3) Unblock input:
        await PageLoader.Hide(PageLoaderTask.Modal, false);
        ActionHandler.SetFocus(element.Modal.IdDialog);

        // 4) After finish events:
        await element.Modal.CallOnAfterOpenFinish();

        // 5) Notify & unlock:
        instance.TCSOpened.TrySetResult();
        UI.Lock.TryUnlock();
    }

    public static void NotifyOpen() => Instance().TCSOpen.TrySetResult();

    // Notification:
    public static async Task NotifyElement(Modal modal) {
        var instance = Instance(); await instance.ElementLock.TryExclusive(() => {
            instance.ElementDictionary.TryGetValue(modal.Id, out var element);
            element?.Notify(); 
        });
    }

    private static async Task AwaitLoading(Modal modal) {
        var instance = Instance();
        if (!modal.CreatedLoading) return;
        await instance.MinLoadingWatch.Task;
    }

    public static async Task FinishLoading(Modal modal) {
        var instance = Instance();
        await instance.TCSLoading.Task;
        await AwaitLoading(modal);
        SetModalState(modal, ModalStateType.Openning);
        await modal.CallOnOpenFinish();
        await NotifyElement(modal);
        await instance.TCSOpened.Task;
    }

    // Closing:
    public static async Task DestroyLoadingModal(Modal modal) {
        var instance = Instance();
        await instance.TCSLoading.Task;
        await AwaitLoading(modal);

        instance.ElementDictionary.TryGetValue(modal.Id, out var element);
        instance.TCSDispose = new TaskCompletionSource();
        element?.StartClosing();
        await instance.TCSDispose.Task;

        await PageLoader.Hide(PageLoaderTask.Modal, false);
        await ActionHandler.RestoreFocusAsync();
        UI.Lock.TryUnlock();
    }
    
    private static async Task DestroyModal(Modal modal, EmptyDelegate? dispose = null, bool withLock = true) {
        var instance = Instance();

        // 1) Lock:
        if (withLock) await UI.Lock.TryLock();
        if (modal.State != ModalStateType.Open) {
            if (withLock) UI.Lock.TryUnlock(); return;
        }

        // 2) Before start events:
        if (dispose != null) await dispose.Invoke();
        await modal.CallOnBeforeCloseStart();

        // 3) Block input:
        await PageLoader.Show(PageLoaderTask.Modal, true);

        // 4) Start events:
        await modal.CallOnCloseStart();

        // 5) Check element:
        instance.ElementDictionary.TryGetValue(modal.Id, out var element);
        if (element == null) {
            await PageLoader.Hide(PageLoaderTask.Modal, false);
            if (withLock) UI.Lock.TryUnlock();
            return;
        }

        // 6) Await close:
        instance.TCSDispose = new TaskCompletionSource();
        element.StartClosing();
        await instance.TCSDispose.Task;

        // 7) Finish events:
        await modal.CallOnCloseFinish();

        // 8) Unblock input:
        await PageLoader.Hide(PageLoaderTask.Modal, false);
        await ActionHandler.RestoreFocusAsync();

        // 9) After finish events:
        await modal.CallOnAfterCloseFinish();

        // 10) Unlock:
        if (withLock) UI.Lock.TryUnlock();
    }

    public static async Task DestroyModal(Modal modal, EmptyDelegate? dispose = null) => await DestroyModal(modal, dispose, true);

    public static async Task RemoveElement(ModalElement element) {
        var instance = Instance(); await instance.ElementLock.TryExclusive(() => {
            instance.ElementDictionary.Remove(element.Modal.Id);
            instance.ModalList.Remove(element.Modal);
            instance.StateHasChanged();
        });
    }

    public static void NotifyDispose(Modal modal) {
        SetModalState(modal, ModalStateType.Closed);
        Instance().TCSDispose.TrySetResult();
    }

    public static async Task CloseAllAbove(Modal? modal = null) {
        var instance = Instance(); await UI.Lock.TryExclusive(async () => {
            for (var i = instance.ModalList.Count - 1; i >= 0; i--) {
                var displayed = instance.ModalList[i];
                if (displayed == modal) break;
                await DestroyModal(displayed, withLock: false);
            }
        });
    }

    // JS Interop -------------------------------------------------------------------------------------------------------------------------
    [JSInvokable]
    public static async Task JS_ModalPreOpened(string id) {
        var instance = Instance();
        var element = instance.ElementDictionary[id];
        SetModalState(element.Modal, element.Modal.CreatedLoading ? ModalStateType.Loading : ModalStateType.Openning);
        if (!element.Modal.CreatedLoading) await element.Modal.CallOnOpenFinish();
        element.Notify();
        instance.TCSLoading.TrySetResult();
    }
    [JSInvokable]
    public static Task JS_ModalOpened(string id) => SetModalOpen(id);
    [JSInvokable]
    public static async Task JS_ModalClosed(string id) {
        Instance().ElementDictionary.TryGetValue(id, out var element);
        if (element is not null) {
            await RemoveElement(element);
        }
    }
    [JSInvokable]
    public static async Task JS_ModalESCPressed() {
        var instance = Instance(); await UI.Lock.TryExclusive(async () => {
            if (instance.ModalList.Count <= 0) return;
            var modal = instance.ModalList[^1];
            if (modal.State == ModalStateType.Open && !modal.Unclosable) {
                await DestroyModal(modal, withLock: false);
            }
        });
    }
}

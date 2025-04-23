namespace Jumpeno.Client.Base;

public abstract class SSRComponent<T> : Component, IAsyncDisposable {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string KEY_PREFIX = "component";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public string Key { get; private set; }
    [Inject]
    public required PersistentComponentState ApplicationState { private get; set; }
    private PersistingComponentStateSubscription PersistingSubscription;

    // Useful properties ------------------------------------------------------------------------------------------------------------------
    public COMPONENT_STATE State { get; private set; }
    public bool Initializing { get; private set; }
    public T Data { get; private set; }
    public Exception? Exception { get; private set; }

    // Useful methods ---------------------------------------------------------------------------------------------------------------------
    /// <summary>Handles state changes while load function executes.</summary>
    /// <param name="loadFunction"></param>
    protected async Task Load(Func<Task<T>> loadFunction) {
        try {
            State = COMPONENT_STATE.LOADING;
            Exception = null;
            var data = await loadFunction();
            State = COMPONENT_STATE.DONE;
            Data = data;
        } catch (Exception e) {
            State = COMPONENT_STATE.ERROR;
            Exception = e;
            Data = EmptyData();
        }
    }
    /// <summary>Returns empty data to show.</summary>
    /// <returns>Empty data</returns>
    protected abstract T EmptyData();

    // Lifecycle overrides ----------------------------------------------------------------------------------------------------------------
    protected virtual void OnComponentInitialized() {}
    protected abstract Task OnComponentInitializedAsync();
    protected virtual void OnComponentParametersSet() {}
    protected virtual Task OnComponentParametersSetAsync() => Task.CompletedTask;
    protected virtual void OnComponentAfterRender(bool firstRender) {}
    protected virtual Task OnComponentAfterRenderAsync(bool firstRender) => Task.CompletedTask;
    protected virtual void OnComponentDispose() {}
    protected virtual ValueTask OnComponentDisposeAsync() => ValueTask.CompletedTask;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public SSRComponent() {
        Page.Current.CountComponent();
        Key = $"{KEY_PREFIX}-{Page.Current.ComponentCount}";
        State =  COMPONENT_STATE.DONE;
        Initializing = false;
        Data = EmptyData();
        Exception = null;
    }

    protected override sealed void OnInitialized() => OnComponentInitialized();
    protected override sealed async Task OnInitializedAsync() {
        PersistingSubscription = ApplicationState.RegisterOnPersisting(PersistData);

        if (ApplicationState.TryTakeFromJson<SSRState<T>>(Key, out var restored)) {
            State = restored!.State;
            Data = restored!.Data;
        } else {
            Initializing = true;
            await OnComponentInitializedAsync();
            Initializing = false;
        }
    }
    protected override sealed void OnParametersSet(bool fristTime) => OnComponentParametersSet();
    protected override sealed async Task OnParametersSetAsync(bool firstTime) => await OnComponentParametersSetAsync();
    protected override sealed void OnAfterRender(bool firstRender) => OnComponentAfterRender(firstRender);
    protected override sealed async Task OnAfterRenderAsync(bool firstRender) => await OnComponentAfterRenderAsync(firstRender);
    public async ValueTask DisposeAsync() {
        OnComponentDispose();
        await OnComponentDisposeAsync();
        PersistingSubscription.Dispose();
        GC.SuppressFinalize(this);
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private Task PersistData() {
        ApplicationState.PersistAsJson(Key, new { State, Data });
        return Task.CompletedTask;
    }
}

namespace Jumpeno.Client.Base;

public abstract class SSRComponent<T> : Component {
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

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public SSRComponent() {
        Page.Current.CountComponent();
        Key = $"{KEY_PREFIX}-{Page.Current.ComponentCount}";
        State =  COMPONENT_STATE.DONE;
        Initializing = false;
        Data = EmptyData();
        Exception = null;
    }
    protected override sealed void OnInitialized() => base.OnInitialized();
    protected override sealed async Task OnInitializedAsync() {
        PersistingSubscription = ApplicationState.RegisterOnPersisting(PersistData);

        if (ApplicationState.TryTakeFromJson<SSRState<T>>(Key, out var restored)) {
            State = restored!.State;
            Data = restored!.Data;
        } else {
            Initializing = true;
            await base.OnInitializedAsync();
            Initializing = false;
        }
    }
    protected override sealed void OnParametersSet() => base.OnParametersSet();
    protected override sealed async Task OnParametersSetAsync() => await base.OnParametersSetAsync();
    protected override sealed void OnAfterRender(bool firstRender) => base.OnAfterRender(firstRender);
    protected override sealed async Task OnAfterRenderAsync(bool firstRender) => await base.OnAfterRenderAsync(firstRender);
    public override sealed async ValueTask DisposeAsync() {
        PersistingSubscription.Dispose();
        await base.DisposeAsync();
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private Task PersistData() {
        ApplicationState.PersistAsJson(Key, new { State, Data });
        return Task.CompletedTask;
    }
}

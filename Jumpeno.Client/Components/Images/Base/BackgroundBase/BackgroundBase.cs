namespace Jumpeno.Client.Components;

public partial class BackgroundBase {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID_PREFIX = "background";
    // Class:
    public const string CLASS = "background";
    public const string CLASS_IMG = "background-loader-image";
    public const string CLASS_ELEMENT = "background-element";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required string URL { get; set; }
    [Parameter]
    public bool Transparent { get; set; } = false;
    [Parameter]
    public bool NoTransition { get; set; } = false;
    [Parameter]
    public bool Preloaded { get; set; } = false;
    [Parameter]
    public Action<bool> OnLoadingFinish { get; set; } = success => {};

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private string ID { get; set; }
    private IMAGE_STATE State { get; set; } = IMAGE_STATE.LOADING;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(CLASS, Base)
        .Set(State)
        .Set(ImageBase.CLASS_TRANSPARENT, Transparent)
        .Set(ImageBase.CLASS_NO_TRANSITION, NoTransition);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public BackgroundBase() => ID = IDGenerator.Generate(ID_PREFIX);

    protected override void OnComponentParametersSet(bool firstTime) {
        if (!firstTime) return;
        if (AppEnvironment.IsServer) {
            State = IMAGE_STATE.LOADING;
        } else {
            State = Preloaded
                    ? (IMAGE_STATE) JS.Invoke<int>(JSImage.CheckPreloadedState, ImagePreloader.ID, URL)
                    : (IMAGE_STATE) JS.Invoke<int>(JSImage.CheckState, URL);
            ImageBase.HandleLoadFinish(State, OnLoadingFinish);
        } 
    }

    // Events -----------------------------------------------------------------------------------------------------------------------------
    private void OnImageLoaded(bool success) {
        if (success) State = IMAGE_STATE.FINISHED;
        else State = IMAGE_STATE.ERROR;
        StateHasChanged();
        ImageBase.HandleLoadFinish(State, OnLoadingFinish);
    }
}

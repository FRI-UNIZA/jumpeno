namespace Jumpeno.Client.Components;

public partial class BackgroundImageBase {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID_PREFIX = "background";
    public const string CLASSNAME = "background-component";
    public const string CLASSNAME_IMG = "background-loader-image";
    public const string CLASSNAME_ELEMENT = "background-element";

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
    public Action<bool> OnLoadingFinish { get; set; } = (bool success) => {};

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private string ID { get; set; }
    private IMAGE_STATE State { get; set; } = IMAGE_STATE.LOADING;
    protected CSSClass ComputeClass() {
        var c = ComputeClass(CLASSNAME);
        if (State == IMAGE_STATE.LOADING) c.Set(ImageBase.CLASSNAME_LOADING);
        if (State == IMAGE_STATE.ERROR) c.Set(ImageBase.CLASSNAME_ERROR);
        if (Transparent) c.Set(ImageBase.CLASSNAME_TRANSPARENT);
        if (NoTransition) c.Set(ImageBase.CLASSNAME_NO_TRANSITION);
        return c;
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public BackgroundImageBase() => ID = IDGenerator.Generate(ID_PREFIX);

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

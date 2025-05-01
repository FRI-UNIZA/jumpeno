namespace Jumpeno.Client.Components;

/// <summary>
/// Usage:
/// To set dimensions and colors redefine css variables in custom css class passed as component parameter.
/// (Custom css class must be defined with ::deep selector)
/// Other css properties (e.g. border-radius...) style as you wish.
/// Modify component parameters to controll transparency, image transition and loading.
/// </summary>
public partial class ImageBase {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID_PREFIX = "image";
    public const string CLASSNAME = "image-component";
    public const string CLASSNAME_LOADING = "loading";
    public const string CLASSNAME_ERROR = "error";
    public const string CLASSNAME_TRANSPARENT = "transparent";
    public const string CLASSNAME_NO_TRANSITION = "no-transition";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required string URL { get; set; }
    [Parameter]
    public string Alt { get; set; } = "";
    [Parameter]
    public bool Draggable { get; set; } = true;
    [Parameter]
    public bool Transparent { get; set; } = false;
    [Parameter]
    public bool NoTransition { get; set; } = false;
    [Parameter]
    public bool Preloaded { get; set; } = false;
    [Parameter]
    public IMAGE_LOADING Loading { get; set; } = IMAGE_LOADING.LAZY;
    [Parameter]
    public Action<bool> OnLoadingFinish { get; set; } = (bool success) => {};
    private readonly Dictionary<string, object> AdditionalAttributes = [];
    
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly string ID = null!;
    private IMAGE_STATE State = IMAGE_STATE.LOADING;
    protected CSSClass ComputeClass() {
        var c = ComputeClass(CLASSNAME);
        if (State == IMAGE_STATE.LOADING) c.Set(CLASSNAME_LOADING);
        else if (State == IMAGE_STATE.ERROR) c.Set(CLASSNAME_ERROR);
        if (Transparent) c.Set(CLASSNAME_TRANSPARENT);
        if (NoTransition) c.Set(CLASSNAME_NO_TRANSITION);
        return c;
    }
    
    private static readonly Dictionary<string, ImageBase> Images = [];

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public ImageBase() {
        if (AppEnvironment.IsServer) return;
        ID = IDGenerator.Generate(ID_PREFIX);
        Images[ID] = this;
    }

    protected override void OnComponentParametersSet(bool firstTime) {
        if (!firstTime) return;
        var alt = Alt.Trim();
        AdditionalAttributes["alt"] = alt;
        if (alt == "") AdditionalAttributes["aria-hidden"] = "true";
        if (AppEnvironment.IsServer) {
            State = IMAGE_STATE.LOADING;
        } else {
            State = Preloaded
                    ? (IMAGE_STATE) JS.Invoke<int>(JSImage.CheckPreloadedState, ImagePreloader.ID, URL)
                    : (IMAGE_STATE) JS.Invoke<int>(JSImage.CheckState, URL);
            HandleLoadFinish(State, OnLoadingFinish);
        } 
    }

    override protected void OnComponentAfterRender(bool firstRender) {
        if (AppEnvironment.IsServer || !firstRender) return;
        JS.InvokeVoid(JSImage.Init, ID);
    }

    protected override void OnComponentDispose() {
        if (AppEnvironment.IsServer) return;
        Images.Remove(ID);
    }
    
    // Events -----------------------------------------------------------------------------------------------------------------------------
    public static void HandleLoadFinish(IMAGE_STATE state, Action<bool> OnLoadingFinish) {
        switch (state) {
            case IMAGE_STATE.ERROR:
                OnLoadingFinish(false);
            break;
            case IMAGE_STATE.FINISHED:
                OnLoadingFinish(true);
            break;
        }
    }

    // JS Interop -------------------------------------------------------------------------------------------------------------------------
    [JSInvokable]
    public static void JS_OnLoad(string id) {
        try {
            var image = Images[id];
            image.State = IMAGE_STATE.FINISHED;
            image.StateHasChanged();
            HandleLoadFinish(image.State, image.OnLoadingFinish);
        } catch {}
    }

    [JSInvokable]
    public static void JS_OnError(string id) {
        try {
            var image = Images[id];
            image.State = IMAGE_STATE.ERROR;
            image.StateHasChanged();
            HandleLoadFinish(image.State, image.OnLoadingFinish);
        } catch {}
    }
}

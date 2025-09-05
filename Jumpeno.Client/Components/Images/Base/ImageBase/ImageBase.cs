namespace Jumpeno.Client.Components;

/// <summary>
/// Usage:
/// To set dimensions and colors redefine css variables in custom css class passed as component parameter.
/// Other css properties (e.g. border-radius...) style as you wish.
/// Modify component parameters to controll transparency, image transition and loading.
/// </summary>
public partial class ImageBase {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID_PREFIX = "image";
    public const string CLASSNAME = "image";
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
    public Action<bool> OnLoadingFinish { get; set; } = success => {};
    private readonly Dictionary<string, object> Attributes = [];
    
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly string ID = null!;
    private IMAGE_STATE State = IMAGE_STATE.LOADING;
    
    private static readonly Dictionary<string, ImageBase> Images = [];

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(CLASSNAME, Base)
        .Set(State)
        .Set(CLASSNAME_TRANSPARENT, Transparent)
        .Set(CLASSNAME_NO_TRANSITION, NoTransition);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public ImageBase() {
        if (AppEnvironment.IsServer) return;
        ID = IDGenerator.Generate(ID_PREFIX);
        Images[ID] = this;
    }

    protected override void OnComponentParametersSet(bool firstTime) {
        if (!firstTime) return;
        var alt = Alt.Trim();
        Attributes["alt"] = alt;
        if (alt == "") Attributes["aria-hidden"] = "true";
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
        if (!firstRender) return;
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

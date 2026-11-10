namespace Jumpeno.Client.Components;

/// <summary>
/// Usage:
/// To set dimensions and colors redefine css variables in custom css class passed as component parameter.
/// Other css properties (e.g. border-radius...) style as you wish.
/// Modify component parameters to controll transparency, image transition and loading.
/// </summary>
public partial class ImageBase {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string IdPrefix = "image";
    // Class:
    public const string ClassName = "image";
    public const string ClassTransparent = "transparent";
    public const string ClassNoTransition = "no-transition";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required string Url { get; set; }
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
    public ImageLoadingType Loading { get; set; } = ImageLoadingType.Lazy;
    [Parameter]
    public Action<bool> OnLoadingFinish { get; set; } = success => {};
    private readonly Dictionary<string, object> Attributes = [];
    
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly string _id = null!;
    private ImageState State = ImageState.Loading;
    
    private static readonly Dictionary<string, ImageBase> Images = [];

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() {
        return base.ComputeClass()
        .Set(ClassName, Base)
        .Set(State)
        .Set(ClassTransparent, Transparent)
        .Set(ClassNoTransition, NoTransition);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public ImageBase() {
        if (AppEnvironment.IsServer) return;
        _id = IDGenerator.Generate(IdPrefix);
        Images[_id] = this;
    }

    protected override void OnComponentParametersSet(bool firstTime) {
        if (!firstTime) return;
        var alt = Alt.Trim();
        Attributes["alt"] = alt;
        if (alt == "") Attributes["aria-hidden"] = "true";
        if (AppEnvironment.IsServer) {
            State = ImageState.Loading;
        } else {
            State = Preloaded
                    ? (ImageState) JS.Invoke<int>(JSImage.CheckPreloadedState, ImagePreloader.ID, Url)
                    : (ImageState) JS.Invoke<int>(JSImage.CheckState, Url);
            HandleLoadFinish(State, OnLoadingFinish);
        } 
    }

    override protected void OnComponentAfterRender(bool firstRender) {
        if (!firstRender) return;
        JS.InvokeVoid(JSImage.Init, _id);
    }

    protected override void OnComponentDispose() {
        if (AppEnvironment.IsServer) return;
        Images.Remove(_id);
    }
    
    // Events -----------------------------------------------------------------------------------------------------------------------------
    public static void HandleLoadFinish(ImageState state, Action<bool> onLoadingFinish) {
        switch (state) {
            case ImageState.Error:
                onLoadingFinish(false);
            break;
            case ImageState.Finished:
                onLoadingFinish(true);
            break;
        }
    }

    // JS Interop -------------------------------------------------------------------------------------------------------------------------
    [JSInvokable]
    public static void JS_OnLoad(string id) {
        try {
            var image = Images[id];
            image.State = ImageState.Finished;
            image.StateHasChanged();
            HandleLoadFinish(image.State, image.OnLoadingFinish);
        } catch {}
    }

    [JSInvokable]
    public static void JS_OnError(string id) {
        try {
            var image = Images[id];
            image.State = ImageState.Error;
            image.StateHasChanged();
            HandleLoadFinish(image.State, image.OnLoadingFinish);
        } catch {}
    }
}

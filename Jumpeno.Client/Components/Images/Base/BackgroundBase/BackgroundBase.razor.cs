namespace Jumpeno.Client.Components;

public partial class BackgroundBase {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string IdPrefix = "background";
    // Class:
    public const string ClassName = "background";
    public const string ClassImg = "background-loader-image";
    public const string ClassElement = "background-element";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required string Url { get; set; }
    [Parameter]
    public bool Transparent { get; set; } = false;
    [Parameter]
    public bool NoTransition { get; set; } = false;
    [Parameter]
    public bool Preloaded { get; set; } = false;
    [Parameter]
    public Action<bool> OnLoadingFinish { get; set; } = success => {};

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private string Id { get; set; }
    private ImageState State { get; set; } = ImageState.Loading;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() {
        return base.ComputeClass()
        .Set(ClassName, Base)
        .Set(State)
        .Set(ImageBase.ClassTransparent, Transparent)
        .Set(ImageBase.ClassNoTransition, NoTransition);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public BackgroundBase() => Id = IDGenerator.Generate(IdPrefix);

    protected override void OnComponentParametersSet(bool firstTime) {
        if (!firstTime) return;
        if (AppEnvironment.IsServer) {
            State = ImageState.Loading;
        } else {
            State = Preloaded
                    ? (ImageState) JS.Invoke<int>(JSImage.CheckPreloadedState, ImagePreloader.ID, Url)
                    : (ImageState) JS.Invoke<int>(JSImage.CheckState, Url);
            ImageBase.HandleLoadFinish(State, OnLoadingFinish);
        } 
    }

    // Events -----------------------------------------------------------------------------------------------------------------------------
    private void OnImageLoaded(bool success) {
        if (success) State = ImageState.Finished;
        else State = ImageState.Error;
        StateHasChanged();
        ImageBase.HandleLoadFinish(State, OnLoadingFinish);
    }
}

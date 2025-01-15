namespace Jumpeno.Client.Components;

public partial class Image {
    [Parameter]
    public required string URL { get; set; }
    [Parameter]
    public string Alt { get; set; } = "";
    [Parameter]
    public string Class { get; set; } = "";
    [Parameter]
    public string Style { get; set; } = "";
    [Parameter]
    public bool NotDraggable { get; set; } = false;
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
}

namespace Jumpeno.Client.Components;

public partial class TextComponent: SurfaceComponent {
    [Parameter]
    public TEXT_VARIANT Variant { get; set; } = TEXT_VARIANT.PRIMARY;
    [Parameter]
    public TEXT_SIZE Size { get; set; } = TEXT_SIZE.M;
    [Parameter]
    public bool NoWrap { get; set; } = false;
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}

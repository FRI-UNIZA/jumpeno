namespace Jumpeno.Client.Components;

public abstract partial class GameCanvasComponent {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "game-canvas";

    // Identifiers ------------------------------------------------------------------------------------------------------------------------
    public override sealed string Selector => $"#{ID} canvas";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    /// <summary>Use for game UI or notifications</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    // Map --------------------------------------------------------------------------------------------------------------------------------
    protected abstract Map CurrentMap();

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);

    public virtual CSSStyle ComputeStyle() {
        var map = CurrentMap(); return new CSSStyle()
        .Set("--canvas-background", map.Background)
        .Set("--canvas-foreground", map.Foreground)
        .Set("--canvas-border-color", map.Border)
        .Set("--canvas-box-shadow-color", map.BoxShadow.ToStringContent());
    }
}

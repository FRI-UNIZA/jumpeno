namespace Jumpeno.Client.Components;

public abstract partial class GameCanvasComponent {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "game-canvas";

    // Identifiers ------------------------------------------------------------------------------------------------------------------------
    public sealed override string Selector => $"#{ID} canvas";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    /// <summary>Use for game UI or notifications</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    // Map --------------------------------------------------------------------------------------------------------------------------------
    protected abstract Map CurrentMap();

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);

    public virtual CSSStyle ComputeStyle() {
        var map = CurrentMap(); return new CSSStyle()
        .Set("--canvas-background", map.Background)
        .Set("--canvas-foreground", map.Foreground)
        .Set("--canvas-border-color", map.Border)
        .Set("--canvas-box-shadow-color", map.BoxShadow.ToStringContent());
    }
}

namespace Jumpeno.Client.Utils;

public class GameScreenDimension() {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    // Computed screen ratio:
    public const double SCREEN_WIDTH_RATIO = CANVAS_WIDTH_RATIO + 2 * SCREEN_MARGIN_RATIO;
    public const double SCREEN_HEIGHT_RATIO = CANVAS_HEIGHT_RATIO + INFO_HEIGHT_RATIO + CONTROLS_HEIGHT_RATIO + 2 * SCREEN_MARGIN_RATIO;
    
    // Component ratios:
    public const double CANVAS_WIDTH_RATIO = 16;
    public const double CANVAS_HEIGHT_RATIO = 9;
    public const double INFO_HEIGHT_RATIO = 1;
    public const double CONTROLS_HEIGHT_RATIO = 2;
    public const double SCREEN_MARGIN_RATIO = 1;

    // Min dimensions (px):
    public const int MIN_CANVAS_BORDER_WIDTH = 2;
    public const int MIN_INFO_PADDING_TOP = 10;
    public const int MIN_INFO_TEXT_SIZE = 14;
    public const int MIN_TIME_WIDTH = 100;
    public const int MIN_CONTROLS_PADDING_TOP = 20;
    public const int MIN_CONTROL_HEIGHT = 50;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public int CanvasWidth { get; private set; } = 0;
    public int CanvasHeight { get; private set; } = 0;
    public int InfoHeight => (int) (CanvasHeight / CANVAS_HEIGHT_RATIO * INFO_HEIGHT_RATIO);
    public int ControlsHeight => (int) (CanvasHeight / CANVAS_HEIGHT_RATIO * CONTROLS_HEIGHT_RATIO);

    // Helpers ----------------------------------------------------------------------------------------------------------------------------
    private static int Padding(int screenWidth, int screenHeight) {
        var ratio = SCREEN_WIDTH_RATIO / SCREEN_HEIGHT_RATIO;
        return (screenWidth / (double) screenHeight < ratio)
        ? (int) (screenWidth / SCREEN_WIDTH_RATIO * SCREEN_MARGIN_RATIO)
        : (int) (screenHeight / SCREEN_HEIGHT_RATIO * SCREEN_MARGIN_RATIO);
    }

    // Update -----------------------------------------------------------------------------------------------------------------------------
    public void Update(int screenWidth, int screenHeight) {
        // 1) Compute space:
        var padding = Padding(screenWidth, screenHeight);
        var spaceWidth = Math.Max(screenWidth - 2 * padding, 1);
        var spaceHeight = Math.Max(screenHeight - 2 * padding, 1);

        // 2) Compute canvas dimensions:
        var heightRatio = CANVAS_HEIGHT_RATIO + INFO_HEIGHT_RATIO + CONTROLS_HEIGHT_RATIO;
        if (spaceWidth / (double) spaceHeight < CANVAS_WIDTH_RATIO / heightRatio) CanvasHeight = (int) (spaceWidth / CANVAS_WIDTH_RATIO * CANVAS_HEIGHT_RATIO);
        else CanvasHeight = (int) (spaceHeight / heightRatio * CANVAS_HEIGHT_RATIO);
        CanvasWidth = (int) (CanvasHeight / CANVAS_HEIGHT_RATIO * CANVAS_WIDTH_RATIO);
    }

    // CSS --------------------------------------------------------------------------------------------------------------------------------
    public CSSStyle CSSVariables() {
        var s = new CSSStyle();
        // Constants:
        s.Set("--min-canvas-border-width", $"{MIN_CANVAS_BORDER_WIDTH}");
        s.Set("--min-info-padding-top", $"{MIN_INFO_PADDING_TOP}");
        s.Set("--min-info-text-size", $"{MIN_INFO_TEXT_SIZE}");
        s.Set("--min-time-width", $"{MIN_TIME_WIDTH}");
        s.Set("--min-controls-padding-top", $"{MIN_CONTROLS_PADDING_TOP}");
        s.Set("--min-control-height", $"{MIN_CONTROL_HEIGHT}");
        // Attributes:
        s.Set("--canvas-width", $"{CanvasWidth}");
        s.Set("--canvas-height", $"{CanvasHeight}");
        s.Set("--info-height", $"{InfoHeight}");
        s.Set("--controls-height", $"{ControlsHeight}");
        return s;
    }
}

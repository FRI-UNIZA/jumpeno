namespace Jumpeno.Client.Components;

/* NOTE: A component to display log messages on devices that are difficult to debug. */
public partial class ConsoleUI {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID = "console-ui";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public int? Left { get; set; } = null;
    [Parameter]
    public int? Top { get; set; } = null;
    [Parameter]
    public int? Right { get; set; } = null;
    [Parameter]
    public int? Bottom { get; set; } = null;
    [Parameter]
    public int? Width { get; set; } = null;
    [Parameter]
    public int? Height { get; set; } = null;
    [Parameter]
    public bool NoEvents { get; set; } = false;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static ConsoleUI? Instance { get; set; }
    private static string Content = "";
    private string ComputeStyle() {
        CSSStyle s = new();
        if (Left != null) s.Set("--console-left", $"{Left}px");
        if (Top != null) s.Set("--console-top", $"{Top}px");
        if (Right != null) s.Set("--console-right", $"{Right}px");
        if (Bottom != null) s.Set("--console-bottom", $"{Bottom}px");
        if (Width != null) s.Set("--console-width", $"{Width}px");
        if (Height != null) s.Set("--console-height", $"{Height}px");
        s.Set("--console-events", NoEvents ? "none" : "all");
        return s;
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public ConsoleUI() => Instance ??= this;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    [JSInvokable]
    public static void Write(string text) { Content = $"{text}{Content}"; Instance?.StateHasChanged(); }
    [JSInvokable]
    public static void WriteLine(string text) => Write($"{text}\n");
    [JSInvokable]
    public static void Clear() { Content = ""; Instance?.StateHasChanged(); }
}

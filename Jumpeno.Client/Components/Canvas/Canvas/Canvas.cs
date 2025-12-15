namespace Jumpeno.Client.Components;

public abstract class Canvas : BasicComponent {
    // Identifiers ------------------------------------------------------------------------------------------------------------------------
    public readonly string ID = IDGenerator.Generate(nameof(Canvas));

    /// <summary>Selector of canvas element.</summary>
    public abstract string Selector { get; }

    /// <summary>Defines whether canvas is dynamic (must be rendered explicitly) or static (rendered automatically).</summary>
    public abstract bool Dynamic { get; }

    // NOTE: Tab switch fix: Canvas is refreshed on visibility change by this rate:
    public virtual int REFRESH_INIT_DELAY => 1000; // ms
    public virtual int REFRESH_TIMES => 2;
    public virtual int REFRESH_INTERVAL => 1000; // ms

    // Views ------------------------------------------------------------------------------------------------------------------------------
    private readonly DotNetObjectReference<Canvas> Ref;
    protected BECanvasComponent CanvasRef = null!;
    protected Canvas2DContext? ctx = null;

    // Visibility -------------------------------------------------------------------------------------------------------------------------
    [JSInvokable]
    public async Task JS_OnVisibilityChange(WindowVisibilityEvent e) {
        if (e.Hidden) return;
        var counter = 0;
        do {
            await Task.Delay(REFRESH_INIT_DELAY);
            await RenderLock.TryExclusive(async () => {
                await PreRenderCanvas();
                if (!Dynamic) await RenderCanvas();
            });
            await Task.Delay(REFRESH_INTERVAL);
            counter += REFRESH_INTERVAL;
        } while (counter < REFRESH_TIMES * REFRESH_INTERVAL);
    }

    // Dimensions -------------------------------------------------------------------------------------------------------------------------
    public double DPR { get; protected set; }
    public int Width { get; protected set; }
    public int Height { get; protected set; }

    protected Task UpdateDimensions() => RenderLock.TryExclusive(() => {
        var size = Window.GetSizeOf(Selector);
        if (size == null) return;
        DPR = JS.Eval<double>("window.devicePixelRatio || 1");
        Width = (int)(size.Width * DPR);
        Height = (int)(size.Height * DPR);
    });

    [JSInvokable]
    public Task JS_OnWindowResize(WindowResizeEvent e) => UpdateDimensions();

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public Canvas() => Ref = DotNetObjectReference.Create(this);

    protected override async Task OnComponentAfterRenderAsync(bool firstRender) {
        try {
            if (!firstRender) return;
            await Window.AddVisibilityChangeEventListener(Ref, JS_OnVisibilityChange);
            await Window.AddResizeEventListener(Ref, JS_OnWindowResize);
            await UpdateDimensions();
        } finally {
            if (!Dynamic) await Render();
        }
    }

    protected override async ValueTask OnComponentDisposeAsync() {
        if (AppEnvironment.IsClient) {
            await Window.RemoveVisibilityChangeEventListener(Ref, JS_OnVisibilityChange);
            await Window.RemoveResizeEventListener(Ref, JS_OnWindowResize);
        }
        await RenderLock.DisposeSafe();
        ctx?.Dispose();
        Ref.Dispose();
    }

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    private readonly LockerSlim RenderLock = new();
    private bool FirstRender = false;

    public Task PreRender() => RenderLock.TryExclusive(PreRenderCanvas);
    public Task Render() => RenderLock.TryExclusive(async () => {
        if (!FirstRender) {
            await PreRenderCanvas();
            FirstRender = true;
        }
        await RenderCanvas();
    });

    protected abstract Task PreRenderCanvas();
    protected abstract Task RenderCanvas();
}

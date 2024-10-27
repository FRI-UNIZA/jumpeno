namespace Jumpeno.Client.Components;

using AntDesign;

/// <summary>
/// Usage:
/// Use class property to set width, height, max-width or max-height and background of scrollable area.
/// This allows to use media or container queries.
/// </summary>
public partial class ScrollArea: IDisposable {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    private const string MARK = "scroll-area";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    // Each Change of Theme parameter is applied in css (null means current theme):
    [Parameter]
    public SCROLLAREA_THEME? Theme { get; set; }
    // Next parameters can only be set once:
    [Parameter]
    public string ID { get; set; }
    [Parameter]
    public SCROLLAREA_AUTOHIDE AutoHide { get; set; }
    [Parameter]
    public bool OverflowX { get; set; }
    [Parameter]
    public bool OverflowY { get; set; }
    // Class and content:
    [Parameter]
    public string Class { get; set; }
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static readonly Dictionary<string, ScrollArea> Areas = [];
    private Action<ScrollAreaPosition>[] Listeners = [];
    private static readonly Dictionary<string, Action<ScrollAreaPosition>[]> RegisterListeners = [];

    private string CSSClass() {
        return new CSSClass($"{MARK} {Class}".Trim());
    }
    private static string CSSContentClass() {
        return $"{MARK}-content";
    }
    private static string GetThemeString(SCROLLAREA_THEME theme) {
        return theme.ToString()!.ToLower().Replace("_", "-");
    }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public ScrollArea() {
        Theme = null;
        AutoHide = SCROLLAREA_AUTOHIDE.NEVER;
        OverflowX = true;
        OverflowY = true;
        Class = "";
        ChildContent = null;
        ID = ComponentService.GenerateID(MARK);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    override protected void OnAfterRender(bool firstRender) {
        if (firstRender) {
            Areas.Add(ID, this);
            JS.InvokeVoid(
                JSScrollArea.Activate, ID,
                Theme is null ? null : GetThemeString((SCROLLAREA_THEME) Theme), AutoHide.ToString().ToLower(), OverflowX, OverflowY
            );
            RegisterAreaListeners(ID);
        } else {
            if (Theme is null) return;
            JS.InvokeVoid(JSScrollArea.Update, ID, GetThemeString((SCROLLAREA_THEME) Theme));
        }
    }

    public void Dispose() {
        if (AppEnvironment.IsServer()) return;
        if (Listeners.Length > 0) {
            JS.InvokeVoid(JSScrollArea.RemoveScrollListener, ID);
            Listeners = [];
        }
        JS.InvokeVoid(JSScrollArea.Destroy, ID);
        Areas.Remove(ID);
        GC.SuppressFinalize(this);
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private static ScrollArea? GetArea(string id) {
        Areas.TryGetValue(id, out var area);
        return area;
    }

    private static Action<ScrollAreaPosition>[]? GetRegisterListeners(string id) {
        RegisterListeners.TryGetValue(id, out var reg);
        return reg;
    } 

    private static void RegisterAreaListeners(string id) {
        var area = GetArea(id);
        if (area is null) return;
        var reg = GetRegisterListeners(id);
        if (reg is not null) {
            foreach (var listener in reg) {
                area.AddScrollListener(listener);
            }
            RegisterListeners.Remove(id); 
        }
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static void SetTheme(SCROLLAREA_THEME theme) {
        if (AppEnvironment.IsServer()) return;
        JS.InvokeVoid(JSScrollArea.SetTheme, GetThemeString(theme));
    }

    public static void HideScrollbars(string id) {
        if (AppEnvironment.IsServer()) return;
        JS.InvokeVoid(JSScrollArea.HideScrollbars, id);
    }
    public void HideScrollbars() { HideScrollbars(ID); }

    public static void ShowScrollbars(string id) {
        if (AppEnvironment.IsServer()) return;
        JS.InvokeVoid(JSScrollArea.ShowScrollbars, id);
    }
    public void ShowScrollbars() { ShowScrollbars(ID); }

    public static void ScrollTo(string id, double left, double top) {
        if (AppEnvironment.IsServer()) return;
        JS.InvokeVoid(JSScrollArea.Scroll, id, left, top);
    }
    public void ScrollTo(double left, double top) { ScrollTo(ID, left, top); }

    public static ScrollAreaPosition Position(string id) {
        if (AppEnvironment.IsServer()) throw new Exception("Can't use on the server!");
        return JS.Invoke<ScrollAreaPosition>(JSScrollArea.Position, id);
    }
    public ScrollAreaPosition Position() { return Position(ID); }

    public static ScrollAreaItemPosition ItemPosition(string id, string selector) {
        if (AppEnvironment.IsServer()) throw new Exception("Can't use on the server!");
        return JS.Invoke<ScrollAreaItemPosition>(JSScrollArea.ItemPosition, id, selector);
    }
    public ScrollAreaItemPosition ItemPosition(string selector) { return ItemPosition(ID, selector); }

    // Listeners --------------------------------------------------------------------------------------------------------------------------
    public static void AddScrollListener(string id, Action<ScrollAreaPosition> listener) {
        if (AppEnvironment.IsServer()) return;
        var area = GetArea(id);
        if (area is not null) {
            area.Listeners = [.. area.Listeners, listener];
            JS.InvokeVoid(JSScrollArea.AddScrollListener, id);
        } else {
            var reg = GetRegisterListeners(id);
            if (reg is null) reg = [];
            RegisterListeners[id] = [.. reg, listener];
        }
    }
    public void AddScrollListener(Action<ScrollAreaPosition> listener) { AddScrollListener(ID, listener); }

    public static void RemoveScrollListener(string id, Action<ScrollAreaPosition> listener) {
        if (AppEnvironment.IsServer()) return;
        var area = GetArea(id);
        if (area is not null) {
            area.Listeners = area.Listeners.Remove(listener);
            if (area.Listeners.Length <= 0) {
                JS.InvokeVoid(JSScrollArea.RemoveScrollListener, id);
                area.Listeners = [];
            }
        } else {
            var reg = GetRegisterListeners(id);
            if (reg is null) return;
            else RegisterListeners[id] = reg.Remove(listener);
        }
    }
    public void RemoveScrollListener(Action<ScrollAreaPosition> listener) { RemoveScrollListener(ID, listener); }

    // JS Interop -------------------------------------------------------------------------------------------------------------------------
    [JSInvokable]
    public static void JS_OnScroll(string id, ScrollAreaPosition position) {
        var area = GetArea(id);
        if (area is null) return;
        foreach (var listener in area.Listeners) {
            listener(position);
        }
    }
}

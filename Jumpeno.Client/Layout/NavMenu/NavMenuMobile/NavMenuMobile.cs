namespace Jumpeno.Client.Components;

public partial class NavMenuMobile : IAsyncDisposable {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "nav-menu-mobile";
    public const string CLASS_SELECTOR = $".{CLASS}";
    public const double MOBILE_MENU_BREAKPOINT = 1200;

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [CascadingParameter]
    public required BaseTheme Theme { get; set; }
    [Parameter]
    public required NavMenu MenuRef { get; set; }
    [Parameter]
    public EventCallback OnMobileMenuOpen { get; set; } = EventCallback.Empty;
    [Parameter]
    public EventCallback OnMobileMenuClose { get; set; } = EventCallback.Empty;
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public string ID { get; private set; }
    private DotNetObjectReference<NavMenuMobile> ObjRef;
    private ScrollArea ScrollAreaRef = null!;
    private MENU_STATE State { get; set; } = MENU_STATE.CLOSED;
    private readonly SemaphoreSlim Lock = new(1, 1);
    private TaskCompletionSource StateTCS { get; set; } = null!;
    private string ComputeClass() {
        var c = new CSSClass(CLASS);
        switch (State) {
            case MENU_STATE.CLOSED:
                c.Set("closed");
                break;
            case MENU_STATE.OPENING:
                c.Set("opening");
                break;
            case MENU_STATE.OPENED:
                c.Set("opened");
                break;
            case MENU_STATE.CLOSING:
                c.Set("closing");
                break;
        }
        return c;
    }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public NavMenuMobile() {
        ID = ComponentService.GenerateID(CLASS);
        ObjRef = DotNetObjectReference.Create(this);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (firstRender) {
            await Window.AddResizeEventListener(ObjRef, JS_OnWindowResize);
            if (Page.CurrentPage().GetType() != typeof(ErrorPage)) {
                await Navigator.AddAfterFinishEventListener(CloseAfter);
            }
        } else {
            if (State == MENU_STATE.OPENED || State == MENU_STATE.CLOSED) {
                StateTCS?.TrySetResult();
            } 
        }
    }
    public async ValueTask DisposeAsync() {
        if (!AppEnvironment.IsServer) {
            await Navigator.RemoveAfterFinishEventListener(CloseAfter);
            await Window.RemoveResizeEventListener(ObjRef, JS_OnWindowResize);
        }
        ObjRef.Dispose();
    }

    // Listeners --------------------------------------------------------------------------------------------------------------------------
    private async Task CloseAfter(NavigationEvent e) {
        await Close();
    }

    private async Task OnKeyDown(KeyboardEventArgs e) {
        if (e.Key != KEYBOARD.ESC) return;
        await Close();
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task Open() {
        await Lock.WaitAsync();
        if (State != MENU_STATE.CLOSED) {
            Lock.Release();
            return;
        }

        await PageLoader.Show(PAGE_LOADER_TASK.MENU, true);

        var objRef = DotNetObjectReference.Create(this);
        AnimationHandler.CallOnAnimationEnd(CLASS_SELECTOR, objRef, nameof(JS_OnAnimationEnd));
        State = MENU_STATE.OPENING;
        StateTCS = new TaskCompletionSource();
        StateHasChanged();
        ScrollAreaRef.ScrollTo(0, 0);
        await StateTCS.Task;

        State = MENU_STATE.OPENED;
        StateTCS = new TaskCompletionSource();
        StateHasChanged();
        await StateTCS.Task;

        objRef.Dispose();
        await PageLoader.Hide(PAGE_LOADER_TASK.MENU);

        ActionHandler.SetFocus(ID);

        await OnMobileMenuOpen.InvokeAsync();
        Lock.Release();
    }
    public async Task Close() {
        await Lock.WaitAsync();
        if (State != MENU_STATE.OPENED) {
            Lock.Release();
            return;
        }
        await PageLoader.Show(PAGE_LOADER_TASK.MENU, true);

        var objRef = DotNetObjectReference.Create(this);
        AnimationHandler.CallOnAnimationEnd(CLASS_SELECTOR, objRef, nameof(JS_OnAnimationEnd));
        State = MENU_STATE.CLOSING;
        StateTCS = new TaskCompletionSource();
        StateHasChanged();
        await StateTCS.Task;

        State = MENU_STATE.CLOSED;
        StateTCS = new TaskCompletionSource();
        StateHasChanged();
        await StateTCS.Task;

        objRef.Dispose();
        await PageLoader.Hide(PAGE_LOADER_TASK.MENU);
    
        var windowSize = Window.GetSize();
        if (windowSize.Width < MOBILE_MENU_BREAKPOINT) {
            ActionHandler.SetFocus(NavMenu.MOBILE_MENU_BUTTON_ID);
        } else {
            ActionHandler.SetFocus(MenuControls.FIRST_LINK_ID);
        }

        await OnMobileMenuClose.InvokeAsync();
        Lock.Release();
    }

    // JS Interop -------------------------------------------------------------------------------------------------------------------------
    [JSInvokable]
    public void JS_OnAnimationEnd() {
        StateTCS.TrySetResult();        
    }

    [JSInvokable]
    public async Task JS_OnWindowResize(WindowResizeEvent e) {
        // Change to desktop:
        if (e.WidthPrevious < MOBILE_MENU_BREAKPOINT && MOBILE_MENU_BREAKPOINT <= e.Width) {
            if (State == MENU_STATE.CLOSED && MenuRef.MobileMenuButtonFocused) {
                ActionHandler.SetFocus(MenuControls.FIRST_LINK_ID);
            } else if (State == MENU_STATE.OPENED) {
                await Close();
            }
        // Change to mobile:
        } else if (MOBILE_MENU_BREAKPOINT <= e.WidthPrevious && e.Width < MOBILE_MENU_BREAKPOINT) {
            if (State == MENU_STATE.CLOSED && MenuRef.ControlsFocused) {
                ActionHandler.SetFocus(NavMenu.MOBILE_MENU_BUTTON_ID);
            }
        }
    }
}

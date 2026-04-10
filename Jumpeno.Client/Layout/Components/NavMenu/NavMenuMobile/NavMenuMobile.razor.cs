namespace Jumpeno.Client.Components;

public partial class NavMenuMobile {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "nav-menu-mobile";
    public const string CLASS_SELECTOR = $".{CLASS}";
    public const double MOBILE_MENU_BREAKPOINT = 1200;

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public NavMenuSurface? Surface { get; set; } = NavMenuSurface.SECONDARY;
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
    private readonly DotNetObjectReference<NavMenuMobile> ObjRef;
    private ScrollArea ScrollAreaRef = null!;
    private MenuState State { get; set; } = MenuState.CLOSED;
    private readonly LockerSlim Lock = new();
    private TaskCompletionSource StateTCS { get; set; } = null!;
    
    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(CLASS, Base)
        .SetSurface(Surface)
        .Set(State);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public NavMenuMobile() {
        ID = IDGenerator.Generate(CLASS);
        ObjRef = DotNetObjectReference.Create(this);
    }

    protected override async Task OnComponentAfterRenderAsync(bool firstRender) {
        if (firstRender) {
            await Window.AddResizeEventListener(ObjRef, JS_OnWindowResize);
            await Navigator.AddAfterFinishEventListener(CloseAfter);
        } else {
            if (State == MenuState.OPENED || State == MenuState.CLOSED) {
                StateTCS?.TrySetResult();
            } 
        }
    }

    protected override async ValueTask OnComponentDisposeAsync() {
        if (!AppEnvironment.IsServer) {
            await Navigator.RemoveAfterFinishEventListener(CloseAfter);
            await Window.RemoveResizeEventListener(ObjRef, JS_OnWindowResize);
        }
        await Lock.DisposeSafe();
        ObjRef.Dispose();
    }

    // Listeners --------------------------------------------------------------------------------------------------------------------------
    private async Task CloseAfter(NavigationEvent e) => await Close();

    private async Task OnKeyDown(KeyboardEventArgs e) {
        if (e.Key != KeyBoard.ESC) return;
        await Close();
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task Open() {
        await Lock.TryExclusive(async () => {
            if (State != MenuState.CLOSED) return;

            await PageLoader.Show(PageLoaderTask.MENU, true);

            var objRef = DotNetObjectReference.Create(this);
            AnimationHandler.CallOnAnimationEnd(CLASS_SELECTOR, objRef, nameof(JS_OnAnimationEnd));
            State = MenuState.OPENING;
            StateTCS = new TaskCompletionSource();
            StateHasChanged();
            ScrollAreaRef.ScrollTo(0, 0);
            await StateTCS.Task;

            State = MenuState.OPENED;
            StateTCS = new TaskCompletionSource();
            StateHasChanged();
            await StateTCS.Task;

            objRef.Dispose();
            await PageLoader.Hide(PageLoaderTask.MENU, false);

            ActionHandler.SetFocus(ID);

            await OnMobileMenuOpen.InvokeAsync();
        });
    }
    public async Task Close() {
        await Lock.TryExclusive(async () => {
            if (State != MenuState.OPENED) return;
            await PageLoader.Show(PageLoaderTask.MENU, true);

            var objRef = DotNetObjectReference.Create(this);
            AnimationHandler.CallOnAnimationEnd(CLASS_SELECTOR, objRef, nameof(JS_OnAnimationEnd));
            State = MenuState.CLOSING;
            StateTCS = new TaskCompletionSource();
            StateHasChanged();
            await StateTCS.Task;

            State = MenuState.CLOSED;
            StateTCS = new TaskCompletionSource();
            StateHasChanged();
            await StateTCS.Task;

            objRef.Dispose();
            await PageLoader.Hide(PageLoaderTask.MENU, false);
        
            var windowSize = Window.GetSize();
            if (windowSize.Width < MOBILE_MENU_BREAKPOINT) {
                ActionHandler.SetFocus(NavMenu.MOBILE_MENU_BUTTON_ID);
            } else {
                ActionHandler.SetFocus(MenuControls.FIRST_LINK_ID);
            }

            await OnMobileMenuClose.InvokeAsync();
        });
    }

    // JS Interop -------------------------------------------------------------------------------------------------------------------------
    [JSInvokable]
    public void JS_OnAnimationEnd() => StateTCS.TrySetResult();

    [JSInvokable]
    public async Task JS_OnWindowResize(WindowResizeEvent e) {
        // Change to desktop:
        if (e.WidthPrevious < MOBILE_MENU_BREAKPOINT && MOBILE_MENU_BREAKPOINT <= e.Width) {
            if (State == MenuState.CLOSED && MenuRef.MobileMenuButtonFocused) {
                ActionHandler.SetFocus(MenuControls.FIRST_LINK_ID);
            } else if (State == MenuState.OPENED) {
                await Close();
            }
        // Change to mobile:
        } else if (MOBILE_MENU_BREAKPOINT <= e.WidthPrevious && e.Width < MOBILE_MENU_BREAKPOINT) {
            if (State == MenuState.CLOSED && MenuRef.ControlsFocused) {
                ActionHandler.SetFocus(NavMenu.MOBILE_MENU_BUTTON_ID);
            }
        }
    }
}

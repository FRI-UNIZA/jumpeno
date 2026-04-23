namespace Jumpeno.Client.Components;

public partial class NavMenuMobile {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "nav-menu-mobile";
    public const string ClassSelector = $".{ClassName}";
    public const double MobileMenuBreakpoint = 1200;

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public NavMenuSurface? Surface { get; set; } = NavMenuSurface.Secondary;
    [Parameter]
    public required NavMenu MenuRef { get; set; }
    [Parameter]
    public EventCallback OnMobileMenuOpen { get; set; } = EventCallback.Empty;
    [Parameter]
    public EventCallback OnMobileMenuClose { get; set; } = EventCallback.Empty;
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public string Id { get; private set; }
    private readonly DotNetObjectReference<NavMenuMobile> ObjRef;
    private ScrollArea ScrollAreaRef = null!;
    private MenuState State { get; set; } = MenuState.Closed;
    private readonly LockerSlim Lock = new();
    private TaskCompletionSource StateTCS { get; set; } = null!;
    
    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() {
        return base.ComputeClass()
        .Set(ClassName, Base)
        .SetSurface(Surface)
        .Set(State);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public NavMenuMobile() {
        Id = IDGenerator.Generate(ClassName);
        ObjRef = DotNetObjectReference.Create(this);
    }

    protected override async Task OnComponentAfterRenderAsync(bool firstRender) {
        if (firstRender) {
            await Window.AddResizeEventListener(ObjRef, JS_OnWindowResize);
            await Navigator.AddAfterFinishEventListener(CloseAfter);
        } else {
            if (State == MenuState.Opened || State == MenuState.Closed) {
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
        if (e.Key != KeyBoard.Esc) return;
        await Close();
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task Open() {
        await Lock.TryExclusive(async () => {
            if (State != MenuState.Closed) return;

            await PageLoader.Show(PageLoaderTask.Menu, true);

            var objRef = DotNetObjectReference.Create(this);
            AnimationHandler.CallOnAnimationEnd(ClassSelector, objRef, nameof(JS_OnAnimationEnd));
            State = MenuState.Opening;
            StateTCS = new TaskCompletionSource();
            StateHasChanged();
            ScrollAreaRef.ScrollTo(0, 0);
            await StateTCS.Task;

            State = MenuState.Opened;
            StateTCS = new TaskCompletionSource();
            StateHasChanged();
            await StateTCS.Task;

            objRef.Dispose();
            await PageLoader.Hide(PageLoaderTask.Menu, false);

            ActionHandler.SetFocus(Id);

            await OnMobileMenuOpen.InvokeAsync();
        });
    }
    public async Task Close() {
        await Lock.TryExclusive(async () => {
            if (State != MenuState.Opened) return;
            await PageLoader.Show(PageLoaderTask.Menu, true);

            var objRef = DotNetObjectReference.Create(this);
            AnimationHandler.CallOnAnimationEnd(ClassSelector, objRef, nameof(JS_OnAnimationEnd));
            State = MenuState.Closing;
            StateTCS = new TaskCompletionSource();
            StateHasChanged();
            await StateTCS.Task;

            State = MenuState.Closed;
            StateTCS = new TaskCompletionSource();
            StateHasChanged();
            await StateTCS.Task;

            objRef.Dispose();
            await PageLoader.Hide(PageLoaderTask.Menu, false);
        
            var windowSize = Window.GetSize();
            if (windowSize.Width < MobileMenuBreakpoint) {
                ActionHandler.SetFocus(NavMenu.MobileMenuButtonId);
            } else {
                ActionHandler.SetFocus(MenuControls.FirstLinkId);
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
        if (e.WidthPrevious < MobileMenuBreakpoint && MobileMenuBreakpoint <= e.Width) {
            if (State == MenuState.Closed && MenuRef.MobileMenuButtonFocused) {
                ActionHandler.SetFocus(MenuControls.FirstLinkId);
            } else if (State == MenuState.Opened) {
                await Close();
            }
        // Change to mobile:
        } else if (MobileMenuBreakpoint <= e.WidthPrevious && e.Width < MobileMenuBreakpoint) {
            if (State == MenuState.Closed && MenuRef.ControlsFocused) {
                ActionHandler.SetFocus(NavMenu.MobileMenuButtonId);
            }
        }
    }
}

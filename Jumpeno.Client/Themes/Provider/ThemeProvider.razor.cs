namespace Jumpeno.Client.Themes;

#pragma warning disable CS8618

public partial class ThemeProvider {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    // Cascade:
    public const string CascadeAppTheme = $"{nameof(ThemeProvider)}.{nameof(CascadeAppTheme)}";
    public const string CascadeChangeAppTheme = $"{nameof(ThemeProvider)}.{nameof(CascadeChangeAppTheme)}";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public RenderFragment ChildContent { get; set; }
    
    // Initialization ---------------------------------------------------------------------------------------------------------------------
    // Component:
    private readonly TaskCompletionSource InitTCS = new();
    public Task Initialization => InitTCS.Task;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public ThemeProvider()
    {
        ThemeUtils.Theme = ThemeType.Default;
    }
    protected override async Task OnInitializedAsync() {
        await HTTP.Sync(() => {
            var cookie = ThemeUtils.GetThemeCookie();
            if (AppSettings.Theme.AutoDetect && cookie is null) {
                if (AppEnvironment.IsServer) {
                    ThemeUtils.Theme = ThemeType.Default;
                } else {
                    ThemeUtils.Theme = JS.Invoke<bool>(JSThemeProvider.DarkThemePreferred) ? ThemeType.Dark : ThemeType.Light;
                }
            } else if (cookie is null) {
                ThemeUtils.Theme = ThemeType.Default;
            } else {
                ThemeUtils.Theme = ThemeUtils.CreateThemeByName(cookie);
                if (!AppEnvironment.IsServer) {
                    ThemeUtils.SetThemeCookie(ThemeUtils.Theme);
                }
            }
            ScrollArea.SetTheme(ThemeUtils.Theme.BodyScrollTheme);
            InitTCS.SetResult();
        });
    }

    

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task<bool> ChangeAppTheme(BaseTheme theme) {
        try {
            await PageLoader.Show(PageLoaderTask.ThemeChange);
            if (AppEnvironment.IsServer) throw new InvalidOperationException("Theme change not allowed on the server!");
            if (theme.GetType().Name == ThemeUtils.Theme.GetType().Name) throw new InvalidOperationException("Theme already set!");
            await HTTP.Sync(() => ThemeUtils.SetThemeCookie(theme));
            ThemeUtils.Theme = theme;
            return true;
        } catch {
            Notification.Error(Messages.Default.T);
            return false;
        } finally {
            ScrollArea.SavePositions();
            ActionHandler.PreventScroll();
            AnimationHandler.DisableAnimation();
            JS.InvokeVoid(JSThemeProvider.StartSettingTheme);
            JS.InvokeVoid(JSThemeProvider.SetCustomTheme, ThemeUtils.ThemeCssClass());
            ScrollArea.SetTheme(ThemeUtils.Theme.BodyScrollTheme);
            StateHasChanged();
            await Task.Yield();
            AnimationHandler.RestoreAnimation();
            await Task.Yield();
            ScrollArea.RestorePositions();
            JS.InvokeVoid(JSThemeProvider.ApplyThemeAnimation);
            await Task.Delay(ThemeUtils.Theme.TransitionExtraSlow);
            ActionHandler.RestoreScroll();
            JS.InvokeVoid(JSThemeProvider.FinishSettingTheme);
            await PageLoader.Hide(PageLoaderTask.ThemeChange);
        }
    }
}

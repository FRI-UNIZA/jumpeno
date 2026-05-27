namespace Jumpeno.Client.Components;

public partial class SwitchTheme {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "switch-theme";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [CascadingParameter(Name = ThemeProvider.CascadeChangeAppTheme)]
    public required Func<BaseTheme, Task<bool>> ChangeAppTheme { get; set; }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly string formId = Form.Of<SwitchTheme>();
    private readonly SwitchViewModel SwitchVM;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public SwitchTheme() => SwitchVM = new(new(
        formId,
        ClassName,
        OnChange: new(async e => await PageLoader.Show(PageLoaderTask.ThemeChangeSwitch)),
        OnAfterChange: new(TriggerChange)
    ));
    protected override void OnComponentParametersSet(bool firstTime) => SwitchVM.SetValue(AppTheme is LightTheme);

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task TriggerChange(SwitchEvent e) {
        if (!await ChangeAppTheme(AppTheme is DarkTheme ? new LightTheme() : new DarkTheme())) {
            SwitchVM.SetValue(!e.Value);
        }
        await PageLoader.Hide(PageLoaderTask.ThemeChangeSwitch);
    }
}

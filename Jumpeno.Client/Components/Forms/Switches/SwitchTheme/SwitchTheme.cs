namespace Jumpeno.Client.Components;

public partial class SwitchTheme {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "switch-theme";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [CascadingParameter(Name = ThemeProvider.CASCADE_CHANGE_APP_THEME)]
    public required Func<BaseTheme, Task> ChangeAppTheme { get; set; }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly string FORM = Form.Of<SwitchTheme>();
    private readonly SwitchViewModel SwitchVM;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public SwitchTheme() => SwitchVM = new(new(
        FORM,
        CLASS,
        OnChange: new(async e => await PageLoader.Show(PAGE_LOADER_TASK.THEME_CHANGE)),
        OnAfterChange: new(async e => await TriggerChange())
    ));
    protected override void OnComponentParametersSet(bool firstTime) => SwitchVM.SetValue(AppTheme is LightTheme);

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task TriggerChange() => await ChangeAppTheme(AppTheme is DarkTheme ? new LightTheme() : new DarkTheme());
}

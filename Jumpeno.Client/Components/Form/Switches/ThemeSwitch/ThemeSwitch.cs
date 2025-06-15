
namespace Jumpeno.Client.Components;

public partial class ThemeSwitch {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASSNAME = "theme-switch";
    public const string CLASSNAME_ELEMENT = "theme-switch-element";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [CascadingParameter]
    public required Func<BaseTheme, Task> ChangeAppTheme { get; set; }
    protected CSSClass ComputeClass() => ComputeClass(CLASSNAME);

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private bool DefaultValue;
    private Switch SwitchRef = null!;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentParametersSet(bool firstTime) => DefaultValue = AppTheme is LightTheme;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task TriggerChange() {
        await ChangeAppTheme(AppTheme is DarkTheme ? new LightTheme() : new DarkTheme());
    }
}

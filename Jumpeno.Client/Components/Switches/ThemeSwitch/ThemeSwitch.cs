
namespace Jumpeno.Client.Components;

public partial class ThemeSwitch {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASSNAME = "theme-switch";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [CascadingParameter]
    public required BaseTheme Theme { get; set; }
    [CascadingParameter]
    public required Func<BaseTheme, Task> ChangeTheme { get; set; }
    protected CSSClass ComputeClass() {
        return ComputeClass(CLASSNAME);
    }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private bool DefaultValue;
    private Switch SwitchRef = null!;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnParametersSet() {
        DefaultValue = Theme is LightTheme;
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task TriggerChange() {
        await ChangeTheme(Theme is DarkTheme ? new LightTheme() : new DarkTheme());
    }
}

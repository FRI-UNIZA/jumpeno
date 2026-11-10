namespace Jumpeno.Client.Components;

public partial class PasswordStrength
{
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "password-strength";
    public const int Segments = 4;

    // Paramters --------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public string Password { get; set; } = string.Empty;
    [Parameter]
    public bool HideLock { get; set; } = false;

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    private int FilledSegments => Math.Min(Segments, UserValidator.PasswordRules.Count(x => !x.invalid(Password)));

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    private static CssClass SegmentClass(bool filled) => new CssClass("segment").Set("filled", filled);
    private static CssClass RuleClass(bool completed) => new CssClass("rule").Set("comp", completed).Set("not-comp", !completed);
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base).Set("hide-lock", HideLock);
}

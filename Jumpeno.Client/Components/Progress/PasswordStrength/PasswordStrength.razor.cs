namespace Jumpeno.Client.Components;

public partial class PasswordStrength
{
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "password-strength";
    public const int SEGMENTS = 4;

    // Paramters --------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public string Password { get; set; } = string.Empty;
    [Parameter]
    public bool HideLock { get; set; } = false;

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    private int FilledSegments => Math.Min(SEGMENTS, UserValidator.PASSWORD_RULES.Count(x => !x.invalid(Password)));

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    private static CSSClass SegmentClass(bool filled) => new CSSClass("segment").Set("filled", filled);
    private static CSSClass RuleClass(bool completed) => new CSSClass("rule").Set("comp", completed).Set("not-comp", !completed);
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base).Set("hide-lock", HideLock);
}

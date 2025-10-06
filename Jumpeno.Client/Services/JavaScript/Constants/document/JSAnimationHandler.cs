namespace Jumpeno.Client.Constants;

public static class JSAnimationHandler {
    public static readonly string ClassName = nameof(JSAnimationHandler);

    public const string CLASS_DISABLED_ANIMATION = "disabled-animation";
    public const string CLASS_PREVENT_DISABLED_ANIMATION = "prevent-disabled-animation";

    public static readonly string DisableAnimation = $"{ClassName}.{nameof(DisableAnimation)}";
    public static readonly string RestoreAnimation = $"{ClassName}.{nameof(RestoreAnimation)}";
    public static readonly string CallOnAnimationEnd = $"{ClassName}.{nameof(CallOnAnimationEnd)}";
    public static readonly string CallOnTransitionEnd = $"{ClassName}.{nameof(CallOnTransitionEnd)}";
    public static readonly string RenderFrames = $"{ClassName}.{nameof(RenderFrames)}";
}

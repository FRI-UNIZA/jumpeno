namespace Jumpeno.Client.Constants;

public static class JSAnimationHandler {
    public static readonly string ClassName = nameof(JSAnimationHandler);

    public const string IMUNE_TRANSITION_CLASSNAME = "animation-handler-imune-transition";
    public const string IMUNE_ANIMATION_CLASSNAME = "animation-handler-imune-animation";

    public static readonly string SetTransitions = $"{ClassName}.{nameof(SetTransitions)}";
    public static readonly string DisableTransitions = $"{ClassName}.{nameof(DisableTransitions)}";
    public static readonly string RestoreTransitions = $"{ClassName}.{nameof(RestoreTransitions)}";
    public static readonly string DisableAnimations = $"{ClassName}.{nameof(DisableAnimations)}";
    public static readonly string RestoreAnimations = $"{ClassName}.{nameof(RestoreAnimations)}";
    public static readonly string CallOnAnimationEnd = $"{ClassName}.{nameof(CallOnAnimationEnd)}";
    public static readonly string CallOnTransitionEnd = $"{ClassName}.{nameof(CallOnTransitionEnd)}";
    public static readonly string RenderFrames = $"{ClassName}.{nameof(RenderFrames)}";
}

namespace Jumpeno.Client.Constants;

public static class JSAnimationHandler {
    public static readonly string ClassName = typeof(JSAnimationHandler).Name;

    public const string IMUNE_TRANSITION_CLASSNAME = "animation-handler-imune-transition";
    public const string IMUNE_ANIMATION_CLASSNAME = "animation-handler-imune-animation";

    public static readonly string SetTransitions = $"{ClassName}.SetTransitions";
    public static readonly string DisableTransitions = $"{ClassName}.DisableTransitions";
    public static readonly string RestoreTransitions = $"{ClassName}.RestoreTransitions";
    public static readonly string DisableAnimations = $"{ClassName}.DisableAnimations";
    public static readonly string RestoreAnimations = $"{ClassName}.RestoreAnimations";
    public static readonly string CallOnAnimationEnd = $"{ClassName}.CallOnAnimationEnd";
    public static readonly string RenderFrames = $"{ClassName}.RenderFrames";
}

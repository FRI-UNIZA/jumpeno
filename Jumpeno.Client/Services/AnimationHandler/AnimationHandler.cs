namespace Jumpeno.Client.Services;

public class AnimationHandler {
    public const string IMUNE_TRANSITION_CLASSNAME = JSAnimationHandler.IMUNE_TRANSITION_CLASSNAME;
    public const string IMUNE_ANIMATION_CLASSNAME = JSAnimationHandler.IMUNE_ANIMATION_CLASSNAME;

    public static void SetTransitions(string timing = "") { JS.InvokeVoid(JSAnimationHandler.SetTransitions, timing); }
    public static async Task SetTransitionsAsync(string timing = "") { await JS.InvokeVoidAsync(JSAnimationHandler.SetTransitions, timing); }
    public static void DisableTransitions() { JS.InvokeVoid(JSAnimationHandler.DisableTransitions);}
    public static async Task DisableTransitionsAsync() { await JS.InvokeVoidAsync(JSAnimationHandler.DisableTransitions);}
    public static void RestoreTransitions() { JS.InvokeVoid(JSAnimationHandler.RestoreTransitions); }
    public static async Task RestoreTransitionsAsync() { await JS.InvokeVoidAsync(JSAnimationHandler.RestoreTransitions); }
    public static void DisableAnimations() { JS.InvokeVoid(JSAnimationHandler.DisableAnimations); }
    public static async Task DisableAnimationsAsync() { await JS.InvokeVoidAsync(JSAnimationHandler.DisableAnimations); }
    public static void RestoreAnimations() { JS.InvokeVoid(JSAnimationHandler.RestoreAnimations); }
    public static async Task RestoreAnimationsAsync() { await JS.InvokeVoidAsync(JSAnimationHandler.RestoreAnimations); }
    public static void CallOnAnimationEnd<T>(string selector, DotNetObjectReference<T> objRef, string method) where T : class
    { JS.InvokeVoid(JSAnimationHandler.CallOnAnimationEnd, selector, objRef, method); }
    public static async Task CallOnAnimationEndAsync<T>(string selector, DotNetObjectReference<T> objRef, string method) where T : class
    { await JS.InvokeVoidAsync(JSAnimationHandler.CallOnAnimationEnd, selector, objRef, method); }
}

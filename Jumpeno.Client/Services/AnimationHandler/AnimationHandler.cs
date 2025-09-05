namespace Jumpeno.Client.Services;

public class AnimationHandler {
    public const string IMUNE_TRANSITION_CLASSNAME = JSAnimationHandler.IMUNE_TRANSITION_CLASSNAME;
    public const string IMUNE_ANIMATION_CLASSNAME = JSAnimationHandler.IMUNE_ANIMATION_CLASSNAME;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static void SetTransitions(string timing = "") => JS.InvokeVoid(JSAnimationHandler.SetTransitions, timing);
    public static async Task SetTransitionsAsync(string timing = "") => await JS.InvokeVoidAsync(JSAnimationHandler.SetTransitions, timing);

    public static void DisableTransitions() => JS.InvokeVoid(JSAnimationHandler.DisableTransitions);
    public static async Task DisableTransitionsAsync() => await JS.InvokeVoidAsync(JSAnimationHandler.DisableTransitions);

    public static void RestoreTransitions() => JS.InvokeVoid(JSAnimationHandler.RestoreTransitions);
    public static async Task RestoreTransitionsAsync() => await JS.InvokeVoidAsync(JSAnimationHandler.RestoreTransitions);

    public static void DisableAnimations() => JS.InvokeVoid(JSAnimationHandler.DisableAnimations);
    public static async Task DisableAnimationsAsync() => await JS.InvokeVoidAsync(JSAnimationHandler.DisableAnimations);

    public static void RestoreAnimations() => JS.InvokeVoid(JSAnimationHandler.RestoreAnimations);
    public static async Task RestoreAnimationsAsync() => await JS.InvokeVoidAsync(JSAnimationHandler.RestoreAnimations);

    // Animation end ----------------------------------------------------------------------------------------------------------------------
    public static void CallOnAnimationEnd<T>(string selector, DotNetObjectReference<T> objRef, string method) where T : class
    => JS.InvokeVoid(JSAnimationHandler.CallOnAnimationEnd, selector, objRef, method);
    public static async Task CallOnAnimationEndAsync<T>(string selector, DotNetObjectReference<T> objRef, string method) where T : class
    => await JS.InvokeVoidAsync(JSAnimationHandler.CallOnAnimationEnd, selector, objRef, method);

    public static void CallOnAnimationEnd(string selector, Action action)
    => JS.InvokeVoid(JSAnimationHandler.CallOnAnimationEnd, selector, new JSInvoker(new(action)).Ref, nameof(JSInvoker.JS_Execute));
    public static async Task CallOnAnimationEndAsync(string selector, Action action)
    => await JS.InvokeVoidAsync(JSAnimationHandler.CallOnAnimationEnd, selector, new JSInvoker(new(action)).Ref, nameof(JSInvoker.JS_Execute));
    public static void CallOnAnimationEnd(string selector, Func<Task> action)
    => JS.InvokeVoid(JSAnimationHandler.CallOnAnimationEnd, selector, new JSInvoker(new(action)).Ref, nameof(JSInvoker.JS_Execute));
    public static async Task CallOnAnimationEndAsync(string selector, Func<Task> action)
    => await JS.InvokeVoidAsync(JSAnimationHandler.CallOnAnimationEnd, selector, new JSInvoker(new(action)).Ref, nameof(JSInvoker.JS_Execute));

    public static void SetOnAnimationEndEvent<T>(string selector, EventDelegate<T> action, T e) {
        if (action == EventDelegate<T>.EMPTY) return;
        CallOnAnimationEnd(selector, async () => await action.Invoke(e));
    }
    public static async Task SetOnAnimationEndEventAsync<T>(string selector, EventDelegate<T> action, T e) {
        if (action == EventDelegate<T>.EMPTY) return;
        await CallOnAnimationEndAsync(selector, async () => await action.Invoke(e));
    }

    // Transition end ---------------------------------------------------------------------------------------------------------------------
    public static void CallOnTransitionEnd<T>(string selector, DotNetObjectReference<T> objRef, string method) where T : class
    => JS.InvokeVoid(JSAnimationHandler.CallOnTransitionEnd, selector, objRef, method);
    public static async Task CallOnTransitionEndAsync<T>(string selector, DotNetObjectReference<T> objRef, string method) where T : class
    => await JS.InvokeVoidAsync(JSAnimationHandler.CallOnTransitionEnd, selector, objRef, method);

    public static void CallOnTransitionEnd(string selector, Action action)
    => JS.InvokeVoid(JSAnimationHandler.CallOnTransitionEnd, selector, new JSInvoker(new(action)).Ref, nameof(JSInvoker.JS_Execute));
    public static async Task CallOnTransitionEndAsync(string selector, Action action)
    => await JS.InvokeVoidAsync(JSAnimationHandler.CallOnTransitionEnd, selector, new JSInvoker(new(action)).Ref, nameof(JSInvoker.JS_Execute));
    public static void CallOnTransitionEnd(string selector, Func<Task> action)
    => JS.InvokeVoid(JSAnimationHandler.CallOnTransitionEnd, selector, new JSInvoker(new(action)).Ref, nameof(JSInvoker.JS_Execute));
    public static async Task CallOnTransitionEndAsync(string selector, Func<Task> action)
    => await JS.InvokeVoidAsync(JSAnimationHandler.CallOnTransitionEnd, selector, new JSInvoker(new(action)).Ref, nameof(JSInvoker.JS_Execute));

    public static void SetOnTransitionEndEvent<T>(string selector, EventDelegate<T> action, T e) {
        if (action == EventDelegate<T>.EMPTY) return;
        CallOnTransitionEnd(selector, async () => await action.Invoke(e));
    }
    public static async Task SetOnTransitionEndEventAsync<T>(string selector, EventDelegate<T> action, T e) {
        if (action == EventDelegate<T>.EMPTY) return;
        await CallOnTransitionEndAsync(selector, async () => await action.Invoke(e));
    }

    // Frames -----------------------------------------------------------------------------------------------------------------------------
    public static void RenderFrames(int count) => JS.InvokeVoid(JSAnimationHandler.RenderFrames, count);
    public static async Task RenderFramesAsync(int count) => await JS.InvokeVoidAsync(JSAnimationHandler.RenderFrames, count);
}

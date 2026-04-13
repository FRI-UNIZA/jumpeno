namespace Jumpeno.Client.Services;

public partial class AnimationHandler {
    // Class ------------------------------------------------------------------------------------------------------------------------------
    public const string CLASS_DISABLED_ANIMATION = JSAnimationHandler.CLASS_DISABLED_ANIMATION;
    public const string CLASS_PREVENT_DISABLED_ANIMATION = JSAnimationHandler.CLASS_PREVENT_DISABLED_ANIMATION;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static void DisableAnimation(string selector = "body") => JS.InvokeVoid(JSAnimationHandler.DisableAnimation, selector);
    public static void RestoreAnimation(string selector = "body") => JS.InvokeVoid(JSAnimationHandler.RestoreAnimation, selector);

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

    /// <summary>Requests animation frame.</summary>
    /// <typeparam name="T">Type of object to execute callback on</typeparam>
    /// <param name="objRef">Reference to an object to execute callback on</param>
    /// <param name="method">The method to invoke. It must be marked with the [JSInvokable] attribute</param>
    /// <returns>Task to await</returns>
    public static Task RequestAnimationFrame<T>(DotNetObjectReference<T> objRef, Action method) where T : class
    => JS.InvokeVoidAsync(JSAnimationHandler.RequestAnimationFrame, objRef, method.Method.Name);
    /// <summary>Requests animation frame.</summary>
    /// <typeparam name="T">Type of object to execute callback on</typeparam>
    /// <param name="objRef">Reference to an object to execute callback on</param>
    /// <param name="method">The method to invoke. It must be marked with the [JSInvokable] attribute</param>
    /// <returns>Task to await</returns>
    public static Task RequestAnimationFrame<T>(DotNetObjectReference<T> objRef, Func<Task> method) where T : class
    => JS.InvokeVoidAsync(JSAnimationHandler.RequestAnimationFrame, objRef, method.Method.Name);

    /// <summary>Executes animation frame and waits for it's completion.</summary>
    /// <param name="method">The method to invoke</param>
    /// <returns>Task to await</returns>
    public static async Task ExecuteAnimationFrame(Action method) {
        var obj = DotNetObjectReference.Create(new AnimationHandlerFrameObject(new(method)));
        await JS.InvokeVoidAsync(JSAnimationHandler.RequestAnimationFrame, obj, nameof(obj.Value.Execute));
        await obj.Value.Task;
        obj.Dispose();
    }
    /// <summary>Executes animation frame and waits for it's completion.</summary>
    /// <param name="method">The method to invoke</param>
    /// <returns>Task to await</returns>
    public static async Task ExecuteAnimationFrame(Func<Task> method) {
        var obj = DotNetObjectReference.Create(new AnimationHandlerFrameObject(new(method)));
        await JS.InvokeVoidAsync(JSAnimationHandler.RequestAnimationFrame, obj, nameof(obj.Value.Execute));
        await obj.Value.Task;
        obj.Dispose();
    }
}

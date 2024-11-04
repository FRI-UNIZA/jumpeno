namespace Jumpeno.Client.Services;

public static class Animator {
    public static void AddAnimator<T>(DotNetObjectReference<T> objRef, Action method) where T : class
    { JS.InvokeVoid(JSAnimator.AddAnimator, objRef, method.Method.Name); }
    public static void AddAnimator<T>(DotNetObjectReference<T> objRef, Func<Task> method) where T : class
    { JS.InvokeVoid(JSAnimator.AddAnimator, objRef, method.Method.Name); }
    public static void RemoveAnimator<T>(DotNetObjectReference<T> objRef, Action method) where T : class
    { JS.InvokeVoid(JSAnimator.RemoveAnimator, objRef, method.Method.Name); }
    public static void RemoveAnimator<T>(DotNetObjectReference<T> objRef, Func<Task> method) where T : class
    { JS.InvokeVoid(JSAnimator.RemoveAnimator, objRef, method.Method.Name); }
}

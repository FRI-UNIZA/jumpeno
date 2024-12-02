namespace Jumpeno.Client.Services;

public static class Animator {
    public static async Task AddAnimator<T>(DotNetObjectReference<T> objRef, Action method) where T : class
    { await JS.InvokeVoidAsync(JSAnimator.AddAnimator, objRef, method.Method.Name); }
    public static async Task AddAnimator<T>(DotNetObjectReference<T> objRef, Func<Task> method) where T : class
    { await JS.InvokeVoidAsync(JSAnimator.AddAnimator, objRef, method.Method.Name); }
    public static async Task RemoveAnimator<T>(DotNetObjectReference<T> objRef, Action method) where T : class
    { await JS.InvokeVoidAsync(JSAnimator.RemoveAnimator, objRef, method.Method.Name); }
    public static async Task RemoveAnimator<T>(DotNetObjectReference<T> objRef, Func<Task> method) where T : class
    { await JS.InvokeVoidAsync(JSAnimator.RemoveAnimator, objRef, method.Method.Name); }
}

namespace Jumpeno.Client.Services;

public class Window {
    // Size -------------------------------------------------------------------------------------------------------------------------------
    public static WindowSize GetSize() { return JS.Invoke<WindowSize>(JSWindow.GetSize); }
    public static async Task<WindowSize> GetSizeAsync() { return await JS.InvokeAsync<WindowSize>(JSWindow.GetSize); }

    // Resize -----------------------------------------------------------------------------------------------------------------------------
    public static async Task AddResizeEventListener<T>(DotNetObjectReference<T> objRef, Action<WindowResizeEvent> method) where T : class
    { await JS.InvokeVoidAsync(JSWindow.AddResizeEventListener, objRef, method.Method.Name); }
    public static async Task AddResizeEventListener<T>(DotNetObjectReference<T> objRef, Func<WindowResizeEvent, Task> method) where T : class
    { await JS.InvokeVoidAsync(JSWindow.AddResizeEventListener, objRef, method.Method.Name); }
    public static async Task RemoveResizeEventListener<T>(DotNetObjectReference<T> objRef, Action<WindowResizeEvent> method) where T : class
    { await JS.InvokeVoidAsync(JSWindow.RemoveResizeEventListener, objRef, method.Method.Name); }
    public static async Task RemoveResizeEventListener<T>(DotNetObjectReference<T> objRef, Func<WindowResizeEvent, Task> method) where T : class
    { await JS.InvokeVoidAsync(JSWindow.RemoveResizeEventListener, objRef, method.Method.Name); }

    // KeyDown ----------------------------------------------------------------------------------------------------------------------------
    public static async Task AddKeyDownEventListener<T>(DotNetObjectReference<T> objRef, Action<string> method) where T : class
    { await JS.InvokeVoidAsync(JSWindow.AddKeyDownEventListener, objRef, method.Method.Name); }
    public static async Task AddKeyDownEventListener<T>(DotNetObjectReference<T> objRef, Func<string, Task> method) where T : class
    { await JS.InvokeVoidAsync(JSWindow.AddKeyDownEventListener, objRef, method.Method.Name); }
    public static async Task RemoveKeyDownEventListener<T>(DotNetObjectReference<T> objRef, Action<string> method) where T : class
    { await JS.InvokeVoidAsync(JSWindow.RemoveKeyDownEventListener, objRef, method.Method.Name); }
    public static async Task RemoveKeyDownEventListener<T>(DotNetObjectReference<T> objRef, Func<string, Task> method) where T : class
    { await JS.InvokeVoidAsync(JSWindow.RemoveKeyDownEventListener, objRef, method.Method.Name); }

    // KeyUp ------------------------------------------------------------------------------------------------------------------------------
    public static async Task AddKeyUpEventListener<T>(DotNetObjectReference<T> objRef, Action<string> method) where T : class
    { await JS.InvokeVoidAsync(JSWindow.AddKeyUpEventListener, objRef, method.Method.Name); }
    public static async Task AddKeyUpEventListener<T>(DotNetObjectReference<T> objRef, Func<string, Task> method) where T : class
    { await JS.InvokeVoidAsync(JSWindow.AddKeyUpEventListener, objRef, method.Method.Name); }
    public static async Task RemoveKeyUpEventListener<T>(DotNetObjectReference<T> objRef, Action<string> method) where T : class
    { await JS.InvokeVoidAsync(JSWindow.RemoveKeyUpEventListener, objRef, method.Method.Name); }
    public static async Task RemoveKeyUpEventListener<T>(DotNetObjectReference<T> objRef, Func<string, Task> method) where T : class
    { await JS.InvokeVoidAsync(JSWindow.RemoveKeyUpEventListener, objRef, method.Method.Name); }
}

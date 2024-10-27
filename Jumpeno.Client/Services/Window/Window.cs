namespace Jumpeno.Client.Services;

public class Window {
    public static void AddResizeEventListener<T>(DotNetObjectReference<T> objRef, Action<WindowResizeEvent> method) where T : class
    { JS.InvokeVoid(JSWindow.AddResizeEventListener, objRef, method.Method.Name); }
    public static void AddResizeEventListener<T>(DotNetObjectReference<T> objRef, Func<WindowResizeEvent, Task> method) where T : class
    { JS.InvokeVoid(JSWindow.AddResizeEventListener, objRef, method.Method.Name); }
    public static void RemoveResizeEventListener<T>(DotNetObjectReference<T> objRef, Action<WindowResizeEvent> method) where T : class
    { JS.InvokeVoid(JSWindow.RemoveResizeEventListener, objRef, method.Method.Name); }
    public static void RemoveResizeEventListener<T>(DotNetObjectReference<T> objRef, Func<WindowResizeEvent, Task> method) where T : class
    { JS.InvokeVoid(JSWindow.RemoveResizeEventListener, objRef, method.Method.Name); }
    public static WindowSize GetSize() { return JS.Invoke<WindowSize>(JSWindow.GetSize); }
    public static async Task<WindowSize> GetSizeAsync() { return await JS.InvokeAsync<WindowSize>(JSWindow.GetSize); }
}

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

    // MouseDown --------------------------------------------------------------------------------------------------------------------------
    public static async Task AddMouseDownEventListener<T>(DotNetObjectReference<T> objRef, Action<(int X, int Y)> method) where T : class
    { await JS.InvokeVoidAsync(JSWindow.AddMouseDownEventListener, objRef, method.Method.Name); }
    public static async Task AddMouseDownEventListener<T>(DotNetObjectReference<T> objRef, Func<(int X, int Y), Task> method) where T : class
    { await JS.InvokeVoidAsync(JSWindow.AddMouseDownEventListener, objRef, method.Method.Name); }
    public static async Task RemoveMouseDownEventListener<T>(DotNetObjectReference<T> objRef, Action<(int X, int Y)> method) where T : class
    { await JS.InvokeVoidAsync(JSWindow.RemoveMouseDownEventListener, objRef, method.Method.Name); }
    public static async Task RemoveMouseDownEventListener<T>(DotNetObjectReference<T> objRef, Func<(int X, int Y), Task> method) where T : class
    { await JS.InvokeVoidAsync(JSWindow.RemoveMouseDownEventListener, objRef, method.Method.Name); }

    // MouseUp ----------------------------------------------------------------------------------------------------------------------------
    public static async Task AddMouseUpEventListener<T>(DotNetObjectReference<T> objRef, Action<(int X, int Y)> method) where T : class
    { await JS.InvokeVoidAsync(JSWindow.AddMouseUpEventListener, objRef, method.Method.Name); }
    public static async Task AddMouseUpEventListener<T>(DotNetObjectReference<T> objRef, Func<(int X, int Y), Task> method) where T : class
    { await JS.InvokeVoidAsync(JSWindow.AddMouseUpEventListener, objRef, method.Method.Name); }
    public static async Task RemoveMouseUpEventListener<T>(DotNetObjectReference<T> objRef, Action<(int X, int Y)> method) where T : class
    { await JS.InvokeVoidAsync(JSWindow.RemoveMouseUpEventListener, objRef, method.Method.Name); }
    public static async Task RemoveMouseUpEventListener<T>(DotNetObjectReference<T> objRef, Func<(int X, int Y), Task> method) where T : class
    { await JS.InvokeVoidAsync(JSWindow.RemoveMouseUpEventListener, objRef, method.Method.Name); }

    // User select ------------------------------------------------------------------------------------------------------------------------
    public static void BlockUserSelect() => JS.InvokeVoid(JSWindow.BlockUserSelect);
    public static async Task BlockUserSelectAsync() => await JS.InvokeVoidAsync(JSWindow.BlockUserSelect);
    public static void AllowUserSelect() => JS.InvokeVoid(JSWindow.AllowUserSelect);
    public static async Task AllowUserSelectAsync() => await JS.InvokeVoidAsync(JSWindow.AllowUserSelect);

    // Touch actions ----------------------------------------------------------------------------------------------------------------------
    public static void TouchActionAutoOn() => JS.InvokeVoid(JSWindow.TouchActionAutoOn);
    public static async Task TouchActionAutoOnAsync() => await JS.InvokeVoidAsync(JSWindow.TouchActionAutoOn);
    public static void TouchActionAutoOff() => JS.InvokeVoid(JSWindow.TouchActionAutoOff);
    public static async Task TouchActionAutoOffAsync() => await JS.InvokeVoidAsync(JSWindow.TouchActionAutoOff);

    public static void TouchActionNoneOn() => JS.InvokeVoid(JSWindow.TouchActionNoneOn);
    public static async Task TouchActionNoneOnAsync() => await JS.InvokeVoidAsync(JSWindow.TouchActionNoneOn);
    public static void TouchActionNoneOff() => JS.InvokeVoid(JSWindow.TouchActionNoneOff);
    public static async Task TouchActionNoneOffAsync() => await JS.InvokeVoidAsync(JSWindow.TouchActionNoneOff);

    public static void TouchActionPanOn() => JS.InvokeVoid(JSWindow.TouchActionPanOn);
    public static async Task TouchActionPanOnAsync() => await JS.InvokeVoidAsync(JSWindow.TouchActionPanOn);
    public static void TouchActionPanOff() => JS.InvokeVoid(JSWindow.TouchActionPanOff);
    public static async Task TouchActionPanOffAsync() => await JS.InvokeVoidAsync(JSWindow.TouchActionPanOff);

    public static void TouchActionPinchZoomOn() => JS.InvokeVoid(JSWindow.TouchActionPinchZoomOn);
    public static async Task TouchActionPinchZoomOnAsync() => await JS.InvokeVoidAsync(JSWindow.TouchActionPinchZoomOn);
    public static void TouchActionPinchZoomOff() => JS.InvokeVoid(JSWindow.TouchActionPinchZoomOff);
    public static async Task TouchActionPinchZoomOffAsync() => await JS.InvokeVoidAsync(JSWindow.TouchActionPinchZoomOff);

    public static void TouchActionManipulationOn() => JS.InvokeVoid(JSWindow.TouchActionManipulationOn);
    public static async Task TouchActionManipulationOnAsync() => await JS.InvokeVoidAsync(JSWindow.TouchActionManipulationOn);
    public static void TouchActionManipulationOff() => JS.InvokeVoid(JSWindow.TouchActionManipulationOff);
    public static async Task TouchActionManipulationOffAsync() => await JS.InvokeVoidAsync(JSWindow.TouchActionManipulationOff);

    // Overscroll -------------------------------------------------------------------------------------------------------------------------
    public static void OverscrollAutoOn() => JS.InvokeVoid(JSWindow.OverscrollAutoOn);
    public static async Task OverscrollAutoOnAsync() => await JS.InvokeVoidAsync(JSWindow.OverscrollAutoOn);
    public static void OverscrollAutoOff() => JS.InvokeVoid(JSWindow.OverscrollAutoOff);
    public static async Task OverscrollAutoOffAsync() => await JS.InvokeVoidAsync(JSWindow.OverscrollAutoOff);
    
    public static void OverscrollContainOn() => JS.InvokeVoid(JSWindow.OverscrollContainOn);
    public static async Task OverscrollContainOnAsync() => await JS.InvokeVoidAsync(JSWindow.OverscrollContainOn);
    public static void OverscrollContainOff() => JS.InvokeVoid(JSWindow.OverscrollContainOff);
    public static async Task OverscrollContainOffAsync() => await JS.InvokeVoidAsync(JSWindow.OverscrollContainOff);

    public static void OverscrollNoneOn() => JS.InvokeVoid(JSWindow.OverscrollNoneOn);
    public static async Task OverscrollNoneOnAsync() => await JS.InvokeVoidAsync(JSWindow.OverscrollNoneOn);
    public static void OverscrollNoneOff() => JS.InvokeVoid(JSWindow.OverscrollNoneOff);
    public static async Task OverscrollNoneOffAsync() => await JS.InvokeVoidAsync(JSWindow.OverscrollNoneOff);

    // Prevents ---------------------------------------------------------------------------------------------------------------------------
    public static void PreventTouchStart() => JS.InvokeVoid(JSWindow.PreventTouchStart);
    public static async Task PreventTouchStartAsync() => await JS.InvokeVoidAsync(JSWindow.PreventTouchStart);
    public static void DefaultTouchStart() => JS.InvokeVoid(JSWindow.DefaultTouchStart);
    public static async Task DefaultTouchStartAsync() => await JS.InvokeVoidAsync(JSWindow.DefaultTouchStart);
    
    public static void PreventTouchMove() => JS.InvokeVoid(JSWindow.PreventTouchMove);
    public static async Task PreventTouchMoveAsync() => await JS.InvokeVoidAsync(JSWindow.PreventTouchMove);
    public static void DefaultTouchMove() => JS.InvokeVoid(JSWindow.DefaultTouchMove);
    public static async Task DefaultTouchMoveAsync() => await JS.InvokeVoidAsync(JSWindow.DefaultTouchMove);

    public static void PreventTouchEnd() => JS.InvokeVoid(JSWindow.PreventTouchEnd);
    public static async Task PreventTouchEndAsync() => await JS.InvokeVoidAsync(JSWindow.PreventTouchEnd);
    public static void DefaultTouchEnd() => JS.InvokeVoid(JSWindow.DefaultTouchEnd);
    public static async Task DefaultTouchEndAsync() => await JS.InvokeVoidAsync(JSWindow.DefaultTouchEnd);
}

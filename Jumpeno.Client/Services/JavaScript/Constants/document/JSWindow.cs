namespace Jumpeno.Client.Constants;

public static class JSWindow {
    public static readonly string ClassName = typeof(JSWindow).Name;
    
    public static readonly string GetSize = $"{ClassName}.GetSize";

    public static readonly string AddResizeEventListener = $"{ClassName}.AddResizeEventListener";
    public static readonly string RemoveResizeEventListener = $"{ClassName}.RemoveResizeEventListener";

    public static readonly string AddKeyDownEventListener = $"{ClassName}.AddKeyDownEventListener";
    public static readonly string RemoveKeyDownEventListener = $"{ClassName}.RemoveKeyDownEventListener";

    public static readonly string AddKeyUpEventListener = $"{ClassName}.AddKeyUpEventListener";
    public static readonly string RemoveKeyUpEventListener = $"{ClassName}.RemoveKeyUpEventListener";
    
    public static readonly string AddMouseDownEventListener = $"{ClassName}.AddMouseDownEventListener";
    public static readonly string RemoveMouseDownEventListener = $"{ClassName}.RemoveMouseDownEventListener";
    
    public static readonly string AddMouseUpEventListener = $"{ClassName}.AddMouseUpEventListener";
    public static readonly string RemoveMouseUpEventListener = $"{ClassName}.RemoveMouseUpEventListener";

    public static readonly string BlockUserSelect = $"{ClassName}.BlockUserSelect";
    public static readonly string AllowUserSelect = $"{ClassName}.AllowUserSelect";

    public static readonly string TouchActionAutoOn = $"{ClassName}.TouchActionAutoOn";
    public static readonly string TouchActionAutoOff = $"{ClassName}.TouchActionAutoOff";

    public static readonly string TouchActionNoneOn = $"{ClassName}.TouchActionNoneOn";
    public static readonly string TouchActionNoneOff = $"{ClassName}.TouchActionNoneOff";

    public static readonly string TouchActionPanOn = $"{ClassName}.TouchActionPanOn";
    public static readonly string TouchActionPanOff = $"{ClassName}.TouchActionPanOff";

    public static readonly string TouchActionPinchZoomOn = $"{ClassName}.TouchActionPinchZoomOn";
    public static readonly string TouchActionPinchZoomOff = $"{ClassName}.TouchActionPinchZoomOff";

    public static readonly string TouchActionManipulationOn = $"{ClassName}.TouchActionManipulationOn";
    public static readonly string TouchActionManipulationOff = $"{ClassName}.TouchActionManipulationOff";
}

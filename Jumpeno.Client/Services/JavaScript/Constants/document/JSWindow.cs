namespace Jumpeno.Client.Constants;

public static class JSWindow {
    public static readonly string ClassName = nameof(JSWindow);
    
    public static readonly string GetSize = $"{ClassName}.{nameof(GetSize)}";

    public static readonly string AddResizeEventListener = $"{ClassName}.{nameof(AddResizeEventListener)}";
    public static readonly string RemoveResizeEventListener = $"{ClassName}.{nameof(RemoveResizeEventListener)}";

    public static readonly string AddKeyDownEventListener = $"{ClassName}.{nameof(AddKeyDownEventListener)}";
    public static readonly string RemoveKeyDownEventListener = $"{ClassName}.{nameof(RemoveKeyDownEventListener)}";

    public static readonly string AddKeyUpEventListener = $"{ClassName}.{nameof(AddKeyUpEventListener)}";
    public static readonly string RemoveKeyUpEventListener = $"{ClassName}.{nameof(RemoveKeyUpEventListener)}";
    
    public static readonly string AddMouseDownEventListener = $"{ClassName}.{nameof(AddMouseDownEventListener)}";
    public static readonly string RemoveMouseDownEventListener = $"{ClassName}.{nameof(RemoveMouseDownEventListener)}";
    
    public static readonly string AddMouseUpEventListener = $"{ClassName}.{nameof(AddMouseUpEventListener)}";
    public static readonly string RemoveMouseUpEventListener = $"{ClassName}.{nameof(RemoveMouseUpEventListener)}";

    public static readonly string AddClickEventListener = $"{ClassName}.{nameof(AddClickEventListener)}";
    public static readonly string RemoveClickEventListener = $"{ClassName}.{nameof(RemoveClickEventListener)}";

    public static readonly string AddScrollEventListener = $"{ClassName}.{nameof(AddScrollEventListener)}";
    public static readonly string RemoveScrollEventListener = $"{ClassName}.{nameof(RemoveScrollEventListener)}";

    public static readonly string BlockUserSelect = $"{ClassName}.{nameof(BlockUserSelect)}";
    public static readonly string AllowUserSelect = $"{ClassName}.{nameof(AllowUserSelect)}";

    public static readonly string TouchActionAutoOn = $"{ClassName}.{nameof(TouchActionAutoOn)}";
    public static readonly string TouchActionAutoOff = $"{ClassName}.{nameof(TouchActionAutoOff)}";

    public static readonly string TouchActionNoneOn = $"{ClassName}.{nameof(TouchActionNoneOn)}";
    public static readonly string TouchActionNoneOff = $"{ClassName}.{nameof(TouchActionNoneOff)}";

    public static readonly string TouchActionPanOn = $"{ClassName}.{nameof(TouchActionPanOn)}";
    public static readonly string TouchActionPanOff = $"{ClassName}.{nameof(TouchActionPanOff)}";

    public static readonly string TouchActionPinchZoomOn = $"{ClassName}.{nameof(TouchActionPinchZoomOn)}";
    public static readonly string TouchActionPinchZoomOff = $"{ClassName}.{nameof(TouchActionPinchZoomOff)}";

    public static readonly string TouchActionManipulationOn = $"{ClassName}.{nameof(TouchActionManipulationOn)}";
    public static readonly string TouchActionManipulationOff = $"{ClassName}.{nameof(TouchActionManipulationOff)}";

    public static readonly string OverscrollAutoOn = $"{ClassName}.{nameof(OverscrollAutoOn)}";
    public static readonly string OverscrollAutoOff = $"{ClassName}.{nameof(OverscrollAutoOff)}";
    
    public static readonly string OverscrollContainOn = $"{ClassName}.{nameof(OverscrollContainOn)}";
    public static readonly string OverscrollContainOff = $"{ClassName}.{nameof(OverscrollContainOff)}";
    
    public static readonly string OverscrollNoneOn = $"{ClassName}.{nameof(OverscrollNoneOn)}";
    public static readonly string OverscrollNoneOff = $"{ClassName}.{nameof(OverscrollNoneOff)}";

    public static readonly string PreventTouchStart = $"{ClassName}.{nameof(PreventTouchStart)}";
    public static readonly string DefaultTouchStart = $"{ClassName}.{nameof(DefaultTouchStart)}";
    
    public static readonly string PreventTouchMove = $"{ClassName}.{nameof(PreventTouchMove)}";
    public static readonly string DefaultTouchMove = $"{ClassName}.{nameof(DefaultTouchMove)}";
    
    public static readonly string PreventTouchEnd = $"{ClassName}.{nameof(PreventTouchEnd)}";
    public static readonly string DefaultTouchEnd = $"{ClassName}.{nameof(DefaultTouchEnd)}";

    public static readonly string IsTouchDevice = $"{ClassName}.{nameof(IsTouchDevice)}";

    public static readonly string ReloadTabs = $"{ClassName}.{nameof(ReloadTabs)}";

    public static readonly string Lock = $"{ClassName}.{nameof(Lock)}";

    public static readonly string Inert = $"{ClassName}.{nameof(Inert)}";
}

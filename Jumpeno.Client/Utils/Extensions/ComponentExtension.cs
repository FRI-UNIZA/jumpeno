namespace Jumpeno.Client.Utils;

[EventHandler("ontransitionstart", typeof(EventArgs), enableStopPropagation: true, enablePreventDefault: true)]
[EventHandler("ontransitionend", typeof(EventArgs), enableStopPropagation: true, enablePreventDefault: true)]
[EventHandler("onanimationstart", typeof(EventArgs), enableStopPropagation: true, enablePreventDefault: true)]
[EventHandler("onanimationend", typeof(EventArgs), enableStopPropagation: true, enablePreventDefault: true)]
public static class EventHandlers {}

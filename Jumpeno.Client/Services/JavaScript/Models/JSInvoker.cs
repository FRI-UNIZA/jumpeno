namespace Jumpeno.Client.Models;

public class JSInvoker {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public DotNetObjectReference<JSInvoker> Ref;
    public EmptyDelegate Action;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public JSInvoker(EmptyDelegate action) {
        Ref = DotNetObjectReference.Create(this);
        Action = action;
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    [JSInvokable]
    public async Task JS_Execute() {
        Ref.Dispose();
        await Action.Invoke();
    }
}

public class JSEventInvoker<T> {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public DotNetObjectReference<JSEventInvoker<T>> Ref;
    public EventDelegate<T> Action;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public JSEventInvoker(EventDelegate<T> action) {
        Ref = DotNetObjectReference.Create(this);
        Action = action;
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    [JSInvokable]
    public async Task JS_Execute(T e) {
        Ref.Dispose();
        await Action.Invoke(e);
    }
}

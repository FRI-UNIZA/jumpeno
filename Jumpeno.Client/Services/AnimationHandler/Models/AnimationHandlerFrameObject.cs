namespace Jumpeno.Client.Models;

public class AnimationHandlerFrameObject(EmptyDelegate action) {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly EmptyDelegate Action = action;

    // Task to await:
    private readonly TaskCompletionSource TCS = new();
    public Task Task => TCS.Task;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    [JSInvokable]
    public async Task Execute() {
        try { await Action.Invoke(); }
        finally { TCS.TrySetResult(); }
    }
}

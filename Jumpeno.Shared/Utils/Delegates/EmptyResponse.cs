namespace Jumpeno.Shared.Utils;

#pragma warning disable CS1998

public class EmptyResponse<R> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static EmptyResponse<R> EMPTY(R response) => new(() => response);

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly Func<R>? WrappedAction;
    private readonly Func<Task<R>> Action;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public EmptyResponse(Func<R> action) {
        WrappedAction = action;
        Action = async () => action();
    }

    public EmptyResponse(Func<Task<R>> action) {
        WrappedAction = null;
        Action = action;
    }

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static Func<Task<R>> Task(Func<R> action) => () => {
        return System.Threading.Tasks.Task.FromResult(action());
    };

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public async Task<R> Invoke() => await Action();

    public bool Equals(EmptyResponse<R> o) {
        return WrappedAction is null ? Action == o.Action : WrappedAction == o.WrappedAction;
    }
}

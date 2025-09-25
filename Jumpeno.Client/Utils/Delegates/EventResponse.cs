namespace Jumpeno.Client.Utils;

#pragma warning disable CS1998

public class EventResponse<T, R> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static EventResponse<T, R> EMPTY(R response) => new(v => response);

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly Func<T, R>? WrappedAction;
    private readonly Func<T, Task<R>> Action;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public EventResponse(Func<T, R> action) {
        WrappedAction = action;
        Action = async data => action(data);
    }

    public EventResponse(Func<T, Task<R>> action) {
        WrappedAction = null;
        Action = action;
    }

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static Func<T, Task<R>> Task(Func<T, R> action) => value => {
        return System.Threading.Tasks.Task.FromResult(action(value));
    };

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public async Task<R> Invoke(T data) => await Action(data);

    public bool Equals(EventResponse<T, R> o) {
        return WrappedAction is null ? Action == o.Action : WrappedAction == o.WrappedAction;
    }
}

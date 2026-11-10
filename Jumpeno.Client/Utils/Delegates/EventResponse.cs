namespace Jumpeno.Client.Utils;

#pragma warning disable CS1998

public class EventResponse<T, TR> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static EventResponse<T, TR> Empty(TR response) => new(v => response);

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly Func<T, TR>? WrappedAction;
    private readonly Func<T, Task<TR>> Action;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public EventResponse(Func<T, TR> action) {
        WrappedAction = action;
        Action = async data => action(data);
    }

    public EventResponse(Func<T, Task<TR>> action) {
        WrappedAction = null;
        Action = action;
    }

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static Func<T, Task<TR>> Task(Func<T, TR> action) => value => {
        return System.Threading.Tasks.Task.FromResult(action(value));
    };

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public async Task<TR> Invoke(T data) => await Action(data);

    public bool Equals(EventResponse<T, TR> o) {
        return WrappedAction is null ? Action == o.Action : WrappedAction == o.WrappedAction;
    }
}

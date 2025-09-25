namespace Jumpeno.Client.Utils;

#pragma warning disable CS1998

public class EventDelegate<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static readonly EventDelegate<T> EMPTY = new(v => {});

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly Action<T>? WrappedAction;
    private readonly Func<T, Task> Action;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public EventDelegate(Action<T> action) {
        WrappedAction = action;
        Action = async data => action(data);
    }

    public EventDelegate(Func<T, Task> action) {
        WrappedAction = null;
        Action = action;
    }

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static Func<T, Task> Task(Action<T> action) => value => {
        action(value);
        return System.Threading.Tasks.Task.CompletedTask;
    };

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public async Task Invoke(T data) => await Action(data);

    public bool Equals(EventDelegate<T> o) {
        return WrappedAction is null ? Action == o.Action : WrappedAction == o.WrappedAction;
    }
}

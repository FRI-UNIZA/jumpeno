namespace Jumpeno.Client.Utils;

#pragma warning disable CS1998

public class EventPredicate<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static EventPredicate<T> EMPTY(bool result) => new(v => result);

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly Func<T, bool>? WrappedAction;
    private readonly Func<T, Task<bool>> Action;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public EventPredicate(Func<T, bool> action) {
        WrappedAction = action;
        Action = async data => action(data);
    }

    public EventPredicate(Func<T, Task<bool>> action) {
        WrappedAction = null;
        Action = action;
    }

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static Func<T, Task<bool>> Task(Func<T, bool> action) => value => {
        return System.Threading.Tasks.Task.FromResult(action(value));
    };

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public async Task<bool> Invoke(T data) => await Action(data);

    public bool Equals(EventPredicate<T> o) {
        return WrappedAction is null ? Action == o.Action : WrappedAction == o.WrappedAction;
    }
}

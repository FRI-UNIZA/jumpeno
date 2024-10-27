namespace Jumpeno.Shared.Delegates;

#pragma warning disable CS1998

public class EventDelegate<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static readonly EventDelegate<T> EMPTY = new(v => {});

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly Action<T>? WrappedAction;
    private readonly Func<T, Task> Action;

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public EventDelegate(Action<T> action) {
        WrappedAction = action;
        Action = async (T data) => action(data);
    }

    public EventDelegate(Func<T, Task> action) {
        WrappedAction = null;
        Action = action;
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public async Task Invoke(T data) {
        await Action(data);
    }

    public bool Equals(EventDelegate<T> o) {
        return WrappedAction is null ? Action == o.Action : WrappedAction == o.WrappedAction;
    }
}

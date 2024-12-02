namespace Jumpeno.Shared.Utils;

#pragma warning disable CS1998

public class EventPredicate<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static readonly EventPredicate<T> EMPTY = new(v => true);

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly Func<T, bool>? WrappedAction;
    private readonly Func<T, Task<bool>> Action;

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public EventPredicate(Func<T, bool> action) {
        WrappedAction = action;
        Action = async (T data) => action(data);
    }

    public EventPredicate(Func<T, Task<bool>> action) {
        WrappedAction = null;
        Action = action;
    }

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static Func<T, Task<bool>> Task(Func<T, bool> action) => (T value) => {
        return System.Threading.Tasks.Task.FromResult(action(value));
    };

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public async Task<bool> Invoke(T data) {
        return await Action(data);
    }

    public bool Equals(EventPredicate<T> o) {
        return WrappedAction is null ? Action == o.Action : WrappedAction == o.WrappedAction;
    }
}

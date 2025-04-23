namespace Jumpeno.Shared.Utils;

#pragma warning disable CS1998

public class EmptyPredicate {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static EmptyPredicate EMPTY(bool result) => new(() => result);

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly Func<bool>? WrappedAction;
    private readonly Func<Task<bool>> Action;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public EmptyPredicate(Func<bool> action) {
        WrappedAction = action;
        Action = async () => action();
    }

    public EmptyPredicate(Func<Task<bool>> action) {
        WrappedAction = null;
        Action = action;
    }

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static Func<Task<bool>> Task(Func<bool> action) => () => {
        return System.Threading.Tasks.Task.FromResult(action());
    };

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public async Task<bool> Invoke() => await Action();

    public bool Equals(EmptyPredicate o) {
        return WrappedAction is null ? Action == o.Action : WrappedAction == o.WrappedAction;
    }
}

namespace Jumpeno.Shared.Utils;

#pragma warning disable CS1998

public class EmptyDelegate {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static readonly EmptyDelegate EMPTY = new(() => {});

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly Action? WrappedAction;
    private readonly Func<Task> Action;

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public EmptyDelegate(Action action) {
        WrappedAction = action;
        Action = async () => action();
    }

    public EmptyDelegate(Func<Task> action) {
        WrappedAction = null;
        Action = action;
    }

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static Func<Task> Task(Action action) => () => {
        action();
        return System.Threading.Tasks.Task.CompletedTask;
    };

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public async Task Invoke() {
        await Action();
    }

    public bool Equals(EmptyDelegate o) {
        return WrappedAction is null ? Action == o.Action : WrappedAction == o.WrappedAction;
    }
}

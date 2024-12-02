namespace Jumpeno.Shared.Utils;

public class Delay {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    protected readonly EmptyDelegate Action;
    protected readonly int Time;
    protected readonly CancellationTokenSource TokenSource;

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    protected Delay(EmptyDelegate action, int time) {
        Action = action;
        Time = time;
        TokenSource = new CancellationTokenSource();
    }

    // Instance methods -------------------------------------------------------------------------------------------------------------------
    protected async Task ExecuteTimeout() {
        try {
            await Task.Delay(Time, TokenSource.Token);
            await Action.Invoke();
        } catch (TaskCanceledException) {
        } catch (Exception e) {
            if (AppEnvironment.IsDevelopment) Console.Error.WriteLine(e);
        }
    }

    protected virtual async void StartExecution() {
        await ExecuteTimeout();
    }

    // Static methods ---------------------------------------------------------------------------------------------------------------------
    protected static Delay Set(EmptyDelegate action, int time) {
        var timeDelay = new Delay(action, time);
        timeDelay.StartExecution();
        return timeDelay;
    }
    public static Delay Set(Func<Task> action, int time) {
        return Set(new EmptyDelegate(action), time);
    }
    public static Delay Set(Action action, int time) {
        return Set(new EmptyDelegate(action), time);
    }

    public static void Clear(Delay? delay) {
        if (delay is null) return;
        delay.TokenSource.Cancel();
    }
}

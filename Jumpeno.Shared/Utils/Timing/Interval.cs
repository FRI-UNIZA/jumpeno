namespace Jumpeno.Shared.Utils;

public class Interval(EmptyDelegate action, int time) : Delay(action, time) {
    // Instance methods -------------------------------------------------------------------------------------------------------------------
    protected override async void StartExecution() {
        do await ExecuteTimeout();
        while (!TokenSource.IsCancellationRequested);
    }

    // Static methods ---------------------------------------------------------------------------------------------------------------------
    protected static new Interval Set(EmptyDelegate action, int time) {
        var timeDelay = new Interval(action, time);
        timeDelay.StartExecution();
        return timeDelay;
    }
    public static new Interval Set(Func<Task> action, int time) {
        return Set(new EmptyDelegate(action), time);
    }
    public static new Interval Set(Action action, int time) {
        return Set(new EmptyDelegate(action), time);
    }

    public static void Clear(Interval? interval) {
        if (interval is null) return;
        interval.TokenSource.Cancel();
    }
}

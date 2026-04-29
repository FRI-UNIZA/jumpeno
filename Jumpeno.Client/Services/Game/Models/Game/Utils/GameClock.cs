namespace Jumpeno.Client.Utils;

public class GameClock(int fps) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public readonly int FPS = fps;
    public readonly int Interval = 1000 / fps; // ms

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private DateTime Time = DateTime.UtcNow;

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static double Delta(DateTime time1, DateTime time2) => Math.Abs((time1 - time2).TotalMilliseconds);

    public static double DeltaAhead(DateTime time) => Math.Max((DateTime.UtcNow - time).TotalMilliseconds, 0);

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public void Set(DateTime time) => Time = time;

    public void Reset() => Set(DateTime.UtcNow);

    public void Update(double deltaT) => Time = Time.AddMilliseconds(deltaT);

    public double ComputeDelta() => (DateTime.UtcNow - Time).TotalMilliseconds;

    // Await ------------------------------------------------------------------------------------------------------------------------------
    // NOTE: Must run under lock:
    private async Task<double> AwaitDeltaT((Func<Task> Lock, Action Unlock)? locker) {
        while (true) {
            // 1) Compute delta:
            var deltaT = ComputeDelta();
            // 2) Check valid value:
            if (deltaT <= Interval) {
                locker?.Unlock();
                await Task.Delay(Interval - (int) Math.Ceiling(deltaT));
                if (locker != null) await (locker?.Lock!)();
                continue;
            }
            // 3) Update time:
            Update(deltaT);
            // 4) Return delta:
            return deltaT;
        }
    }
    public async Task<double> AwaitDelta() => await AwaitDeltaT(null);
    public async Task<double> AwaitDelta(LockerSlim locker) => await AwaitDeltaT((locker.Lock, locker.TryUnlock));
    public async Task<double> AwaitDelta(Locker locker) => await AwaitDeltaT((() => { locker.Lock(); return Task.CompletedTask; }, locker.TryUnlock));
}

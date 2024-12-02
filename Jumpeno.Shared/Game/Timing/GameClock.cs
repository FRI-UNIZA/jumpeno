namespace Jumpeno.Shared.Utils;

public class GameClock {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private DateTime Time;
    public int FPS { get; private set; }
    public int IntervalMS { get; private set; }

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    private static int ComputeInterval(int fps) => 1000 / fps;

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public GameClock(int fps) {
        Time = DateTime.UtcNow;
        FPS = fps;
        IntervalMS = ComputeInterval(FPS);
    }

    // Helpers ----------------------------------------------------------------------------------------------------------------------------
    public static double Delta(DateTime time1, DateTime time2) => Math.Abs((time1 - time2).TotalMilliseconds);
    
    public static double DeltaAhead(DateTime time) => Math.Max((DateTime.UtcNow - time).TotalMilliseconds, 0);

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public void Set(DateTime time) => Time = time;
    
    public void Reset() => Set(DateTime.UtcNow);
    
    public void Update(double deltaT) => Time = Time.AddMilliseconds(deltaT);
    
    public double ComputeDelta() => (DateTime.UtcNow - Time).TotalMilliseconds;
    
    public async Task<double> AwaitDelta() {
        while (true) {
            // 1) Compute delta:
            var deltaT = ComputeDelta();
            // 2) Check valid value:
            if (deltaT <= IntervalMS) {
                await Task.Delay(IntervalMS - (int) Math.Ceiling(deltaT));
                continue;
            }
            // 3) Update time:
            Update(deltaT);
            // 4) Return delta:
            return deltaT;
        }
    }
}

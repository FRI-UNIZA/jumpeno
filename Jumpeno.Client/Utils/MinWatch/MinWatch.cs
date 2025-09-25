namespace Jumpeno.Client.Utils;

using System.Diagnostics;

public class MinWatch(int ms = 0) {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public int MS { get; private set; } = ms;
    // Watch:
    private readonly Stopwatch watch = new();

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public MinWatch(uint ms) : this((int)ms) {}

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public void Start(int? ms = null) {
        if (ms != null) MS = (int)ms;
        watch.Restart();
    }
    public void Start(uint ms) => Start((int?)ms);

    public Task Task { get {
        if (!watch.IsRunning) return Task.CompletedTask;
        watch.Stop();
        var ellapsedMS = (int)watch.ElapsedMilliseconds;
        if (ellapsedMS >= MS) return Task.CompletedTask;
        return Task.Delay(MS - ellapsedMS);
    }}
}

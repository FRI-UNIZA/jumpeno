namespace Jumpeno.Client.Models;

public class TimeFlowUpdate(double deltaT) : PartialUpdate {
    public double DeltaT { get; private set; } = deltaT;

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}

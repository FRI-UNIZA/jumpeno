namespace Jumpeno.Shared.Models;

public abstract class GameUpdate(ulong id) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public abstract string HUB_ACTION { get; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public ulong ID { get; private set; } = id;
}

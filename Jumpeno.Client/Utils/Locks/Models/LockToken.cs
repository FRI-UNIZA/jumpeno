namespace Jumpeno.Client.Models;

public class LockToken(Action unlock) {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public bool Locked { get; private set; } = true;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public void Unlock() {
        if (Locked) unlock();
        Locked = false;
    }
}

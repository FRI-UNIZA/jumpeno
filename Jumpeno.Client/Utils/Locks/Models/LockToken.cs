namespace Jumpeno.Client.Models;

public class LockToken(Action unlock, Action tryUnlock) {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public bool Locked { get; private set; } = true;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public void Unlock() {
        if (Locked) unlock();
        Locked = false;
    }

    public void TryUnlock() {
        if (Locked) tryUnlock();
        Locked = false;
    }
}

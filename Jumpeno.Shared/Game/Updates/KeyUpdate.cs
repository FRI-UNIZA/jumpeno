namespace Jumpeno.Shared.Models;

public class KeyUpdate(Guid id, GAME_CONTROLS key, bool pressed) {
    public double Time { get; private set; }
    public Guid ID { get; private set; } = id;
    public GAME_CONTROLS Key { get; private set; } = key;
    public bool Pressed { get; private set; } = pressed;
}

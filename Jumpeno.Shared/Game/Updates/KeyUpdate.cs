namespace Jumpeno.Shared.Models;

public class KeyUpdate(byte id, GAME_CONTROLS key, bool pressed) {
    public double Time { get; private set; }
    public byte ID { get; private set; } = id;
    public GAME_CONTROLS Key { get; private set; } = key;
    public bool Pressed { get; private set; } = pressed;
}

namespace Jumpeno.Shared.Models;

public class PlayerUpdate(byte id, bool alive, Point position, Point direction) {
    public byte ID { get; private set; } = id;
    public bool Alive { get; private set; } = alive;
    public Point Position { get; private set; } = position;
    public Point Direction { get; private set; } = direction;
}

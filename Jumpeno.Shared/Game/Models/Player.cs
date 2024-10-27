namespace Jumpeno.Shared.Models;

public class Player(byte id, string name, SKIN skin, bool alive, Point position, Point direction) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const int WIDTH = 64;
    public const int HEIGHT = 76;
    public const double SPEED = 20; // m per second

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public byte ID { get; private set; } = id;
    public string Name { get; private set; } = name;
    public SKIN Skin { get; private set; } = skin;
    public bool Alive { get; private set; } = alive;
    public Point Position { get; private set; } = position;
    public Point Direction { get; private set; } = direction;
}

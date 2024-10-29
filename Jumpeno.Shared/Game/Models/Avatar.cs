namespace Jumpeno.Shared.Models;

public class Avatar {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const int WIDTH = 64;
    public const int HEIGHT = 76;
    public const double SPEED = 20; // m per second

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public bool Alive { get; set; }
    public Point Position { get; set; }
    public Point Direction { get; set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private Avatar(bool alive, Point position, Point direction) {
        Alive = alive;
        Position = position;
        Direction = direction;
    }
    public Avatar(): this(false, new(0, 0), new(0, 0)) {}
}

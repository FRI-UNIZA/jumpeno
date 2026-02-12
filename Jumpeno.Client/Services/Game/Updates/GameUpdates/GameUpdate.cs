namespace Jumpeno.Client.Models;

public abstract class GameUpdate {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    // NOTE: Valid after goes through game update method:
    [JsonIgnore] public Game Game { get; set; } = null!;
}

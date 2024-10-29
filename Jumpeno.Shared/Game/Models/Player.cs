namespace Jumpeno.Shared.Models;

public class Player(string connectionID, User user, Avatar avatar) {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public string ConnectionID { get; private set; } = connectionID;
    public User User { get; private set; } = user;
    public Avatar Avatar { get; private set; } = avatar;

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public bool Equals(Player player) {
        return ConnectionID == player.ConnectionID;
    }
}

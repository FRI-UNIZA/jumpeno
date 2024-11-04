namespace Jumpeno.Shared.Models;

public class Player: IUpdateAble {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public byte ID { get; private set; }
    public User? User { get; private set; }
    public Avatar Avatar { get; private set; }
    public bool Connected { get; private set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private Player(byte id, User? user, Avatar avatar, bool connected) {
        ID = id;
        User = user;
        Avatar = avatar;
        Connected = connected;
    }
    
    public Player(byte id) : this(id, null, new(), false) {}

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    public bool Equals(Player player) {
        return ID == player.ID;
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public void ConnectUser(User user) {
        User = user;
        Connected = true;
    }

    public void DisconnectUser(User user) {
        // NOTE: User is remembered!
        Connected = false;
    }

    // Updates ----------------------------------------------------------------------------------------------------------------------------    
    public bool Update(GameUpdate update) {
        if (update is PlayerUpdate p) return PlayerUpdate(p);
        return false;
    }

    private readonly UpdateGuard<PlayerUpdate> PlayerUpdateGuard = new();
    private bool PlayerUpdate(PlayerUpdate update) {
        return PlayerUpdateGuard.Update(update, () => {
            User = update.Player.User;
            Connected = update.Action == PLAYER_ACTION.JOIN;
        });
    }
}

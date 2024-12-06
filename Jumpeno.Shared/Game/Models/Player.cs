namespace Jumpeno.Shared.Models;

public class Player : IRectFQuadStorable, IUpdateable, IRenderableParametric<Game> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string TOUCH_DEVICE_ID = "touch-device";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public byte ID { get; private set; }
    public User? User { get; private set; }
    public Body Body { get; private set; }
    public bool Connected { get; private set; }
    public bool TouchDevice { get; private set; }
    public int Kills { get; private set; }
    public RectangleF Rect => Body.Rect;

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private Player(byte id, User? user, Body body, bool connected, bool touchDevice, int kills) {
        ID = id;
        User = user;
        Body = body;
        Connected = connected;
        TouchDevice = touchDevice;
        Kills = kills;
    }

    public Player(byte id) : this(id, null, new(), false, false, 0) {}

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    public bool Equals(Player player) => ID == player.ID;
    public bool IsJumping => Body.IsJumping;
    public bool JumpedOn(Player player) => Body.JumpedOn(player.Body);
    public bool CollisionDetected => Body.CollisionDetected;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public void ConnectUser(User user, bool touchDevice = false) {
        User = user;
        Connected = true;
        TouchDevice = touchDevice;
    }

    public void ForgetUser() {
        User = null;
    }

    // Updates ----------------------------------------------------------------------------------------------------------------------------    
    public bool Update(GameUpdate update) {
        if (update is TimeFlowUpdate time) return TimeFlowUpdate(time);
        if (update is KeyUpdate key) return KeyUpdate(key);
        if (update is GamePlayUpdate game) return GamePlayUpdate(game);
        if (update is KillUpdate kill) return KillUpdate(kill);
        if (update is LifeUpdate life) return LifeUpdate(life);
        if (update is PlayerUpdate player) return PlayerUpdate(player);
        return false;
    }

    private bool TimeFlowUpdate(TimeFlowUpdate update) {
        return Body.Update(update);
    }

    private bool KeyUpdate(KeyUpdate update) {
        // NOTE: Has update guard:
        return Body.Update(update);
    }

    private readonly UpdateGuard<GamePlayUpdate> GamePlayUpdateGuard = new();
    private bool GamePlayUpdate(GamePlayUpdate update) {
        update.Movements.TryGetValue(ID, out var movementUpdate);
        if (movementUpdate == null) return false;
        return GamePlayUpdateGuard.Update(update, () => {
            return Body.Update(movementUpdate); 
        });
    }

    private bool KillUpdate(KillUpdate update) {
        if (update.DeadID == ID) return Body.Update(update);
        else if (update.KillerID == ID) { Kills++; return true; }
        return false;
    }

    private bool LifeUpdate(LifeUpdate update) {
        if (update.PlayerID != ID) return false;
        return Body.Update(update);
    }

    private readonly UpdateGuard<PlayerUpdate> PlayerUpdateGuard = new();
    private bool PlayerUpdate(PlayerUpdate update) {
        return PlayerUpdateGuard.Update(update, () => {
            User = update.Player.User;
            Connected = update.Action == PLAYER_ACTION.JOIN;
        });
    }

    public void ResetUpdateGuards() {
        GamePlayUpdateGuard.Reset();
        PlayerUpdateGuard.Reset();
        Body.ResetUpdateGuards();
    }

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public async Task Render(Canvas2DContext ctx, Game game) {
        await Body.Render(ctx, (game, User?.Skin));
    }
}

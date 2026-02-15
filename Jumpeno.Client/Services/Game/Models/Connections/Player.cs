namespace Jumpeno.Client.Models;

public class Player : Connection, IRectFQuadStorable, IUpdateable, IRenderable<Game> {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public byte ID { get; private set; }
    public Body Body { get; private set; }
    public int Score { get; private set; }
    public int ReadyForRound { get; private set; }
    public RectangleF Rect => Body.Rect;

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    public bool Equals(Player? player) => ID == player?.ID;
    public static bool IsValid(Player? player) => !User.UNKNOWN.Equals(player?.User);
    public bool IsValid() => IsValid(this);
    public bool IsJumping => Body.IsJumping;
    public bool JumpedOn(Player player) => Body.JumpedOn(player.Body);
    public bool CollisionDetected => Body.CollisionDetected;
    public bool IsShrinked(Shrink shrink) => Body.IsShrinked(shrink);
    public bool IsAlive => Body.Alive;
    public bool IsReady(Game game) => game.IsPlayerReady(this);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private Player(
        string? connectionID, User user, DEVICE_TYPE device,
        byte id, Body body, int score, int readyForRound
    ) : base(connectionID, user, device) {
        ID = id;
        Body = body;
        Score = score;
        ReadyForRound = readyForRound;
    }

    public Player(byte id) : this(
        null, User.UNKNOWN, DEVICE_TYPE.POINTER,
        id, new(), 0, 0
    ) {}

    public Player(Player player) : this(
        player.ConnectionID, player.User, player.Device,
        player.ID, player.Body, player.Score, player.ReadyForRound
    ) {}

    private void Invalidate() {
        Synchronize(null, User.UNKNOWN, DEVICE_TYPE.POINTER);
        // NOTE: Body is preserved!
        Score = 0;
        ReadyForRound = 0;
    }

    // Updates ----------------------------------------------------------------------------------------------------------------------------
    public bool Update(GameUpdate update)
    => update switch {
        TimeFlowUpdate time => TimeFlowUpdate(time),
        KeyUpdate key => KeyUpdate(key),
        GamePlayUpdate game => GamePlayUpdate(game),
        MovementUpdate move => MovementUpdate(move),
        KillUpdate kill => KillUpdate(kill),
        LifeUpdate life => LifeUpdate(life),
        PlayerUpdate player => PlayerUpdate(player),
        StateUpdate state => StateUpdate(state),
        RoundUpdate round => RoundUpdate(round),
        _ => false
    };

    private bool TimeFlowUpdate(TimeFlowUpdate update) => Body.Update(update);

    private bool KeyUpdate(KeyUpdate update) => Body.Update(update); // NOTE: Has update guard:

    private readonly UpdateGuard<GamePlayUpdate> GamePlayMoveUpdateGuard = new();
    private readonly UpdateGuard<GamePlayUpdate> GamePlayAliveUpdateGuard = new();
    private bool GamePlayUpdate(GamePlayUpdate update) {
        // 1) Initialize response:
        var response = new GamePlayResponse();
        // 2) Update movements:
        if (update.Movements.TryGetValue(ID, out var moveUpdate)) {
            GamePlayMoveUpdateGuard.Update(update, () => {
                response.MoveUpdated = MovementUpdate(moveUpdate);
            });
        }
        // 3) Update kills and lives:
        update.Kills.TryGetValue(ID, out var killUpdate);
        update.Lives.TryGetValue(ID, out var lifeUpdate);
        if (killUpdate != null || lifeUpdate != null) {
            GamePlayAliveUpdateGuard.Update(update, () => {
                if (killUpdate != null) response.KillUpdated = KillUpdate(killUpdate);
                if (lifeUpdate != null) response.LifeUpdated = LifeUpdate(lifeUpdate);
            });
        }
        // 4) Return response:
        update.Response = response;
        return response.Updated;
    }

    private bool MovementUpdate(MovementUpdate update) => Body.Update(update);

    private bool KillUpdate(KillUpdate update) {
        if (update.DeadID == ID) {
            var updated = Body.Update(update);
            if (!update.Penalize) return updated;
            Score = Math.Max(0, Score - 1);
            return true;
        } else if (update.KillerID == ID) {
            Score++;
            return true;
        }
        return false;
    }

    private bool LifeUpdate(LifeUpdate update) {
        if (update.PlayerID != ID) return false;
        return Body.Update(update);
    }

    private readonly UpdateGuard<PlayerUpdate> PlayerUpdateGuard = new();
    private bool PlayerUpdate(PlayerUpdate update)
    => PlayerUpdateGuard.Update(update, () => {
        if (update.Invalidate) { Invalidate(); return; }
        Synchronize(update.Player);
        ReadyForRound = update.ReadyForRound;
    });

    private bool StateUpdate(StateUpdate update) => Body.Update(update);

    private bool RoundUpdate(RoundUpdate update) {
        if (!update.Players.TryGetValue(ID, out var player)) return false;
        Body = player.Body;
        Score = player.Score;
        if (User.ID == update.Game.Host.ID) ReadyForRound = update.Game.Round + 1;
        return true;
    }

    public void ResetUpdateGuards() {
        GamePlayMoveUpdateGuard.Reset();
        GamePlayAliveUpdateGuard.Reset();
        PlayerUpdateGuard.Reset();
        Body.ResetUpdateGuards();
    }

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public async Task<bool> Render(Canvas2DContext ctx, Game game) => await Body.Render(ctx, (game, User.Skin));
}

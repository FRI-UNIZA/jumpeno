namespace Jumpeno.Client.Models;

public class Player : Connection, IRectFQuadStorable, IUpdateable, IRenderable<Game> {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public byte ID { get; private set; }
    public Body Body { get; private set; }
    public int Score { get; private set; }
    public RectangleF Rect => Body.Rect;

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    public bool Equals(Player? player) => ID == player?.ID;
    public bool IsJumping => Body.IsJumping;
    public bool JumpedOn(Player player) => Body.JumpedOn(player.Body);
    public bool CollisionDetected => Body.CollisionDetected;
    public bool IsShrinked(Shrink shrink) => Body.IsShrinked(shrink);
    public bool IsAlive => Body.Alive;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private Player(string? connectionID, User user, DEVICE_TYPE device, byte id, Body body, int score) : base(connectionID, user, device) {
        ID = id;
        Body = body;
        Score = score;
    }
    public Player(byte id) : this(null, User.UNKNOWN, DEVICE_TYPE.POINTER, id, new(), 0) {}
    
    // Utils ------------------------------------------------------------------------------------------------------------------------------
    private void ResolveJump(Game game) {
        foreach (var other in game.GetCollidingPlayers(this)) {
            // 1) Check self:
            if (Equals(other)) continue;
            // 2) Check jump:
            if (!JumpedOn(other)) continue;
            // 3) Movement update:
            Update(game.NewMovementUpdate(this, other));
        }
    }

    // Updates ----------------------------------------------------------------------------------------------------------------------------    
    public bool Update(GameUpdate update) {
        if (update is TimeFlowUpdate time) return TimeFlowUpdate(time);
        if (update is KeyUpdate key) return KeyUpdate(key);
        if (update is GamePlayUpdate game) return GamePlayUpdate(game);
        if (update is MovementUpdate move) return MovementUpdate(move);
        if (update is KillUpdate kill) return KillUpdate(kill);
        if (update is LifeUpdate life) return LifeUpdate(life);
        if (update is PlayerUpdate player) return PlayerUpdate(player);
        if (update is StateUpdate state) return StateUpdate(state);
        if (update is RoundUpdate round) return RoundUpdate(round);
        return false;
    }

    private bool TimeFlowUpdate(TimeFlowUpdate update) {
        return Body.Update(update);
    }

    private bool KeyUpdate(KeyUpdate update) {
        // NOTE: Has update guard:
        return Body.Update(update);
    }

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
                if (lifeUpdate != null) {
                    response.LifeUpdated = LifeUpdate(lifeUpdate);
                    if (response.LifeUpdated) Body = lifeUpdate.Player.Body;
                }
            });
        }
        // 4) Return response:
        update.Response = response;
        return response.Updated;
    }

    private bool MovementUpdate(MovementUpdate update) {
        return Body.Update(update);
    }

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
        if (update.Player.ID != ID) return false;
        return Body.Update(update);
    }

    private readonly UpdateGuard<PlayerUpdate> PlayerUpdateGuard = new();
    private bool PlayerUpdate(PlayerUpdate update) {
        return PlayerUpdateGuard.Update(update, () => Synchronize(update.Player));
    }

    private bool StateUpdate(StateUpdate update) {
        return Body.Update(update);
    }

    private bool RoundUpdate(RoundUpdate update) {
        if (!update.Players.TryGetValue(ID, out var player)) return false;
        Body = player.Body;
        Score = player.Score;
        return true;
    }

    public void ResetUpdateGuards() {
        GamePlayMoveUpdateGuard.Reset();
        GamePlayAliveUpdateGuard.Reset();
        PlayerUpdateGuard.Reset();
        Body.ResetUpdateGuards();
    }

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public async Task<bool> Render(Canvas2DContext ctx, Game game) {
        ResolveJump(game);
        return await Body.Render(ctx, (game, User.Skin));
    }
}

namespace Jumpeno.Client.Models;

public class Body : IRectFPositionable, IUpdateable, IRenderable<(Game Game, Skin Skin)> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const double IMMORTAL_MS = 2000; // ms
    // Size:
    public const int WIDTH = 50; // px
    public const int HEIGHT = 63; // px
    // Speed:
    public const float SPEED = 0.38f; // px per ms
    // Jump:
    public const float JUMP_HEIGHT = 160f; // px
    public const float JUMP_SPEED = 0.95f; // px per ms (at the start)
    public const float JUMP_SPEED_BASE = 0.2f; // minimal fraction of JUMP_SPEED
    public const float JUMP_SPEED_MAX = 1.3f; // px per ms
    public const double PENDING_JUMP_TIMEOUT = 150; // ms

    // Computed constants -----------------------------------------------------------------------------------------------------------------
    public const int HALF_WIDTH = WIDTH / 2;
    public const int HALF_HEIGHT = HEIGHT / 2;
    public static readonly PointF DEFAULT_POSITION = new(0, 0);
    public static readonly PointF DEFAULT_DIRECTION = new(0, -1);
    public static readonly PointF DEFAULT_NORMAL = new(0, 1);

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    // Health:
    public bool Alive { get; private set; }
    public bool Fallen { get; private set; }
    public bool IsImmortal { get; private set; }
    public double ImmortalUntil { get; private set; }
    // Position:
    public RectFPosition LastPosition { get; private set; }
    public RectFPosition Position { get { return position; } private set { position = value; } } private RectFPosition position;
    [JsonInclude] private PointF Center => position.Center;
    public RectangleF Rect => Collision.GetBoundingBox(Position);
    // Direction:
    public PointF Direction { get { return direction; } private set { direction = value; } } private PointF direction;
    // Jump:
    private (KeyUpdate? Update, DateTime Time) PendingJump = (null, DateTime.UtcNow);
    public float? JumpFinishY { get; private set; }
    public bool IsJumping => JumpFinishY != null;
    public float? FallStartY { get; private set; } = null;
    // Collision (normal vector):
    public PointF LastNormal { get; private set; }
    public PointF Normal { get; private set; }
    // Animation:
    public Animation Animation { get; private set; }

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    public bool JumpedOn(Body body) {
        var bodyRect = body.Rect; bodyRect.Inflate(1, 1);
        return Alive && body.Alive && !IsImmortal && !body.IsImmortal
        && Direction.Y < 0 && Normal.Y <= 0
        && bodyRect.IntersectsWith(Rect)
        && (FallStartY == null || (FallStartY - Position.Center.Y > Tile.HALF_SIZE))
        && (LastPosition.Center.Y - HALF_HEIGHT >= body.LastPosition.Center.Y + HALF_HEIGHT)
        && (Position.Center.Y - HALF_HEIGHT <= body.Position.Center.Y + HALF_HEIGHT);
    }
    
    public bool CollisionDetected => !(Normal.Equals(Collision.ZERO_VECTOR) || Normal.Equals(LastNormal));
    
    public bool IsShrinked(Shrink shrink) {
        if (!Alive) return false;
        var rect = shrink.Rect;
        return Center.X - HALF_WIDTH < rect.X - 1 || rect.X + rect.Width + 1 < Center.X + HALF_WIDTH;
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private Body(
        bool alive, bool fallen,
        bool isImmortal, double immortalUntil,
        PointF center, PointF direction, float? jumpFinishY,
        Animation animation
    ) {
        Alive = alive;
        Fallen = fallen;
        IsImmortal = isImmortal;
        ImmortalUntil = immortalUntil;
        Position = new(center, WIDTH, HEIGHT);
        LastPosition = Position;
        Direction = direction;
        JumpFinishY = jumpFinishY;
        LastNormal = DEFAULT_NORMAL;
        Normal = DEFAULT_NORMAL;
        Animation = animation;
    }
    public Body() : this(false, false, false, 0, DEFAULT_POSITION, DEFAULT_DIRECTION, null, new(DEFAULT_DIRECTION)) {}

    // Movement ---------------------------------------------------------------------------------------------------------------------------
    private void ChangeDirection(GameControls key, bool pressed) {
        if (!pressed) direction.X = 0;
        else direction.X = key == GameControls.Left ? -1 : 1;
    }

    private float ComputeNextX(double deltaT) => (float)(Center.X + deltaT * Direction.X * SPEED);

    private float ComputeNextY(double deltaT) {
        if (JumpFinishY == null) return Center.Y;
        var jumpSpeed = Math.Min((double)
            (JUMP_SPEED_BASE + (JumpFinishY - (Center.Y + HALF_HEIGHT)) / JUMP_HEIGHT) * JUMP_SPEED,
            JUMP_SPEED_MAX // NOTE: Gravity restriction
        );
        return (float)(Center.Y + deltaT * Direction.Y * jumpSpeed);
    }

    private PointF ComputeNextCenter(double deltaT) => new(ComputeNextX(deltaT), ComputeNextY(deltaT));

    // Jump -------------------------------------------------------------------------------------------------------------------------------
    private void ApplyDeathFall(Game game) {
        // 1) Compute:
        var pointTop = Mark.CalculateMarkPointTop(this);
        var halfHeight = pointTop.Y - position.Center.Y;
        var isUnderMap = pointTop.Y <= game.Map.WorldMinY;
        // 2) Apply:
        Fallen = isUnderMap || Fallen;
        Normal = Collision.ZERO_VECTOR;
        position.Center.Y = isUnderMap ? game.Map.WorldMinY - halfHeight : position.Center.Y;
        direction.X = 0;
        direction.Y = isUnderMap ? 0 : -1 ;
        JumpFinishY = Center.Y + HALF_HEIGHT + JUMP_HEIGHT;
    }
    
    private void StartFall() {
        direction.Y = -1;
        JumpFinishY = Center.Y + HALF_HEIGHT + JUMP_HEIGHT * 0.3f;
        FallStartY = Position.Center.Y;
    }

    private void StartJump() {
        direction.Y = 1;
        JumpFinishY = Center.Y + HALF_HEIGHT + JUMP_HEIGHT;
    }

    private void ReverseJump() {
        direction.Y = -1;
    }

    private void FinishJump() {
        direction.Y = -1;
        JumpFinishY = null;
        FallStartY = null;
    }

    // Collision resolution ---------------------------------------------------------------------------------------------------------------
    private void SaveNormal(PointF normal) {
        Normal = new(
            Normal.X == 0 ? normal.X : Normal.X,
            Normal.Y == 0 ? normal.Y : Normal.Y
        );
    }

    private void ResolveCollision((RectFPosition Resolved, PointF Normal) fix) {
        Position = fix.Resolved;
        if (fix.Normal.Y < 0) ReverseJump();
        else if (fix.Normal.Y > 0) FinishJump();
        SaveNormal(fix.Normal);
    }

    // Updates ----------------------------------------------------------------------------------------------------------------------------
    public bool Update(GameUpdate update)
    => update switch {
        TimeFlowUpdate time => TimeFlowUpdate(time),
        KeyUpdate key => KeyUpdate(key),
        MovementUpdate move => MovementUpdate(move),
        KillUpdate kill => KillUpdate(kill),
        LifeUpdate life => LifeUpdate(life),
        StateUpdate state => StateUpdate(state),
        _ => false
    };

    private bool TimeFlowUpdate(TimeFlowUpdate update) {
        // 1) Check delta:
        if (update.DeltaT <= 0) return false;
        if (Alive && update.Game.Map.Shrink.Rect.Width < WIDTH) return false;
        
        // 2) Save last & compute new position:
        LastPosition = Position;
        LastNormal = Normal;
        // 2.1) Reset normall & fall:
        Normal = Collision.ZERO_VECTOR;
        if (JumpFinishY == null) StartFall();
        // 2.2) Killed player:
        if (!Alive) ApplyDeathFall(update.Game);
        // 2.3) Move body:
        position.Center = ComputeNextCenter(update.DeltaT);

        // 3) Resolve collisions for alive body:
        if (!Alive) return true;
        // 3.1) Resolve map collisions:
        if (update.Game.Map.Shrink.Rect.Width > WIDTH)
            Collision.Resolve(update.Game.Map.Shrink.Rect, position, ResolveCollision);
        // 3.2) Resolve jump height:
        if (JumpFinishY != null)
            Collision.Resolve((float)JumpFinishY, PositionDir.Top, position, ResolveCollision);
        // 3.3) Resolve tile collisions:
        var moveBox = Collision.GetMoveBox(LastPosition, position);
        List<Tile> tiles = update.Game.Map.GetCollidingTiles(moveBox);
        Collision.Resolve(tiles, LastPosition, position, ResolveCollision);
        // TODO: [Optional] Implement jump reverse motion (will increase complexity!)
        
        // 4) Apply pending jump:
        if (Normal.Y == 1 && PendingJump.Update != null) {
            PendingJump.Update = null;
            if (GameClock.DeltaAhead(PendingJump.Time) <= PENDING_JUMP_TIMEOUT) StartJump();
        }

        // 5) Resolve immortality:
        IsImmortal = update.Game.Time < ImmortalUntil;

        // 6) Return result:
        return true;
    }

    private readonly UpdateGuard<KeyUpdate> KeyUpdateGuard = new();
    private bool KeyUpdate(KeyUpdate update) {
        var updated = false;
        if (!Alive) return updated;
        foreach (var control in update.Controls) {
            switch (control.Key) {
                case GameControls.Left:
                case GameControls.Right:
                    if (KeyUpdateGuard.Update(
                        update, () => ChangeDirection(control.Key, control.Pressed)
                    )) updated = true;
                break;
                case GameControls.Space:
                    if (IsJumping) {
                        PendingJump = (update, DateTime.UtcNow);
                        break;
                    }
                    StartJump();
                    updated = true;
                break;
            }
        }
        if (updated) Animation.UpdateDirection(Direction);
        return updated;
    }

    private bool MovementUpdate(MovementUpdate update) {
        position.Center = update.Center;
        direction = update.Direction;
        JumpFinishY = update.JumpFinishY;
        LastNormal = Normal = update.Normal;
        if (update.AnimationDirection is PointF dir) Animation.ResetDirection(dir);
        else Animation.UpdateDirection(Direction);
        return true;
    }

    private bool KillUpdate(KillUpdate update) {
        if (!Alive) return false;
        Alive = false;
        return true;
    }

    private bool LifeUpdate(LifeUpdate update) {
        if (Alive) return false;
        Alive = true;
        Fallen = false;
        IsImmortal = update.Game.Time < update.ImmortalUntil;
        ImmortalUntil = update.ImmortalUntil;
        return true;
    }

    private bool StateUpdate(StateUpdate update) {
        switch (update.State) {
            case GameStates.Pause:
                direction.X = 0;
                Animation.UpdateDirection(direction);
            return true;
        }
        return false;
    }

    public void ResetUpdateGuards() => KeyUpdateGuard.Reset();

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public async Task<bool> Render(Canvas2DContext ctx, (Game Game, Skin Skin) @params) {
        var (game, skin) = @params;
        return await Animation.Render(ctx, (game, skin, this));
    }
}

namespace Jumpeno.Shared.Models;

public class Body : IRectFPositionable, IUpdateable, IRenderableParametric<(Game Game, SKIN Skin)> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const int WIDTH = 51; // px
    public const int HEIGHT = 64; // px
    public const float SPEED = 0.38f; // px per ms
    public const float JUMP_HEIGHT = 180f; // px
    public const float JUMP_SPEED = 0.95f; // px per ms (at the start)
    public const double PENDING_JUMP_TIMEOUT = 150; // ms

    // Computed constants -----------------------------------------------------------------------------------------------------------------
    public const int HALF_WIDTH = WIDTH / 2;
    public const int HALF_HEIGHT = HEIGHT / 2;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    // Health:
    public bool Alive { get; private set; }
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
    // Collision (normal vector):
    public PointF LastNormal { get; private set; }
    public PointF Normal { get; private set; }
    // Animation:
    private Animation Animation { get; set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private Body(bool alive, PointF center, PointF direction, float? jumpFinishY) {
        Alive = alive;
        Position = new(center, WIDTH, HEIGHT);
        LastPosition = Position;
        Direction = direction;
        JumpFinishY = jumpFinishY;
        LastNormal = new(0, 1);
        Normal = new(0, 1);
        Animation = new(Direction);
    }
    public Body() : this(false, new(500, HALF_HEIGHT), new(0, -1), null) {}

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    public bool JumpedOn(Body body) =>
        Alive && body.Alive && Direction.Y < 0
        && body.Rect.IntersectsWith(Rect)
        && (LastPosition.Center.Y - HALF_HEIGHT >= body.LastPosition.Center.Y + HALF_HEIGHT)
        && (Position.Center.Y - HALF_HEIGHT <= body.Position.Center.Y + HALF_HEIGHT);

    public bool CollisionDetected => !(Normal.Equals(Collision.ZERO_VECTOR) || Normal.Equals(LastNormal));

    // Movement ---------------------------------------------------------------------------------------------------------------------------
    private void ChangeDirection(GAME_CONTROLS key, bool pressed) {
        if (!pressed) direction.X = 0;
        else direction.X = key == GAME_CONTROLS.LEFT ? -1 : 1;
    }

    private float ComputeNextX(double deltaT) {
        return (float) (Center.X + deltaT * Direction.X * SPEED);
    }
    
    private float ComputeNextY(double deltaT) {
        if (JumpFinishY == null) return Center.Y;
        var jumpSpeed = (0.2 + (JumpFinishY - (Center.Y + HALF_HEIGHT)) / JUMP_HEIGHT) * JUMP_SPEED;
        return (float) (Center.Y + deltaT * Direction.Y * jumpSpeed);
    }

    private PointF ComputeNextCenter(double deltaT) {
        return new PointF(ComputeNextX(deltaT), ComputeNextY(deltaT));
    }

    // Jump -------------------------------------------------------------------------------------------------------------------------------
    private void StartFall() {
        direction.Y = -1;
        JumpFinishY = Center.Y + HALF_HEIGHT + JUMP_HEIGHT * 0.3f;
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
    public bool Update(GameUpdate update) {
        if (update is TimeFlowUpdate time) return TimeFlowUpdate(time);
        if (update is KeyUpdate key) return KeyUpdate(key);
        if (update is MovementUpdate move) return MovementUpdate(move);
        if (update is KillUpdate kill) return KillUpdate(kill);
        if (update is LifeUpdate life) return LifeUpdate(life);
        return false;
    }

    private bool TimeFlowUpdate(TimeFlowUpdate update) {
        // 1) Check delta:
        if (update.DeltaT <= 0) return false;
        
        // 2.1) Save last position:
        LastPosition = Position;
        LastNormal = Normal;
        // 2.2) Compute next position:
        Normal = Collision.ZERO_VECTOR;
        if (JumpFinishY == null) StartFall();
        position.Center = ComputeNextCenter(update.DeltaT);

        // 3.1) Resolve map collisions:
        Collision.Resolve(update.Game.Map.Rect, position, ResolveCollision);
        // 3.2) Resolve jump height:
        if (JumpFinishY != null) {
            Collision.Resolve((float) JumpFinishY, POSITION.TOP, position, ResolveCollision);
        }
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

        // 5) Return result:
        return true;
    }

    private readonly UpdateGuard<KeyUpdate> KeyUpdateGuard = new();
    private bool KeyUpdate(KeyUpdate update) {
        var updated = false;
        foreach (var control in update.Controls) {
            switch (control.Key) {
                case GAME_CONTROLS.LEFT:
                case GAME_CONTROLS.RIGHT:
                    if (KeyUpdateGuard.Update(
                        update, () => ChangeDirection(control.Key, control.Pressed)
                    )) updated = true;
                break;
                case GAME_CONTROLS.SPACE:
                    if (IsJumping) {
                        PendingJump = (update, DateTime.UtcNow);
                        break;
                    }
                    StartJump();
                    updated = true;
                break;
            }
        }
        return updated;
    }

    private bool MovementUpdate(MovementUpdate update) {
        position.Center = update.Center;
        direction = update.Direction;
        JumpFinishY = update.JumpFinishY;
        return true;
    }

    private bool KillUpdate(KillUpdate update) {
        if (!Alive) return false;
        Alive = false;
        StartFall();
        return true;
    }

    private bool LifeUpdate(LifeUpdate update) {
        if (Alive) return false;
        Alive = true;
        return true;
    }

    public void ResetUpdateGuards() {
        KeyUpdateGuard.Reset();
    }

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public async Task Render(Canvas2DContext ctx, (Game Game, SKIN Skin) @params) {
        var (game, skin) = @params;

        // await ctx.SetFillStyleAsync("lime");
        // var p = game.Map.ToScreen(Center);
        // var pW = game.Map.ToScreenWidth(WIDTH);
        // var pH = game.Map.ToScreenHeight(HEIGHT);
        // await ctx.FillRectAsync(
        //     Math.Floor((double) p.X - pW / 2),
        //     Math.Floor((double) p.Y - pH / 2),
        //     Math.Ceiling((double) pW),
        //     Math.Ceiling((double) pH)
        // );

        await Animation.Render(ctx, (game, skin, Alive, IsJumping, Center, Direction));
    }
}

namespace Jumpeno.Shared.Models;

public class Body : IRectFPositionable, IUpdateable, IRenderableParametric<(Game Game, SKIN? Skin)> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const int WIDTH = 54;
    public const int HEIGHT = 64;
    public const float SPEED = 0.5f; // px per ms
    public const float JUMP_HEIGHT = 220f;
    public const float JUMP_SPEED = 1.2f; // px per ms (at the start)

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
    public float? JumpFinishY { get; private set; }
    public bool IsJumping => JumpFinishY != null;
    // Collision (normal vector):
    public PointF LastNormal { get; private set; }
    public PointF Normal { get; private set; }

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
    }
    public Body(): this(false, new(500, HALF_HEIGHT), new(0, -1), null) {}

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
        
        // 4) Return result:
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
                    if (JumpFinishY != null) break;
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
    public async Task Render(Canvas2DContext ctx, (Game Game, SKIN? Skin) parameters) {
        var (game, skin) = parameters;
        skin ??= SKIN.MAGE_MAGIC;
        var p = game.Map.ToScreen(Center);
        int width = game.Map.ToScreenWidth(WIDTH);
        int height = game.Map.ToScreenHeight(HEIGHT);
        await ctx.SetFillStyleAsync(Alive ? "red" : "blue");
        await ctx.FillRectAsync(
            Math.Floor((double) p.X - width / 2),
            Math.Floor((double) p.Y - height / 2),
            Math.Ceiling((double) width),
            Math.Ceiling((double) height)
        );
    }
}

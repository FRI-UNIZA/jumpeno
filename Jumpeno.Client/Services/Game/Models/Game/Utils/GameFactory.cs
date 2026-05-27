namespace Jumpeno.Client.Models;

public partial class Game {
    // Partial update factory -------------------------------------------------------------------------------------------------------------
    public KillUpdate NewKillUpdate(byte? killerID, byte deadID, bool penalize = false) {
        return new(killerID, deadID, penalize);
    }
    public LifeUpdate NewLifeResetUpdate(byte playerID) {
        return new(playerID, Body.ImmortalMs);
    }
    public LifeUpdate NewLifeUpdate(byte playerID) {
        return new(playerID, Time + Body.ImmortalMs);
    }

    public MovementUpdate NewMovementUpdate(byte playerID) {
        if (!Players.TryGetValue(playerID, out var player)) throw new ArgumentException("Wrong player ID!");
        return new(playerID, player.Body.Position.Center, player.Body.Direction, player.Body.JumpFinishY, player.Body.Normal);
    }
    public MovementUpdate NewMovementUpdate(Player jumper, Player victim) {
        return new MovementUpdate(
            jumper.ID,
            new(jumper.Body.Position.Center.X, victim.Body.Position.Center.Y + Body.Height),
            jumper.Body.Direction, jumper.Body.JumpFinishY, jumper.Body.Normal
        );
    }
    public MovementUpdate NewMovementUnderMapUpdate(Player player) {
        return new MovementUpdate(
            player.ID,
            new(
                player.Body.Position.Center.X,
                Map.WorldMinY - (Mark.CalculateMarkPointTop(player.Body).Y - player.Body.Position.Center.Y)
            ),
            player.Body.Direction, player.Body.JumpFinishY, player.Body.Normal
        );
    }
    public MovementUpdate NewRandomPositionUpdate(Player player, Random? random = null, HashSet<float>? used = null) {
        // 1) Prepare parameters:
        random ??= new Random();
        used ??= [];
        // 2) Randomize position:
        float x = Map.WorldMinX + random.Next(0, (int) Map.WorldWidth) / Tile.Size * Tile.Size + Tile.HalfSize;
        while (used.Contains(x)) x = Map.WorldMinX + (x - Map.WorldMinX + Tile.Size) % Map.WorldWidth;
        used.Add(x);
        var y = Map.WorldMinY + random.Next(0, (int) Map.WorldHeight) / Tile.Size * Tile.Size + Body.HalfHeight;
        // 3) Avoid tile collision:
        var position = new PointF(x, y);
        while (Map.GetCollidingTiles(new(position.X - Tile.HalfSize, position.Y - Tile.HalfSize, Tile.Size, Tile.Size)).Count > 0) {
            position.Y = Map.WorldMinY + (position.Y - Map.WorldMinY + Tile.Size) % Map.WorldHeight;
        }
        // 4) Put on the ground:
        while (
            Map.GetCollidingTiles(new(position.X - Tile.HalfSize, position.Y - Tile.HalfSize, Tile.Size, Tile.Size)).Count <= 0
            && (position.Y > Map.WorldMinY)
        ) position.Y -= Tile.Size;
        position.Y += Tile.Size;
        // 5) Return update:
        return new(player.ID, position, Body.DefaultDirection, null, Body.DefaultNormal, new(random.NextDouble() < 0.5 ? 1 : -1, -1));
    }

    public StateUpdate NewStateUpdate(double time, GameStates state, int? level = null, double? timer = null) {
        return new(time, state, level ?? Map.Shrink.Level, timer ?? Map.Shrink.Timer);
    }
    public TimeFlowUpdate NewTimeFlowUpdate(double deltaT) => new(deltaT);

    // Network update factory -------------------------------------------------------------------------------------------------------------
    private readonly NetworkUpdater Updater = new();

    public GamePlayUpdate NewGamePlayUpdate(
        StateUpdate stateUpdate,
        Dictionary<byte, MovementUpdate>? movements = null,
        Dictionary<byte, KillUpdate>? kills = null,
        Dictionary<byte, LifeUpdate>? lives = null
    ) {
        return Updater.NewGamePlayUpdate(Round, stateUpdate, movements, kills, lives);
    }
    public GamePlayUpdate NewGamePlayCurrentUpdate() {
        return NewGamePlayUpdate(NewStateUpdate(Time, State));
    }

    public KeyUpdate NewKeyUpdate(byte playerID, LinkedList<Control> controls) {
        return Updater.NewKeyUpdate(Round, playerID, controls);
    }

    private GamePlayUpdate NewPlayerManipulationUpdate(Player player) {
        // 1) Create update:
        var update = NewGamePlayCurrentUpdate();
        // 2.1) Add movement update:
        var move = NewMovementUnderMapUpdate(player);
        update.Movements[player.ID] = move;
        // 2.2) Add kill update:
        var kill = NewKillUpdate(null, player.ID);
        update.Kills[player.ID] = kill;
        // 3) Return update:
        return update;
    }
    public PlayerUpdate NewPlayerAddUpdate(Player player) {
        bool isHost = player.User.ID == Host.ID;
        var update = NewPlayerManipulationUpdate(player);
        return Updater.NewPlayerUpdate(Round, isHost || HostConnected, player, isHost ? Round + 1 : Round, false, update);
    }
    public PlayerUpdate NewPlayerReadyUpdate(Player player) {
        return Updater.NewPlayerUpdate(Round, HostConnected, player, Round + 1, false);
    }
    private PlayerUpdate NewPlayerRemoveUpdate(Player player, bool kick) {
        bool isHost = player.User.ID == Host.ID;
        var update = NewPlayerManipulationUpdate(player);
        return Updater.NewPlayerUpdate(Round, !isHost && HostConnected, player, player.ReadyForRound, kick || State == GameStates.Lobby, update);
    }
    public PlayerUpdate NewPlayerRemoveUpdate(Player player) => NewPlayerRemoveUpdate(player, false);
    public PlayerUpdate NewPlayerKickUpdate(Player player) => NewPlayerRemoveUpdate(player, true);

    public SpectatorUpdate NewSpectatorAddUpdate(Spectator spectator) {
        return Updater.NewSpectatorUpdate(Round, spectator.User.ID == Host.ID || HostConnected, SpectatorCount + 1);
    }
    public SpectatorUpdate NewSpectatorRemoveUpdate(Spectator spectator) {
        return Updater.NewSpectatorUpdate(Round, spectator.User.ID != Host.ID && HostConnected, SpectatorCount - 1);
    }

    public RoundUpdate NewRoundStartUpdate() {
        KillPlayers();
        ResurrectPlayers();
        return Updater.NewRoundUpdate(
            Round + 1, NewStateUpdate(0, GameStates.GamePlay, Shrink.Default.LEVEL, Shrink.Default.TIMER),
            Players
        );
    }
    public RoundUpdate NewRoundFinishUpdate() {
        KillPlayers();
        return Updater.NewRoundUpdate(Round, NewStateUpdate(Time, GameStates.ScoreBoard), Players);
    }
}

namespace Jumpeno.Client.Models;

#pragma warning disable CA1822

public partial class Game : IUpdateable, IRenderable<(Player? ScreenPlayer, string Font)> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static readonly int FPS = AppSettings.Game.FPS;
    public static readonly int TOUCH_DEVICE_NOTIFICATIONS = AppSettings.Game.TouchDeviceNotifications.PerSecond; // per second
    // Duration:
    public static readonly double ROUND_DURATION = From.MinToMS(AppSettings.Game.Round.Minutes) - Shrink.TOTAL_DURATION; // ms
    public static readonly double ROUND_FINISH_DELAY = From.SToMS(AppSettings.Game.FinishDelay.Seconds); // ms
    // States:
    public static readonly List<GameStates> LOBBY_STATES = [GameStates.LOBBY, GameStates.SCOREBOARD];
    public static readonly List<GameStates> PAUSE_STATES = [GameStates.PAUSE];
    public static readonly List<GameStates> RUN_STATES = [GameStates.GAMEPLAY, GameStates.SHRINKING];

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    // Identifier:
    public ulong ID { get; }
    private static ulong IDAutoIncrement = 0;
    // Settings:
    public DisplayMode DisplayMode { get; }
    public GameMode Mode { get; }
    public User Host { get;  }
    public string Code { get; }
    public string Name { get; }
    public Map Map { get; }
    public bool Anonyms { get; }
    public byte Rounds { get; }
    public byte Capacity { get; }
    // State:
    public int Round { get; private set; }
    public int ShowRound => Math.Min(LOBBY_STATES.Contains(State) ? Round + 1 : Round, Rounds);
    public double Time { get; private set; }
    public GameStates State { get; private set; }
    public bool IsFinished => State == GameStates.SCOREBOARD && Rounds <= Round;
    // Clock:
    public readonly GameClock Clock;
    public readonly GameClock TouchClock;
    private bool ClockAutoReset = false;
    public void ClockAutoResetOn() => ClockAutoReset = true;
    public void ClockAutoResetOff() => ClockAutoReset = false;
    private void ResetClocks() { Clock.Reset(); TouchClock.Reset(); }
    // Host:
    public bool HostConnected { get; private set; }
    // Players:
    // NOTE: Index containing all possible game players:
    [JsonInclude] private Dictionary<byte, Player> Players { get; }
    // NOTE: Active players are connected:
    [JsonInclude] private List<Player> ActivePlayers { get; }
    public int ValidPlayersCount => ValidPlayerIterator.Count();
    public int ActivePlayersCount => ActivePlayers.Count;
    public int AlivePlayersCount { get; private set; }
    // NOTE: QuadTree of active players:
    private QuadTreeRectF<Player> PlayersQT { get; }
    // Spectators:
    public int SpectatorCount { get; private set; }
    // Empty:
    public bool IsEmpty => ActivePlayersCount <= 0 && SpectatorCount <= 0;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private Game(
        ulong id,
        DisplayMode displayMode, GameMode mode, User host, string code, string name,
        Map map, bool anonyms, byte rounds, byte capacity,
        int round, double time, GameStates state,
        bool hostConnected,
        Dictionary<byte, Player> players, List<Player> activePlayers,
        int spectatorCount
    ) {
        // Validation:
        GameValidator.AssertCode(code);
        GameValidator.AssertName(name);
        GameValidator.AssertCapacity(capacity);
        // Identifier:
        ID = id;
        // Settings:
        DisplayMode = displayMode;
        Mode = mode;
        Host = host;
        Code = code;
        Name = name.Trim();
        Map = map;
        Anonyms = anonyms;
        Rounds = rounds;
        Capacity = capacity;
        // State:
        Round = round;
        Time = time;
        State = state;
        // Clocks:
        Clock = new(FPS);
        TouchClock = new(TOUCH_DEVICE_NOTIFICATIONS);
        // Host:
        HostConnected = hostConnected;
        // Players:
        ActivePlayers = InitActivePlayers(activePlayers, players);
        AlivePlayersCount = InitAlivePlayerCount(ActivePlayers);
        Players = InitPlayers(ActivePlayers, players);
        PlayersQT = InitPlayersQT(ActivePlayers);
        // Spectators:
        SpectatorCount = spectatorCount;
    }
    public Game(
        DisplayMode displayMode, GameMode mode, User host, string code, string name,
        Map map, bool anonyms, byte rounds, byte capacity
    ) : this(
        IDAutoIncrement++,
        displayMode, mode, host, code, name, map, anonyms, rounds, capacity,
        0, 0, GameStates.LOBBY,
        false, [], [], 0
    ) {}

    // Initializers -----------------------------------------------------------------------------------------------------------------------
    private List<Player> InitActivePlayers(List<Player> activePlayers, Dictionary<byte, Player> players) {
        List<Player> result = [];
        foreach (var value in activePlayers) {
            if (!players.TryGetValue(value.ID, out var player)) continue;
            result.Add(player);
        }
        return result;
    }

    private static int InitAlivePlayerCount(List<Player> activePlayers) {
        var alive = 0;
        foreach (var player in activePlayers) {
            if (player.IsAlive) alive++;
        }
        return alive;
    }

    private Dictionary<byte, Player> InitPlayers(List<Player> activePlayers, Dictionary<byte, Player>? players = null) {
        if (players != null && players.Count == Capacity) return players;
        Dictionary<byte, Player> result = [];
        for (byte i = 0; i < Capacity; i++) result.Add(i, new Player(i));
        foreach (var player in activePlayers) result[player.ID] = player;
        return result;
    }

    private QuadTreeRectF<Player> InitPlayersQT(List<Player> activePlayers) {
        var padding = 2 * Animation.MAX_HEIGHT + Mark.HEIGHT;
        QuadTreeRectF<Player> players = new(
            Map.Rect.Left - padding, Map.Rect.Top - padding,
            Map.Rect.Width + 2 * padding, Map.Rect.Height + 2 * padding
        );
        foreach (var player in activePlayers) players.Add(player);
        return players;
    }

    // Player > Connection ----------------------------------------------------------------------------------------------------------------
    public Player CreatePlayer(
        // Parameters:
        Connection connection,
        // Exceptions:
        string nameID = ""
    ) {
        // 1) Validation:
        AppEnvironment.AssertServer();
        UserValidator.AssertConnectionType(connection);
        UserValidator.AssertUnknown(connection.User, nameID);
        AssertAllowedAnonymousUser(connection.User);
        // 2) Check host:
        AssertPlayerHostPresentation(connection.User);
        AssertReservedPlayerHostSpace(connection.User);
        // 3) Find space:
        var player = FindPlayerSpace(connection, nameID);
        // 4) Set skin of anonym:
        SetSkinOfAnonymousUser(connection.User);
        // 5) Synchronize:
        player.Synchronize(connection);
        // 6) Return player:
        return player;
    }

    private Player FindPlayerSpace(
        // Parameters:
        Connection connection,
        // Exceptions:
        string nameID = ""
    ) {
        // 1) Try to find space:
        Player? space = null;
        foreach (var (id, player) in Players) {
            if (player.IsConnected) {
                if (player.User.Name.ToLower() != connection.User.Name.ToLower()) continue;
                if (State == GameStates.LOBBY) {
                    throw Exceptions.DEFAULT.SetInfo("Player name is taken!")
                    .SetErrors(Errors.DEFAULT.SetID(nameID).SetInfo("Player name is taken!"));
                } else {
                    throw Exceptions.DEFAULT.SetInfo("The game is already running.");
                }
            } else if (State == GameStates.LOBBY || player.User.Equals(connection.User)) {
                AssertReservedPlayerHostName(connection.User, userNameID: nameID);
                space = player;
                break;
            }
        }
        // 2.1) Space not found:
        if (space == null) {
            if (State == GameStates.LOBBY) throw Exceptions.DEFAULT.SetInfo("The game is currently full!");
            else throw Exceptions.DEFAULT.SetInfo("The game is already running.");
        }
        // 2.2) Or return found space:
        else return space;
    }

    private void SetSkinOfAnonymousUser(User user) {
        // 1) Check anonym:
        if (user.ID != null) return;
        // 2) Select skin:
        user.Skin = GetAvailableSkin();
    }

    // Player > Getters > Host ------------------------------------------------------------------------------------------------------------
    public Player? GetHostPlayer() {
        foreach (var (_, player) in Players) {
            if (player.User.ID == Host.ID) return player;
        }
        return null;
    }

    // Player > Getters > All players -----------------------------------------------------------------------------------------------------
    public Player? GetPlayer(byte? id) {
        if (id is not byte playerID) return null;
        Players.TryGetValue(playerID, out var player); return player;
    }

    public Player? GetPlayerByConnectionID(string? connectionID) {
        if (connectionID == null) return null;
        foreach (var (_, player) in Players) {
            if (player.ConnectionID == connectionID) return player;
        }
        return null;
    }

    public Player? GetPlayerByName(string? name) {
        if (name == null) return null;
        foreach (var (_, player) in Players) {
            if (player.User.Name == name) return player;
        }
        return null;
    }

    // Player > Getters > Valid players ---------------------------------------------------------------------------------------------------
    public Player? GetValidPlayer(byte? id) {
        var player = GetPlayer(id);
        return Player.IsValid(player) ? player : null;
    }

    public Player? GetValidPlayerByConnectionID(string? connectionID) {
        var player = GetPlayerByConnectionID(connectionID);
        return Player.IsValid(player) ? player : null;
    }

    public Player? GetValidPlayerByName(string? name) {
        var player = GetPlayerByName(name);
        return Player.IsValid(player) ? player : null;
    }

    // Player > Getters > Active players --------------------------------------------------------------------------------------------------
    public Player? GetActivePlayer(byte? id) {
        if (id == null) return null;
        foreach (var player in ActivePlayers) {
            if (player.ID == id) return player;
        }
        return null;
    }

    public Player? GetActivePlayerByConnectionID(string? connectionID) {
        if (connectionID == null) return null;
        foreach (var player in ActivePlayers) {
            if (player.ConnectionID == connectionID) return player;
        }
        return null;
    }

    public Player? GetActivePlayerByName(string? name) {
        if (name == null) return null;
        foreach (var player in ActivePlayers) {
            if (player.User.Name == name) return player;
        }
        return null;
    }

    // Player > Getters > Collisions ------------------------------------------------------------------------------------------------------
    public List<Player> GetCollidingPlayers(Player player) => PlayersQT.GetObjects(player.Rect);

    // Player > Getters > Iterators -------------------------------------------------------------------------------------------------------
    public IEnumerable<(Player player, int index)> PlayerIterator { get {
        int index = 0;
        foreach (var (_, player) in Players) yield return (player, index++);
    }}

    public IEnumerable<(Player player, int index)> ValidPlayerIterator { get {
        int index = 0;
        foreach (var (_, player) in Players) {
            if (player.IsValid()) yield return (player, index++);
        }
    }}

    public IEnumerable<(Player player, int index)> ActivePlayerIterator { get {
        int index = 0;
        foreach (var player in ActivePlayers) yield return (player, index++);
    }}

    public IEnumerable<(Player player, int index)> PlayerScoreIterator { get {
        int index = 0;
        foreach (var (player, id) in ValidPlayerIterator.OrderByDescending(x => x.player.Score)) {
            yield return (player, index++);
        }
    }}

    // Player > Predicates ----------------------------------------------------------------------------------------------------------------
    public bool IsPlayerReady(Player player) => IsFinished || !LOBBY_STATES.Contains(State) || player.ReadyForRound == Round + 1;

    public bool ActivePlayersReady() {
        foreach (var player in ActivePlayers) {
            if (!IsPlayerReady(player)) return false;
        }
        return true;
    }

    // Player > Utils ---------------------------------------------------------------------------------------------------------------------
    private void OnPlayerKill() => AlivePlayersCount--;
    private void OnPlayerAlive() => AlivePlayersCount++;

    private void MovePlayer(Player player) => PlayersQT.Move(player);

    private void ResurrectPlayers() {
        var rand = new Random();
        var used = new HashSet<float>();
        foreach (var player in ActivePlayers) {
            Update(NewRandomPositionUpdate(player, rand, used));
            Update(NewLifeResetUpdate(player.ID));
        }
    }

    private void KillPlayers() {
        foreach (var (id, player) in Players) {
            Update(NewKillUpdate(null, id));
            Update(NewMovementUnderMapUpdate(player));
        }
    }

    private Skin GetAvailableSkin()
    {
        // 1) Select free skins:
        var usedSkins = ActivePlayers.Select(p => p.User.Skin);
        var allSkins = Enum.GetValues<Skin>();
        var freeSkins = allSkins.Except(usedSkins);
        // 2.1) Find random avaible skin:
        if (freeSkins.Any())
            return freeSkins.ElementAt(Random.Shared.Next(freeSkins.Count()));
        // 2.2) Fallback:
        else
            return allSkins.ElementAt(Random.Shared.Next(allSkins.Length));
    }

    // Spectator > Connection -------------------------------------------------------------------------------------------------------------
    public Spectator CreateSpectator(
        // Parameters:
        Connection connection,
        // Exceptions:
        string nameID = ""
    ) {
        // 1) Validation:
        AppEnvironment.AssertServer();
        UserValidator.AssertConnectionType(connection);
        UserValidator.AssertUnknown(connection.User, nameID);
        AssertSpectatorCount();
        // 2) Check host:
        AssertSpectatorHostNonPresentation(connection.User);
        AssertHostConnectedOnce(connection.User);
        AssertReservedSpectatorHostSpace(connection.User);
        // 3) Return spectator:
        return new(connection);
    }

    // Updates ----------------------------------------------------------------------------------------------------------------------------
    public bool Update(GameUpdate update) {
        update.Game = this;
        return update switch {
            TimeFlowUpdate time => TimeFlowUpdate(time),
            KeyUpdate key => KeyUpdate(key),
            GamePlayUpdate game => GamePlayUpdate(game),
            MovementUpdate move => MovementUpdate(move),
            KillUpdate kill => KillUpdate(kill),
            LifeUpdate life => LifeUpdate(life),
            PlayerUpdate player => PlayerUpdate(player),
            SpectatorUpdate watch => SpectatorUpdate(watch),
            StateUpdate state => StateUpdate(state),
            RoundUpdate round => RoundUpdate(round),
            _ => false
        };
    }

    private bool TimeFlowUpdate(TimeFlowUpdate update) {
        if (update.DeltaT <= 0) return false;
        Time += update.DeltaT;
        Map.Update(update);
        foreach (var (_, player) in Players) player.Update(update);
        foreach (var player in ActivePlayers) MovePlayer(player);
        return true;
    }

    private bool KeyUpdate(KeyUpdate update) {
        // 1) Check correct state:
        if (update.Round != Round) return false;
        if (!RUN_STATES.Contains(State)) return false;
        // 2) Find player:
        Players.TryGetValue(update.PlayerID, out var player);
        if (player == null) return false;
        // 3) Execute update:
        return player.Update(update);
    }

    readonly UpdateGuard<GamePlayUpdate> GamePlayUpdateGuard = new();
    private bool GamePlayUpdate(GamePlayUpdate update) {
        // 1) Check correct state:
        if (update.Round != Round) return false;
        if (LOBBY_STATES.Contains(State)) return false;
        // 2) Prepare response with state update:
        var response = new GamePlayResponse {
            StateUpdated = GamePlayUpdateGuard.Update(update, () => Update(update.StateUpdate))
        };
        // 3) Prepare updates:
        HashSet<byte> updates = [];
        Dictionary<byte, LinkedList<KillUpdate>> scoreUpdates = [];
        foreach (var (id, move) in update.Movements) { updates.Add(id); move.Game = this; }
        foreach (var (id, kill) in update.Kills) { updates.Add(id); kill.Game = this; }
        foreach (var (id, life) in update.Lives) { updates.Add(id); life.Game = this; }
        foreach (var (id, killUpdate) in update.Kills) {
            if (killUpdate.KillerID is not byte killerID || id == killUpdate.KillerID) continue;
            if (!scoreUpdates.ContainsKey(killerID)) scoreUpdates[killerID] = [];
            scoreUpdates[killerID].AddLast(killUpdate);
        }
        // 4) Apply player updates:
        foreach (var id in updates) {
            if (!Players.TryGetValue(id, out var player)) continue;
            if (player.Update(update)) {
                response.MoveUpdated = response.MoveUpdated || update.Response.MoveUpdated;
                response.KillUpdated = response.KillUpdated || update.Response.KillUpdated;
                response.LifeUpdated = response.LifeUpdated || update.Response.LifeUpdated;
                if (update.Response.KillUpdated) OnPlayerKill();
                if (update.Response.LifeUpdated) OnPlayerAlive();
            }
        }
        // 5) Update score:
        foreach (var (id, killUpdates) in scoreUpdates) {
            if (!Players.TryGetValue(id, out var player)) continue;
            foreach (var kill in killUpdates) {
                if (player.Update(kill)) response.ScoreUpdated = true;
            }
        }
        // 6) Return result:
        update.Response = response;
        return response.Updated;
    }

    private bool MovementUpdate(MovementUpdate update) {
        if (!Players.TryGetValue(update.PlayerID, out var player)) return false;
        var updated = player.Update(update);
        if (updated) MovePlayer(player);
        return updated;
    }

    private bool KillUpdate(KillUpdate update) {
        // 1) Kill player:
        if (!Players.TryGetValue(update.DeadID, out var dead)) return false;
        var updated = dead.Update(update);
        if (updated) OnPlayerKill();
        // 2) Update score:
        if (update.KillerID is not byte killerID) return updated;
        if (!Players.TryGetValue(killerID, out var killer)) return updated;
        return killer.Update(update);
    }

    private bool LifeUpdate(LifeUpdate update) {
        if (!Players.TryGetValue(update.PlayerID, out var player)) return false;
        var updated = player.Update(update);
        if (updated) OnPlayerAlive();
        return updated;
    }

    private bool PlayerUpdate(PlayerUpdate update) {
        if (!Players.TryGetValue(update.Player.ID, out var player)) return false;
        if (!player.Update(update)) return false;
        // 1) Update host:
        HostConnected = update.HostConnected;
        // 2) Remove player:
        ActivePlayers.Remove(player);
        PlayersQT.Remove(player);
        // 3) Add player if connected:
        if (player.IsConnected) {
            ActivePlayers.Add(player);
            PlayersQT.Add(player);
            if (AppEnvironment.IsServer) player.ResetUpdateGuards();
        }
        // 4) Apply gameplay update:
        if (update.GamePlayUpdate != null) {
            Update(update.GamePlayUpdate);
        }
        return true;
    }

    private readonly UpdateGuard<SpectatorUpdate> SpectatorUpdateGuard = new();
    private bool SpectatorUpdate(SpectatorUpdate update)
    => SpectatorUpdateGuard.Update(update, () => {
        HostConnected = update.HostConnected;
        SpectatorCount = update.SpectatorCount;
    });

    private bool StateUpdate(StateUpdate update) {
        // 1) Current state:
        switch (State) {
            case GameStates.PAUSE:
                if (update.State != GameStates.PAUSE) ResetClocks();
            break;
        }
        // 2) New state:
        switch (update.State) {
            case GameStates.PAUSE:
                foreach (var player in Players.Values) {
                    player.Update(update);
                }
            break;
        }
        // 3) Update state:
        Time = update.Time;
        State = update.State;
        Map.Update(update);
        // 4) AutoReset:
        if (ClockAutoReset) ResetClocks();
        // 5) Return result:
        return true;
    }

    private readonly UpdateGuard<RoundUpdate> RoundUpdateGuard = new();
    private bool RoundUpdate(RoundUpdate update) 
    => RoundUpdateGuard.Update(update, () => {
        // 1) Reset clocks:
        ResetClocks();
        // 2) Update round & state:
        Round = update.Round;
        Update(update.StateUpdate);
        // 3) Reset alive players count:
        AlivePlayersCount = 0;
        // 4) Update players (position & score):
        foreach (var (id, player) in Players) {
            player.Update(update);
            if (player.IsAlive) AlivePlayersCount++;
        }
        foreach (var player in ActivePlayers) MovePlayer(player);
        // 5) Return result:
        return true;
    });

    public void ResetUpdateGuards() {
        GamePlayUpdateGuard.Reset();
        SpectatorUpdateGuard.Reset();
        RoundUpdateGuard.Reset();
    }

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public async Task<bool> Render(Canvas2DContext ctx, (Player? ScreenPlayer, string Font) @params) {
        var (screenPlayer, font) = @params;
        // 1) Render map:
        await Map.Render(ctx, this);
        // 2) Render players:
        Player? myPlayer = null;
        foreach (var player in ActivePlayers) {
            if (player.Equals(screenPlayer)) {
                myPlayer = player; continue;
            }
            await player.Render(ctx, this);
        }
        // 3) Render screen player on top:
        if (myPlayer != null) await myPlayer.Render(ctx, this);
        // 4) Render names or mark:
        foreach (var player in ActivePlayers) {
            if (player.Equals(screenPlayer)) await Mark.RenderMark(ctx, (this, player));
            else await Mark.RenderName(ctx, (this, player, font));
        }
        return true;
    }
}

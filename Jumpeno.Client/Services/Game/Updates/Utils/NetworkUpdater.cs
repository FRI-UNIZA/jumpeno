namespace Jumpeno.Client.Utils;

public class NetworkUpdater {
    // GamePlayUpdate ---------------------------------------------------------------------------------------------------------------------
    private ulong GamePlayUpdateID = 0;
    public GamePlayUpdate NewGamePlayUpdate(
        int round, StateUpdate stateUpdate,
        Dictionary<byte, MovementUpdate>? movements = null,
        Dictionary<byte, KillUpdate>? kills = null,
        Dictionary<byte, LifeUpdate>? lives = null
    ) {
        return new GamePlayUpdate(GamePlayUpdateID++, round, stateUpdate, movements, kills, lives);
    }

    // KeyUpdate --------------------------------------------------------------------------------------------------------------------------
    private ulong KeyUpdateID = 0;
    public KeyUpdate NewKeyUpdate(int round, byte playerID, LinkedList<Control> controls) {
        return new KeyUpdate(KeyUpdateID++, round, playerID, controls);
    }

    // PingUpdate --------------------------------------------------------------------------------------------------------------------------
    private ulong PingUpdateID = 0;
    public PingUpdate NewPingUpdate(int round, DateTime createdAt) {
        return new PingUpdate(PingUpdateID++, round, createdAt);
    }

    // PlayerUpdate -----------------------------------------------------------------------------------------------------------------------
    private ulong PlayerUpdateID = 0;
    public PlayerUpdate NewPlayerUpdate(int round, Player player) {
        return new PlayerUpdate(PlayerUpdateID++, round, player);
    }

    // RoundUpdate ------------------------------------------------------------------------------------------------------------------------
    private ulong RoundUpdateID = 0;
    public RoundUpdate NewRoundUpdate(int round, StateUpdate stateUpdate, Dictionary<byte, Player> players) {
        return new RoundUpdate(RoundUpdateID++, round, stateUpdate, players);
    }

    // SpectatorUpdate --------------------------------------------------------------------------------------------------------------------
    private ulong SpectatorUpdateID = 0;
    public SpectatorUpdate NewSpectatorUpdate(int round, int spectatorCount) {
        return new SpectatorUpdate(SpectatorUpdateID++, round, spectatorCount);
    }

    // Reset ------------------------------------------------------------------------------------------------------------------------------
    public void Reset() {
        GamePlayUpdateID = 0;
        KeyUpdateID = 0;
        PingUpdateID = 0;
        PlayerUpdateID = 0;
        RoundUpdateID = 0;
        SpectatorUpdateID = 0;
    }
}

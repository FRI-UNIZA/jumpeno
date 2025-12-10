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

    // PlayerUpdate -----------------------------------------------------------------------------------------------------------------------
    private ulong PlayerUpdateID = 0;
    public PlayerUpdate NewPlayerUpdate(
        int round, bool hostConnected, Player player, int readyForRound, bool invalidate, GamePlayUpdate? gamePlayUpdate = null
    )
    => new(PlayerUpdateID++, round, hostConnected, player, readyForRound, invalidate, gamePlayUpdate);

    // RoundUpdate ------------------------------------------------------------------------------------------------------------------------
    private ulong RoundUpdateID = 0;
    public RoundUpdate NewRoundUpdate(int round, StateUpdate stateUpdate, Dictionary<byte, Player> players) {
        return new RoundUpdate(RoundUpdateID++, round, stateUpdate, players);
    }

    // SpectatorUpdate --------------------------------------------------------------------------------------------------------------------
    private ulong SpectatorUpdateID = 0;
    public SpectatorUpdate NewSpectatorUpdate(int round, bool hostConnected, int spectatorCount) {
        return new SpectatorUpdate(SpectatorUpdateID++, round, hostConnected, spectatorCount);
    }

    // Reset ------------------------------------------------------------------------------------------------------------------------------
    public void Reset() {
        GamePlayUpdateID = 0;
        KeyUpdateID = 0;
        PlayerUpdateID = 0;
        RoundUpdateID = 0;
        SpectatorUpdateID = 0;
    }
}

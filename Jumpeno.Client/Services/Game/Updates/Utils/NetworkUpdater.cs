namespace Jumpeno.Client.Utils;

public class NetworkUpdater {
    // GamePlayUpdate ---------------------------------------------------------------------------------------------------------------------
    private ulong _gamePlayUpdateId = 0;
    public GamePlayUpdate NewGamePlayUpdate(
        int round, StateUpdate stateUpdate,
        Dictionary<byte, MovementUpdate>? movements = null,
        Dictionary<byte, KillUpdate>? kills = null,
        Dictionary<byte, LifeUpdate>? lives = null
    ) {
        return new GamePlayUpdate(_gamePlayUpdateId++, round, stateUpdate, movements, kills, lives);
    }

    // KeyUpdate --------------------------------------------------------------------------------------------------------------------------
    private ulong _keyUpdateId = 0;
    public KeyUpdate NewKeyUpdate(int round, byte playerID, LinkedList<Control> controls) {
        return new KeyUpdate(_keyUpdateId++, round, playerID, controls);
    }

    // PlayerUpdate -----------------------------------------------------------------------------------------------------------------------
    private ulong _playerUpdateId = 0;
    public PlayerUpdate NewPlayerUpdate(
        int round, bool hostConnected, Player player, int readyForRound, bool invalidate, GamePlayUpdate? gamePlayUpdate = null
    )
    => new(_playerUpdateId++, round, hostConnected, player, readyForRound, invalidate, gamePlayUpdate);

    // RoundUpdate ------------------------------------------------------------------------------------------------------------------------
    private ulong _roundUpdateId = 0;
    public RoundUpdate NewRoundUpdate(int round, StateUpdate stateUpdate, Dictionary<byte, Player> players) {
        return new RoundUpdate(_roundUpdateId++, round, stateUpdate, players);
    }

    // SpectatorUpdate --------------------------------------------------------------------------------------------------------------------
    private ulong _spectatorUpdateId = 0;
    public SpectatorUpdate NewSpectatorUpdate(int round, bool hostConnected, int spectatorCount) {
        return new SpectatorUpdate(_spectatorUpdateId++, round, hostConnected, spectatorCount);
    }

    // Reset ------------------------------------------------------------------------------------------------------------------------------
    public void Reset() {
        _gamePlayUpdateId = 0;
        _keyUpdateId = 0;
        _playerUpdateId = 0;
        _roundUpdateId = 0;
        _spectatorUpdateId = 0;
    }
}

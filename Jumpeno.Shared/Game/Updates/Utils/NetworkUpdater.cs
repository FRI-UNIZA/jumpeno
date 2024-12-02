namespace Jumpeno.Shared.Utils;

public class NetworkUpdater {
    // GamePlayUpdate ---------------------------------------------------------------------------------------------------------------------
    private ulong GamePlayUpdateID = 0;
    public GamePlayUpdate NewGamePlayUpdate(
        StateUpdate stateUpdate,
        Dictionary<byte, MovementUpdate>? movements = null,
        Dictionary<byte, KillUpdate>? kills = null
    ) {
        return new GamePlayUpdate(GamePlayUpdateID++, stateUpdate, movements, kills);
    }

    // KeyUpdate --------------------------------------------------------------------------------------------------------------------------
    private ulong KeyUpdateID = 0;
    public KeyUpdate NewKeyUpdate(byte playerID, LinkedList<Control> controls) {
        return new KeyUpdate(KeyUpdateID++, playerID, controls);
    }

    // PingUpdate --------------------------------------------------------------------------------------------------------------------------
    private ulong PingUpdateID = 0;
    public PingUpdate NewPingUpdate(DateTime createdAt) {
        return new PingUpdate(PingUpdateID++, createdAt);
    }

    // PlayerUpdate -----------------------------------------------------------------------------------------------------------------------
    private ulong PlayerUpdateID = 0;
    public PlayerUpdate NewPlayerUpdate(Player player, PLAYER_ACTION action) {
        return new PlayerUpdate(PlayerUpdateID++, player, action);
    }

    // TimerUpdate ------------------------------------------------------------------------------------------------------------------------
    private ulong TimerUpdateID = 0;
    public TimerUpdate NewTimerUpdate(double time, TIMER_STATE state) {
        return new TimerUpdate(TimerUpdateID++, time, state);
    }

    // Reset ------------------------------------------------------------------------------------------------------------------------------
    public void Reset() {
        GamePlayUpdateID = 0;
        KeyUpdateID = 0;
        PingUpdateID = 0;
        PlayerUpdateID = 0;
        TimerUpdateID = 0;
    }
}

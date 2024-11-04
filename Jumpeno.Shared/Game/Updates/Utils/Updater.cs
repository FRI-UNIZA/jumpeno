namespace Jumpeno.Shared.Utils;

public class Updater {
    // AvatarUpdate -----------------------------------------------------------------------------------------------------------------------
    public AvatarUpdate NewAvatarUpdate(byte playerID, Avatar avatar) {
        return new AvatarUpdate(playerID, avatar);
    }

    // GamePlayUpdate ---------------------------------------------------------------------------------------------------------------------
    private ulong GamePlayUpdateID = 0;
    public GamePlayUpdate NewGamePlayUpdate(double time, GAME_STATE state, List<AvatarUpdate>? updates = null) {
        return new GamePlayUpdate(GamePlayUpdateID++, time, state, updates);
    }

    // KeyUpdate --------------------------------------------------------------------------------------------------------------------------
    private ulong KeyUpdateID = 0;
    public KeyUpdate NewKeyUpdate(byte playerID, GAME_CONTROLS key, bool pressed) {
        return new KeyUpdate(KeyUpdateID++, playerID, key, pressed);
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
        PlayerUpdateID = 0;
        TimerUpdateID = 0;
    }
}

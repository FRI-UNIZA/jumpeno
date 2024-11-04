namespace Jumpeno.Shared.Models;

public class GamePlayUpdate(ulong id, double time, GAME_STATE state, List<AvatarUpdate>? updates = null): GameUpdate(id) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public override string HUB_ACTION => GAME_HUB.GAME_PLAY_UPDATE;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public double Time { get; private set; } = time;
    public GAME_STATE State { get; private set; } = state;
    public List<AvatarUpdate> Updates { get; private set; } = updates ?? [];
    
    // Indexes ----------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public Dictionary<byte, AvatarUpdate> PlayerUpdates { get; private set; } = updates?.ToDictionary(u => u.PlayerID) ?? [];
}

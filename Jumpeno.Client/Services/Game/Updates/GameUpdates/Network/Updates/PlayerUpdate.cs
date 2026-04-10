namespace Jumpeno.Client.Models;

public class PlayerUpdate(
    // Identifiers:
    ulong id, int round,
    // Parameters:
    bool hostConnected, Player player, int readyForRound, bool invalidate,
    GamePlayUpdate? gamePlayUpdate = null
)
: NetworkUpdate(id, round)
{
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public override string HUB_ACTION => GameHubs.PLAYER_UPDATE;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public bool HostConnected { get; private set; } = hostConnected;
    public Player Player { get; private set; } = player;
    public int ReadyForRound { get; private set; } = readyForRound;
    public bool Invalidate { get; private set; } = invalidate;
    public GamePlayUpdate? GamePlayUpdate { get; private set; } = gamePlayUpdate;

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}

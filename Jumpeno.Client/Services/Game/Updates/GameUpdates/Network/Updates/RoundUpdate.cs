namespace Jumpeno.Client.Models;

public class RoundUpdate : NetworkUpdate {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public override string HUB_ACTION => GAME_HUB.ROUND_UPDATE;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public StateUpdate StateUpdate { get; private set; }
    public Dictionary<byte, Player> Players { get; private set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    public RoundUpdate(ulong id, int round, StateUpdate stateUpdate, Dictionary<byte, Player> players) : base(id, round) {
        StateUpdate = stateUpdate;
        Players = players;
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}

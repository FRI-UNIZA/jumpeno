namespace Jumpeno.Client.Models;

public class GamePlayUpdate : NetworkUpdate, IRespondable<GamePlayResponse> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public override string HUB_ACTION => GAME_HUB.GAME_PLAY_UPDATE;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public StateUpdate StateUpdate { get; private set; }
    public Dictionary<byte, MovementUpdate> Movements { get; private set; }
    public Dictionary<byte, KillUpdate> Kills { get; private set; } // NOTE: By dead player ID
    public Dictionary<byte, LifeUpdate> Lives { get; private set; }
    [JsonIgnore]
    public GamePlayResponse Response { get; set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    public GamePlayUpdate(
        ulong id, int round, StateUpdate stateUpdate,
        Dictionary<byte, MovementUpdate>? movements = null,
        Dictionary<byte, KillUpdate>? kills = null,
        Dictionary<byte, LifeUpdate>? lives = null
    ) : base(id, round) {
        StateUpdate = stateUpdate;
        Movements = movements ?? [];
        Kills = kills ?? [];
        Lives = lives ?? [];
        Response = new();
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}

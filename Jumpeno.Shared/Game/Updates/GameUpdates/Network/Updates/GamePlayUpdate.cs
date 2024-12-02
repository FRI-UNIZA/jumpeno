namespace Jumpeno.Shared.Models;

public class GamePlayUpdate : NetworkUpdate, IRespondable<GamePlayResponse> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public override string HUB_ACTION => GAME_HUB.GAME_PLAY_UPDATE;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public DateTime CreatedAt { get; private set; }
    public StateUpdate StateUpdate { get; private set; }
    public Dictionary<byte, MovementUpdate> Movements { get; private set; }
    public Dictionary<byte, KillUpdate> Kills { get; private set; } // NOTE: By dead player ID
    [JsonIgnore]
    public GamePlayResponse Response { get; set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private GamePlayUpdate(
        ulong id, DateTime createdAt, StateUpdate stateUpdate,
        Dictionary<byte, MovementUpdate>? movements = null,
        Dictionary<byte, KillUpdate>? kills = null
    ) : base(id) {
        CreatedAt = createdAt;
        StateUpdate = stateUpdate;
        Movements = movements ?? [];
        Kills = kills ?? [];
        Response = new();
    }
    
    public GamePlayUpdate(
        ulong id, StateUpdate stateUpdate,
        Dictionary<byte, MovementUpdate>? movements = null,
        Dictionary<byte, KillUpdate>? kills = null
    ) : this(id, DateTime.UtcNow, stateUpdate, movements, kills) {}

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}

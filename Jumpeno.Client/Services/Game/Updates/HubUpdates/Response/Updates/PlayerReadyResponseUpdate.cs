namespace Jumpeno.Client.Models;

public class PlayerReadyResponseUpdate : GameResponseUpdate
{
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public override string HUB_ACTION => GameHubs.PLAYER_READY_RESPONSE_UPDATE;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    public PlayerReadyResponseUpdate(AppExceptionDTO? exception = null) : base(exception) {}
    public PlayerReadyResponseUpdate(Exception? exception) : this(DTO(exception)) {}

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}

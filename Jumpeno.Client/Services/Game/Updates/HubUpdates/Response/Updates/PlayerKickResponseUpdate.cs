namespace Jumpeno.Client.Models;

public class PlayerKickResponseUpdate : GameResponseUpdate
{
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public override string HUB_ACTION => GameHubs.PLAYER_KICK_RESPONSE_UPDATE;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    public PlayerKickResponseUpdate(AppExceptionDTO? exception = null) : base(exception) {}
    public PlayerKickResponseUpdate(Exception? exception) : this(DTO(exception)) {}

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}

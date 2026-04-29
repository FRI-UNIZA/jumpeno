namespace Jumpeno.Client.Models;

public class PlayerKickResponseUpdate : GameResponseUpdate
{
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public override string HubAction => GameHubs.PlayerKickResponseUpdate;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    public PlayerKickResponseUpdate(AppExceptionDTO? exception = null) : base(exception) {}
    public PlayerKickResponseUpdate(Exception? exception) : this(DTO(exception)) {}

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}

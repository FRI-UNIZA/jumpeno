namespace Jumpeno.Client.Models;

public class GameActionResponseUpdate : GameResponseUpdate
{
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public override string HubAction => GameHubs.GameActionResponseUpdate;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    public GameActionResponseUpdate(AppExceptionDTO? exception = null) : base(exception) {}
    public GameActionResponseUpdate(Exception? exception) : this(DTO(exception)) {}

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}

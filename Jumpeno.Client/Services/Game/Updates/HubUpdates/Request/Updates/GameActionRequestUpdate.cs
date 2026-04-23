namespace Jumpeno.Client.Models;

public class GameActionRequestUpdate(GameAction action) : GameRequestUpdate
{
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public override string HubAction => GameHubs.GameActionRequestUpdate;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public GameAction Action { get; private set; } = action;
    
    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}

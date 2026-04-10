namespace Jumpeno.Client.Models;

public class GameActionRequestUpdate(GameAction action) : GameRequestUpdate
{
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public override string HUB_ACTION => GameHubs.GAME_ACTION_REQUEST_UPDATE;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public GameAction Action { get; private set; } = action;
    
    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}

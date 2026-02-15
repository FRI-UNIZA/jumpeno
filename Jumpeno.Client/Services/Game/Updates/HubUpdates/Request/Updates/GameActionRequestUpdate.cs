namespace Jumpeno.Client.Models;

public class GameActionRequestUpdate(GAME_ACTION action) : GameRequestUpdate
{
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public override string HUB_ACTION => GAME_HUB.GAME_ACTION_REQUEST_UPDATE;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public GAME_ACTION Action { get; private set; } = action;
    
    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}

namespace Jumpeno.Client.Models;

public class PlayerReadyRequestUpdate : GameRequestUpdate
{
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public override string HUB_ACTION => GameHubs.PLAYER_READY_REQUEST_UPDATE;
    
    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}

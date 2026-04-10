namespace Jumpeno.Client.Models;

public class PlayerKickRequestUpdate(string name) : GameRequestUpdate
{
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public override string HUB_ACTION => GameHubs.PLAYER_KICK_REQUEST_UPDATE;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public string Name { get; private set; } = name;

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}

namespace Jumpeno.Client.Models;

public class PlayerKickRequestUpdate(string name) : GameRequestUpdate
{
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public override string HubAction => GameHubs.PlayerKickRequestUpdate;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public string Name { get; private set; } = name;

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}

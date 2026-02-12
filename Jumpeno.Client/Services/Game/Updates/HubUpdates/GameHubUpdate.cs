namespace Jumpeno.Client.Models;

public abstract class GameHubUpdate
{
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public abstract string HUB_ACTION { get; }
}

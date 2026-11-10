namespace Jumpeno.Client.Models;

public abstract class GameHubUpdate
{
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public abstract string HubAction { get; }
}

namespace Jumpeno.Shared.Models;

public record GamePlayResponse {
    public bool StateUpdated { get; set; } = false;
    public bool PlayersUpdated { get; set; } = false;
    public bool KillsUpdated { get; set; } = false;
    public bool Updated => StateUpdated || PlayersUpdated || KillsUpdated;
};

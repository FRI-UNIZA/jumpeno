namespace Jumpeno.Client.Models;

public record GamePlayResponse {
    public bool StateUpdated { get; set; } = false;
    public bool MoveUpdated { get; set; } = false;
    public bool LifeUpdated { get; set; } = false;
    public bool KillUpdated { get; set; } = false;
    public bool ScoreUpdated { get; set; } = false;
    public bool Updated => StateUpdated || MoveUpdated || LifeUpdated || KillUpdated || ScoreUpdated;
};

namespace Jumpeno.Shared.Constants;

public static class GAME_HUB {
    // Routes -----------------------------------------------------------------------------------------------------------------------------
    public static readonly string ROUTE_EN = $"/en{AppSettings.Hub.Game.URL}";
    public static readonly string ROUTE_SK = $"/sk{AppSettings.Hub.Game.URL}";
    public static readonly Func<string> ROUTE_CULTURE = () => $"/{I18N.Culture()}{AppSettings.Hub.Game.URL}";

    // Client updates ---------------------------------------------------------------------------------------------------------------------
    public const string KEY_UPDATE = "KeyUpdate";

    // Server updates ---------------------------------------------------------------------------------------------------------------------
    public const string CONNECTION_SUCCESSFUL = "ConnectionSuccessful";
    public const string GAME_PLAY_UPDATE = "GamePlayUpdate";
    public const string PING_UPDATE = "PingUpdate";
    public const string PLAYER_UPDATE = "PlayerUpdate";
    public const string TIMER_UPDATE = "TimerUpdate";
    public const string ERROR = "Error";
}

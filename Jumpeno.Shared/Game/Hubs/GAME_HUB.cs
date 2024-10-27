public static class GAME_HUB {
    // Routes -----------------------------------------------------------------------------------------------------------------------------
    public static readonly string ROUTE_EN = $"/en{AppSettings.Hub.Game.URL}";
    public static readonly string ROUTE_SK = $"/sk{AppSettings.Hub.Game.URL}";
    public static readonly Func<string> ROUTE_CULTURE = () => $"/{I18N.Culture()}{AppSettings.Hub.Game.URL}";

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public const string CONNECT_TO_GAME_REQUEST = "ConnectToGameRequest";
    public const string CONNECT_TO_GAME_RESPONSE = "ConnectToGameResponse";
}

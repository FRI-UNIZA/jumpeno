namespace Jumpeno.Client.Constants;

public static class GameHubs {
    // Routes -----------------------------------------------------------------------------------------------------------------------------
    public static string Url => HUB.Base.Game;

    // Data Transfer Object ---------------------------------------------------------------------------------------------------------------
    public const string DtoType = nameof(DtoType);
    public const string Dto = nameof(Dto);

    // Client updates ---------------------------------------------------------------------------------------------------------------------
    // Game:
    public const string KeyUpdate = "KeyUpdate";
    // Request:
    public const string GameActionRequestUpdate = "GameActionRequestUpdate";
    public const string PlayerKickRequestUpdate = "PlayerKickRequestUpdate";
    public const string PlayerReadyRequestUpdate = "PlayerReadyRequestUpdate";

    // Trip updates -----------------------------------------------------------------------------------------------------------------------
    public const string PingUpdate = "PingUpdate";

    // Server updates ---------------------------------------------------------------------------------------------------------------------
    public const string ConnectionSuccessful = "ConnectionSuccessful";
    // Game:
    public const string GamePlayUpdate = "GamePlayUpdate";
    public const string PlayerUpdate = "PlayerUpdate";
    public const string RoundUpdate = "RoundUpdate";
    public const string SpectatorUpdate = "SpectatorUpdate";
    public const string Error = "Error";
    // Response:
    public const string GameActionResponseUpdate = "GameActionResponseUpdate";
    public const string PlayerKickResponseUpdate = "PlayerKickResponseUpdate";
    public const string PlayerReadyResponseUpdate = "PlayerReadyResponseUpdate";
}

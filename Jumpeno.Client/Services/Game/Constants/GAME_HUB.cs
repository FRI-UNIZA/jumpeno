namespace Jumpeno.Client.Constants;

public static class GAME_HUB {
    // Routes -----------------------------------------------------------------------------------------------------------------------------
    public static string URL => HUB.BASE.GAME;

    // Data Transfer Object ---------------------------------------------------------------------------------------------------------------
    public const string DTO_TYPE = nameof(DTO_TYPE);
    public const string DTO = nameof(DTO);

    // Client updates ---------------------------------------------------------------------------------------------------------------------
    // Game:
    public const string KEY_UPDATE = "KeyUpdate";
    // Request:
    public const string GAME_ACTION_REQUEST_UPDATE = "GameActionRequestUpdate";

    // Trip updates -----------------------------------------------------------------------------------------------------------------------
    public const string PING_UPDATE = "PingUpdate";

    // Server updates ---------------------------------------------------------------------------------------------------------------------
    public const string CONNECTION_SUCCESSFUL = "ConnectionSuccessful";
    // Game:
    public const string GAME_PLAY_UPDATE = "GamePlayUpdate";
    public const string PLAYER_UPDATE = "PlayerUpdate";
    public const string ROUND_UPDATE = "RoundUpdate";
    public const string SPECTATOR_UPDATE = "SpectatorUpdate";
    public const string ERROR = "Error";
    // Response:
    public const string GAME_ACTION_RESPONSE_UPDATE = "GameActionResponseUpdate";
}

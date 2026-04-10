namespace Jumpeno.Client.Constants;

public static class GameHubs {
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
    public const string PLAYER_KICK_REQUEST_UPDATE = "PlayerKickRequestUpdate";
    public const string PLAYER_READY_REQUEST_UPDATE = "PlayerReadyRequestUpdate";

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
    public const string PLAYER_KICK_RESPONSE_UPDATE = "PlayerKickResponseUpdate";
    public const string PLAYER_READY_RESPONSE_UPDATE = "PlayerReadyResponseUpdate";
}

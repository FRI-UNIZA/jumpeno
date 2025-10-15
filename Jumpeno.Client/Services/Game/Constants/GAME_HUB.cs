namespace Jumpeno.Client.Constants;

public static class GAME_HUB {
    // Routes -----------------------------------------------------------------------------------------------------------------------------
    public static string URL => HUB.BASE.GAME;

    // Params -----------------------------------------------------------------------------------------------------------------------------
    public const string PARAM_GAME_PARAMS_TYPE = "GameParamsType";
    public const string PARAM_GAME_PARAMS = "GameParams";
    // User:
    public const string PARAM_DEVICE_TYPE = "DeviceType";
    public const string PARAM_NAME = "Name";
    public const string PARAM_SKIN = "Skin";
    public const string PARAM_ACCESS_TOKEN = "AccessToken";
    // Game:
    public const string PARAM_CODE = "Code";
    public const string PARAM_GAME_NAME = "GameName";
    public const string PARAM_MAP = "Map";
    public const string PARAM_GAME_MODE = "GameMode";
    public const string PARAM_DISPLAY_MODE = "DisplayMode";
    public const string PARAM_ROUNDS = "Rounds";
    public const string PARAM_CAPACITY = "Capacity";
    public const string PARAM_ANONYMS = "Anonyms";

    // Client updates ---------------------------------------------------------------------------------------------------------------------
    public const string KEY_UPDATE = "KeyUpdate";
    public const string PING_UPDATE = "PingUpdate";

    // Server updates ---------------------------------------------------------------------------------------------------------------------
    public const string CONNECTION_SUCCESSFUL = "ConnectionSuccessful";
    public const string GAME_PLAY_UPDATE = "GamePlayUpdate";
    public const string PLAYER_UPDATE = "PlayerUpdate";
    public const string ROUND_UPDATE = "RoundUpdate";
    public const string SPECTATOR_UPDATE = "SpectatorUpdate";
    public const string ERROR = "Error";
}

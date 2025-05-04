namespace Jumpeno.Shared.Constants;

public static class GAME_HUB {
    // Routes -----------------------------------------------------------------------------------------------------------------------------
    public static string URL => HUB.BASE.GAME;

    // Params -----------------------------------------------------------------------------------------------------------------------------
    public const string PARAM_CODE = nameof(Game.Code);
    public const string PARAM_USER = nameof(User);
    public const string PARAM_DEVICE = nameof(Connection.Device);
    public const string PARAM_SPECTATOR = nameof(Spectator);

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

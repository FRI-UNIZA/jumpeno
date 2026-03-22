namespace Jumpeno.Client.Constants;

public static class CHAT_HUB {
    // Routes -------------------------------------------------------------------------------------------------------------------------
    public static string URL => HUB.BASE.CHAT;
    // Params -------------------------------------------------------------------------------------------------------------------------
    public const string PARAM_CHAT_PARAMS_TYPE = "ChatParamsType";
    public const string PARAM_CHAT_PARAMS = "ChatParams";
    public const string PARAM_ACCESS_TOKEN = "AccessToken";
    public const string GLOBAL_GROUP = "GlobalChat";
    // Client updates -----------------------------------------------------------------------------------------------------------------
    public const string RECEIVE_GLOBAL_MESSAGE = "ReceiveGlobalMessage";
    // Server updates -----------------------------------------------------------------------------------------------------------------
    public const string SEND_GLOBAL_MESSAGE = "SendGlobalMessage";
    public const string CONNECTION_SUCCESSFUL = "ConnectionSuccessful";
    public const string ERROR = "Error";
}

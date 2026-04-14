namespace Jumpeno.Client.Constants;

public static class ChatHubConstants {
    // Routes -------------------------------------------------------------------------------------------------------------------------
    public static string URL => HUB.BASE.CHAT;
    // Params -------------------------------------------------------------------------------------------------------------------------
    public const string ParamChatParamsType = "ChatParamsType";
    public const string ParamChatParams = "ChatParams";
    public const string ParamAccessToken = "AccessToken";
    public const string GlobalGroup = "GlobalChat";
    // Client updates -----------------------------------------------------------------------------------------------------------------
    public const string ReceiveGlobalMessage = "ReceiveGlobalMessage";
    // Server updates -----------------------------------------------------------------------------------------------------------------
    public const string SendGlobalMessage = "SendGlobalMessage";
    public const string ConnectionSuccessful = "ConnectionSuccessful";
    public const string Error = "Error";
}

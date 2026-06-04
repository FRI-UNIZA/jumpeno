namespace Jumpeno.Client.Constants;

public static class ChatHubConstants {
    // Routes -------------------------------------------------------------------------------------------------------------------------
    public static string URL => HUB.Base.Chat;
    // Params -------------------------------------------------------------------------------------------------------------------------
    public const string ParamAccessToken = "AccessToken";
    public const string ParamLastMessageId = "LastMessageId";
    // Client updates -----------------------------------------------------------------------------------------------------------------
    public const string ReceiveGlobalMessage = "ReceiveGlobalMessage";
    // Server updates -----------------------------------------------------------------------------------------------------------------
    public const string SendGlobalMessage = "SendGlobalMessage";
    public const string ConnectionSuccessful = "ConnectionSuccessful";
    public const string Error = "Error";
}

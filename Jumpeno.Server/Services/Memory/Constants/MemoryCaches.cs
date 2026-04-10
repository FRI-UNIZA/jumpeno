namespace Jumpeno.Server.Constants;

public static class MemoryCaches
{
    public static string IP_ATTEMPT(ATTEMPTS_CATEGORY category, string ip) => $"{nameof(IP_ATTEMPT)}:{category}:{ip}";
    public static string USER_ATTEMPT(string email) => $"{nameof(USER_ATTEMPT)}:{email}";

}

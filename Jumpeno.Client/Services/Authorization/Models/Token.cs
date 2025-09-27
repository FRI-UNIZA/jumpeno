namespace Jumpeno.Client.Utils;

#pragma warning disable IDE1006

public class Token {
    // Token structure --------------------------------------------------------------------------------------------------------------------
    public record Data(
        // Token encoded:
        string raw,
        // Token data:
        TOKEN_TYPE type,
        string sub,
        ROLE role,
        string iss,
        string aud,
        DateTime iat,
        DateTime exp,
        string data
    ) {};

    // Token storage ----------------------------------------------------------------------------------------------------------------------
    public static Data Access => RequestStorage.Get<Data>(REQUEST_STORAGE.TOKEN_ACCESS) ?? throw EXCEPTION.NOT_AUTHENTICATED;
    public static Data Refresh => RequestStorage.Get<Data>(REQUEST_STORAGE.TOKEN_REFRESH) ?? throw EXCEPTION.NOT_AUTHENTICATED;
    public static Data Activation => RequestStorage.Get<Data>(REQUEST_STORAGE.TOKEN_ACTIVATION) ?? throw EXCEPTION.NOT_AUTHENTICATED;
    public static Data PasswordReset => RequestStorage.Get<Data>(REQUEST_STORAGE.TOKEN_PASSWORD_RESET) ?? throw EXCEPTION.NOT_AUTHENTICATED;

    private static void Store(string key, string token) {
        if (AppEnvironment.IsServer && !AppEnvironment.IsController) throw EXCEPTION.NOT_AUTHENTICATED;
        var data = Decode(token) ?? throw EXCEPTION.NOT_AUTHENTICATED;
        RequestStorage.Set(key, data);
    }
    public static void StoreAccess(string token) => Store(TOKEN_TYPE.ACCESS.String(), token);
    public static void StoreRefresh(string token) => Store(TOKEN_TYPE.REFRESH.String(), token);
    public static void StoreActivation(string token) => Store(TOKEN_TYPE.ACTIVATION.String(), token);
    public static void StorePasswordReset(string token) => Store(TOKEN_TYPE.PASSWORD_RESET.String(), token);
    
    private static void Delete(string key) {
        if (AppEnvironment.IsServer && !AppEnvironment.IsController) throw EXCEPTION.NOT_AUTHENTICATED;
        RequestStorage.Delete(key);
    }
    public static void DeleteAccess() => Delete(TOKEN_TYPE.ACCESS.String());
    public static void DeleteRefresh() => Delete(TOKEN_TYPE.REFRESH.String());
    public static void DeleteActivation() => Delete(TOKEN_TYPE.ACTIVATION.String());
    public static void DeletePasswordReset() => Delete(TOKEN_TYPE.PASSWORD_RESET.String());

    // Decoding ---------------------------------------------------------------------------------------------------------------------------
    public static Data? Decode(string token) {
        try {
            var parts = token.Split('.');
            if (parts.Length != 3) return null;

            var payload = parts[1];
            var jsonBytes = Convert.FromBase64String(
                payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=')
            );
            var json = System.Text.Encoding.UTF8.GetString(jsonBytes);

            var principal = JsonSerializer.Deserialize<Dictionary<string, object>>(json)!;
            return new Data(
                raw: token,
                type: Enum.Parse<TOKEN_TYPE>(principal[nameof(Data.type)].ToString()!),
                sub: principal[nameof(Data.sub)].ToString()!,
                role: Enum.Parse<ROLE>(principal[nameof(Data.role)].ToString()!),
                iss: principal[nameof(Data.iss)].ToString()!,
                aud: principal[nameof(Data.aud)].ToString()!,
                iat: From.UnixToDateTime(long.Parse(principal[nameof(Data.iat)].ToString()!)),
                exp: From.UnixToDateTime(long.Parse(principal[nameof(Data.exp)].ToString()!)),
                data: principal[nameof(Data.data)].ToString()!
            );
        } catch {
            return null;
        }
    }
}

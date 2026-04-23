namespace Jumpeno.Client.Utils;

#pragma warning disable IDE1006

public class Token {
    // Token structure --------------------------------------------------------------------------------------------------------------------
    public record Data(
        // Token encoded:
        string raw,
        // Token data:
        TokenType type,
        string sub,
        Role role,
        string iss,
        string aud,
        DateTime iat,
        DateTime exp,
        string data
    ) {};

    // Token storage ----------------------------------------------------------------------------------------------------------------------
    public static Data Access => RequestStorage.Get<Data>(RequestStorages.TokenAccess) ?? throw Exceptions.NotAuthenticated;
    public static Data Refresh => RequestStorage.Get<Data>(RequestStorages.TokenRefresh) ?? throw Exceptions.NotAuthenticated;
    public static Data Activation => RequestStorage.Get<Data>(RequestStorages.TokenActivation) ?? throw Exceptions.NotAuthenticated;
    public static Data PasswordReset => RequestStorage.Get<Data>(RequestStorages.TokenPasswordReset) ?? throw Exceptions.NotAuthenticated;

    private static void Store(string key, string token) {
        if (AppEnvironment.IsServer && !AppEnvironment.IsController) throw Exceptions.NotAuthenticated;
        var data = Decode(token) ?? throw Exceptions.NotAuthenticated;
        RequestStorage.Set(key, data);
    }
    public static void StoreAccess(string token) => Store(TokenType.Access.String(), token);
    public static void StoreRefresh(string token) => Store(TokenType.Refresh.String(), token);
    public static void StoreActivation(string token) => Store(TokenType.Activation.String(), token);
    public static void StorePasswordReset(string token) => Store(TokenType.PasswordReset.String(), token);
    
    private static void Delete(string key) {
        if (AppEnvironment.IsServer && !AppEnvironment.IsController) throw Exceptions.NotAuthenticated;
        RequestStorage.Delete(key);
    }
    public static void DeleteAccess() => Delete(TokenType.Access.String());
    public static void DeleteRefresh() => Delete(TokenType.Refresh.String());
    public static void DeleteActivation() => Delete(TokenType.Activation.String());
    public static void DeletePasswordReset() => Delete(TokenType.PasswordReset.String());

    // Decoding ---------------------------------------------------------------------------------------------------------------------------
    public static Data? Decode(string token) {
        try {
            var parts = token.Split('.');
            if (parts.Length != 3) return null;

            var payload = parts[1];
            var jsonBytes = Convert.FromBase64String(
                payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=')
            );
            var json = Encoding.UTF8.GetString(jsonBytes);

            var principal = JsonSerializer.Deserialize<Dictionary<string, object>>(json)!;
            return new Data(
                raw: token,
                type: Enum.Parse<TokenType>(principal[nameof(Data.type)].ToString()!),
                sub: principal[nameof(Data.sub)].ToString()!,
                role: Enum.Parse<Role>(principal[nameof(Data.role)].ToString()!),
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

namespace Jumpeno.Shared.Utils;

#pragma warning disable IDE1006

public class Token {
    // Token structure --------------------------------------------------------------------------------------------------------------------
    public record Data(
        // Token encoded:
        string raw,
        // Token decoded:
        TOKEN_TYPE type,
        string sub,
        ROLE role,
        string iss,
        string aud,
        long iat,
        long exp
    ) {};

    // Token storage ----------------------------------------------------------------------------------------------------------------------
    public static Data Access => RequestStorage.Get<Data>(TOKEN_TYPE.ACCESS.String()) ?? throw Exceptions.NotAuthenticated;
    public static Data Refresh => RequestStorage.Get<Data>(TOKEN_TYPE.REFRESH.String()) ?? throw Exceptions.NotAuthenticated;
    public static Data Activation => RequestStorage.Get<Data>(TOKEN_TYPE.ACTIVATION.String()) ?? throw Exceptions.NotAuthenticated;

    private static void Store(string key, string token) {
        if (AppEnvironment.IsServer && !AppEnvironment.IsController) throw Exceptions.NotAuthenticated;
        var data = Decode(token) ?? throw Exceptions.NotAuthenticated;
        RequestStorage.Set(key, data);
    }
    public static void StoreAccess(string token) => Store(TOKEN_TYPE.ACCESS.String(), token);
    public static void StoreRefresh(string token) => Store(TOKEN_TYPE.REFRESH.String(), token);
    public static void StoreActivation(string token) => Store(TOKEN_TYPE.ACTIVATION.String(), token);

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
                iat: long.Parse(principal[nameof(Data.iat)].ToString()!),
                exp: long.Parse(principal[nameof(Data.exp)].ToString()!)
            );
        } catch {
            return null;
        }
    }
}

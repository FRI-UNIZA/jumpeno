namespace Jumpeno.Shared.Models;

#pragma warning disable IDE1006

public class AuthToken {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public static string sub => Token.sub;
    public static ROLE role => Token.role;
    public static string iss => Token.iss;
    public static string aud => Token.aud;
    public static long iat => Token.iat;
    public static long exp => Token.exp;

    // Data -------------------------------------------------------------------------------------------------------------------------------
    private record Data(
        string sub,
        ROLE role,
        string iss,
        string aud,
        long iat,
        long exp
    ) {};
    private static Data Token => RequestStorage.Get<Data>(nameof(AuthToken)) ?? throw Exceptions.NotAuthenticated;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static void Decode(string token) {
        if (AppEnvironment.IsServer && !AppEnvironment.IsController) throw Exceptions.NotAuthenticated;
        try {
            var parts = token.Split('.');
            if (parts.Length != 3) throw Exceptions.NotAuthenticated;

            var payload = parts[1];
            var jsonBytes = Convert.FromBase64String(
                payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=')
            );
            var json = System.Text.Encoding.UTF8.GetString(jsonBytes);

            var principal = JsonSerializer.Deserialize<Dictionary<string, object>>(json)!;
            var data = new Data(
                sub: principal[nameof(sub)].ToString()!,
                role: Enum.Parse<ROLE>(principal[nameof(role)].ToString()!),
                iss: principal[nameof(iss)].ToString()!,
                aud: principal[nameof(aud)].ToString()!,
                iat: long.Parse(principal[nameof(iat)].ToString()!),
                exp: long.Parse(principal[nameof(exp)].ToString()!)
            );
            RequestStorage.Set(nameof(AuthToken), data);
        } catch {
            throw Exceptions.NotAuthenticated;
        }
    }
}

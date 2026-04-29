namespace Jumpeno.Client.Models;

public class Cookie(
    Enum key, string value, DateTimeOffset? expires = null,
    string? domain = null, string? path = null,
    bool httpOnly = false, bool secure = true,
    SameSite sameSite = SameSite.Strict
) {
    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public static string? NormDomain(string domain) => domain == Cookies.DefaultDomain ? null : domain;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public Enum Key { get; set; } = key;
    public string Value { get; set; } = value;
    public DateTimeOffset? Expires { get; set; } = expires is null ? expires : ((DateTimeOffset)expires).UtcDateTime;
    public string Domain { get; set; } = domain is null ? Cookies.DefaultDomain : domain;
    public string Path { get; set; } = path is null ? Cookies.DefaultPath : path;
    public bool HttpOnly { get; set; } = httpOnly;
    public bool Secure { get; set; } = secure;
    public SameSite SameSite { get; set; } = sameSite;
}

namespace Jumpeno.Shared.Models;

public class Cookie(
    Enum key, string value, DateTimeOffset? expires = null,
    string? domain = null, string? path = null,
    bool httpOnly = false, bool secure = true,
    SAME_SITE sameSite = SAME_SITE.STRICT
) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static string DEFAULT_DOMAIN() { return URL.Domain(); }
    public static string DEFAULT_PATH() { return "/"; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public Enum Key { get; set; } = key;
    public string Value { get; set; } = value;
    public DateTimeOffset? Expires { get; set; } = expires is null ? expires : ((DateTimeOffset)expires).UtcDateTime;
    public string Domain { get; set; } = domain is null ? DEFAULT_DOMAIN() : domain;
    public string Path { get; set; } = path is null ? DEFAULT_PATH() : path;
    public bool HttpOnly { get; set; } = httpOnly;
    public bool Secure { get; set; } = secure;
    public SAME_SITE SameSite { get; set; } = sameSite;
}

namespace Jumpeno.Client.Enums;

public enum SameSite {
    [StringValue("Unspecified")] Unspecified = -1,
    [StringValue("None")] None = 0,
    [StringValue("Lax")] Lax = 1,
    [StringValue("Strict")] Strict = 2
}

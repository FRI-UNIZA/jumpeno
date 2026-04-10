namespace Jumpeno.Client.Constants;

public enum SameSite {
    [StringValue("Unspecified")] UNSPECIFIED = -1,
    [StringValue("None")] NONE = 0,
    [StringValue("Lax")] LAX = 1,
    [StringValue("Strict")] STRICT = 2
}

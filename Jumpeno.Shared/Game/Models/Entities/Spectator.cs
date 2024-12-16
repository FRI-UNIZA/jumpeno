namespace Jumpeno.Shared.Models;

public class Spectator {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string DEVICE_ID = "device";
    public const string WATCH_ID = "watch";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public User User { get; protected set; }
    public DEVICE_TYPE Device { get; protected set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    public Spectator(User user, DEVICE_TYPE device) {
        User = user;
        Device = device;
    }
}

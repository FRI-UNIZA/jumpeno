
namespace Jumpeno.Shared.Models;

public class Spectator : Connection {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string SPECTATOR_ID = "spectator";

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    public Spectator(string? connectionID, User user, DEVICE_TYPE device) : base(connectionID, user, device) {}
    public Spectator(Connection connection) : this(connection.ConnectionID, connection.User, connection.Device) {}
}

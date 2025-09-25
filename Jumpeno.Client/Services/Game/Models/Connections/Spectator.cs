namespace Jumpeno.Client.Models;

public class Spectator : Connection {
    [JsonConstructor]
    public Spectator(string? connectionID, User user, DEVICE_TYPE device) : base(connectionID, user, device) {}
    public Spectator(Connection connection) : this(connection.ConnectionID, connection.User, connection.Device) {}
}

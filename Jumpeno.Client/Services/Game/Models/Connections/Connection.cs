namespace Jumpeno.Client.Models;

public class Connection {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public string? ConnectionID { get; protected set; }
    public User User { get; protected set; }
    public DeviceType Device { get; protected set; }

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    public bool IsConnected => ConnectionID != null;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    public Connection(string? connectionID, User user, DeviceType device) {
        ConnectionID = connectionID;
        User = user;
        Device = device;
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public void Connect(string connectionID, DeviceType device) {
        ConnectionID = connectionID;
        Device = device;
    }

    public void Synchronize(string? connectionID, User user, DeviceType device) {
        ConnectionID = connectionID;
        User = user;
        Device = device;
    }
    public void Synchronize(Connection connection) => Synchronize(connection.ConnectionID, connection.User, connection.Device);

    public void Disconnect() => ConnectionID = null;
}

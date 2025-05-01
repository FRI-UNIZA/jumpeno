namespace Jumpeno.Shared.Models;

// NOTE: Error should contain only untranslated message!
//       Values are used in message translation.
public class Error {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public string ID { get; set; }
    public string Message { get; set; }
    public Dictionary<string, string> Values { get; set; }
    public bool Client { get; private set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [method: JsonConstructor]
    [method: Newtonsoft.Json.JsonConstructor]
    protected Error(string id, string message, Dictionary<string, string>? values, bool client) {
        ID = id;
        Message = client ? I18N.T(message, values, unsplit: true) : message;
        Values = values ?? [];
        Client = client;
    }
    public Error(string message) : this("", message, [], AppEnvironment.IsClient) {}
    public Error(string message, Dictionary<string, string> values) : this("", message, values, AppEnvironment.IsClient) {}
    public Error(string id, string message) : this(id, message, null, AppEnvironment.IsClient) {}
    public Error(string id, string message, Dictionary<string, string> values) : this(id, message, values, AppEnvironment.IsClient) {}
}

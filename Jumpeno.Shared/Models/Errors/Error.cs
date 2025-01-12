namespace Jumpeno.Shared.Models;

// NOTE: Error should contain only untranslated message!
//       Values are used in message translation.
[method: JsonConstructor]
[method: Newtonsoft.Json.JsonConstructor]
public class Error(string id, string message, Dictionary<string, string>? values = null) {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public string ID { get; set; } = id;
    public string Message { get; set; } = message;
    public Dictionary<string, string> Values { get; set; } = values ?? [];

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public Error(string message) : this("", message, []) {}
    public Error(string message, Dictionary<string, string> values) : this("", message, values) {}
}

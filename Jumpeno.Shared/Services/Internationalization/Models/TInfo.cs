namespace Jumpeno.Shared.Models;

public class TInfo {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public string Key { get; private set; }
    public Dictionary<string, object>? Values { get; private set; }
    public string T { get; private set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor][Newtonsoft.Json.JsonConstructor]
    protected TInfo(string key, Dictionary<string, object>? values, string t) {
        Key = key;
        Values = values;
        T = t;
    }
    public TInfo(string key, Dictionary<string, object>? values = null) : this(key, values, I18N.T(key, values)) {}
    public TInfo(TInfo message) : this(message.Key, message.Values, message.T) {}
}

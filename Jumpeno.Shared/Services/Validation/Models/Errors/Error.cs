namespace Jumpeno.Shared.Models;

public class Error {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string DEFAULT_ID = "";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public string ID { get; private set; }
    public Error SetID(string id) { ID = id; return this; }

    public TInfo Info { get; private set; }
    public Error SetInfo(string key, Dictionary<string, object>? values = null) { Info = new(key, values); return this; }
    public Error SetInfo(TInfo info) { Info = new(info); return this; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor][Newtonsoft.Json.JsonConstructor]
    protected Error(string id, TInfo info) { ID = id; Info = info; }
    public Error() : this(DEFAULT_ID, new(MESSAGE.DEFAULT)) {}
}

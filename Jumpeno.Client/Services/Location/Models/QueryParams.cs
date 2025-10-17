namespace Jumpeno.Client.Models;

public class QueryParams {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private Dictionary<string, QUERY_ARRAY_TYPE> ArrayTypes;
    private Dictionary<string, StringValues> Items;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public QueryParams(Dictionary<string, QUERY_ARRAY_TYPE> arrayTypes, Dictionary<string, StringValues> items) {
        ArrayTypes = arrayTypes;
        Items = items;
    }
    public QueryParams() : this([], []) {}

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public string? GetString(string key) {
        try {
            string item = Items[key]!;
            return item == "" ? null : item;
        }
        catch { return null; }
    }
    public int? GetInt(string key) {
        try { return int.Parse(Items[key]!); }
        catch { return null; }
    }
    public double? GetDouble(string key) {
        try { return double.Parse(Items[key]!); }
        catch { return null; }
    }
    public bool? GetBool(string key) {
        try { return bool.Parse(Items[key]!); }
        catch { return null; }
    }
    public bool IsTrue(string key) => GetBool(key) is bool value && value;

    public QueryArray GetArray(string key) {
        QUERY_ARRAY_TYPE type;
        try { type = ArrayTypes[key]; }
        catch { type = QUERY_ARRAY_TYPE.REPEATED_KEY; }
        try { return new QueryArray(type, Items[key]); }
        catch { return new QueryArray(type); }
    }

    public void Set<T>(string key, T value) {
        Checker.CheckPrimitiveType(typeof(T));
        var setValue = $"{value}";
        if (setValue != "") {
            Items[key] = setValue;
        }
    }
    public void Set(string key, QueryArray value) {
        if (value.IsEmpty()) {
            Remove(key);
            return;
        }
        ArrayTypes[key] = value.Type;
        Items[key] = value.ToArray();
    }
    public bool Remove(string key) {
        ArrayTypes.Remove(key);
        return Items.Remove(key);
    }
    public void Clear() {
        ArrayTypes = [];
        Items = [];
    }

    public int Count() => Items.Count;
    public bool IsEmpty() => Count() == 0;
    public void ForEachKey(Func<string, int, bool> callback) {
        var index = 0;
        foreach (KeyValuePair<string, StringValues> item in Items) {
            if (callback(item.Key, index) == false) return;
            index++;
        }
    }

    // Conversions ------------------------------------------------------------------------------------------------------------------------
    public override string ToString() {
        var result = "";
        if (Count() == 0) return result;
        ForEachKey((string key, int index) => {
            StringValues item = Items[key];
            string value = "";

            bool isArray = false;
            if (ArrayTypes.ContainsKey(key) || item.Count() > 1) {
                isArray = true;
                QueryArray arr = GetArray(key);
                if (arr.IsEmpty()) return true;
                value = arr.ToString(key);
            } else {
                value = item!;
            }
            if (value == "") return true;

            char delimiter = index == 0 ? '?': '&';
            var equals = isArray ? "" : $"{URL.EncodeValue(key)}=";
            value = isArray ? value : URL.EncodeValue(value);
            result = $"{result}{delimiter}{equals}{value}";
            return true;
        });
        return result;
    }
}

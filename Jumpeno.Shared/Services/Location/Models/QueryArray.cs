namespace Jumpeno.Shared.Models;

public class QueryArray {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public QUERY_ARRAY_TYPE Type { get; set; }
    private List<string> Items;

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public QueryArray(QUERY_ARRAY_TYPE type, StringValues? value) {
        Type = type;
        Items = ParseArray(value);
    }
    public QueryArray(QUERY_ARRAY_TYPE type) : this(type, null) {}
    public QueryArray() : this(QUERY_ARRAY_TYPE.REPEATED_KEY, null) {}

    // Type specific ----------------------------------------------------------------------------------------------------------------------
    private List<string> ParseArray(StringValues? value) {
        if (value is null) return [];
        try {
            switch (Type) {
                case QUERY_ARRAY_TYPE.REPEATED_KEY:
                    return ParseArrayRepeatedKey((StringValues) value);
                default:
                    return [];
            }
        } catch { return []; }
    }
    private List<string> ParseArrayRepeatedKey(StringValues value) {
        try {
            string[] arr = value!;
            List<string> items = [];
            foreach (var v in arr) {
                if (v != "") items.Add(v);
            }
            return items;
        } catch { return []; }
    }
    private string[] ToArrayRepeatedKey() {
        return Items.ToArray();
    } 
    private string ToStringRepeatedKey(string key) {
        string result = "";
        ForEachIndex((int index) => {
            var delimiter = index == 0 ? "" : "&";
            result = $"{result}{delimiter}{URL.EncodeValue(key)}={URL.EncodeValue(Items[index])}";
            return true;
        });
        return result;
    }
    // Methods ----------------------------------------------------------------------------------------------------------------------------

    public string? GetString(int index) {
        try { return Items[index]; }
        catch { return null; }
    }
    public int? GetInt(int index) {
        try { return int.Parse(Items[index]!); }
        catch { return null; }
    }
    public double? GetDouble(int index) {
        try { return double.Parse(Items[index]!); }
        catch { return null; }
    }
    public bool? GetBool(int index) {
        try { return bool.Parse(Items[index]!); }
        catch { return null; }
    }

    public void Add<T>(T value) {
        Checker.CheckPrimitiveType(typeof(T));
        var setValue = $"{value}";
        if (setValue != "") {
            Items.Add(setValue);
        }
    }
    public void Insert<T>(int index, T value) {
        Checker.CheckPrimitiveType(typeof(T));
        var setValue = $"{value}";
        if (setValue != "") {
            Items.Insert(index, setValue);
        }
    }
    public void Set<T>(int index, T value) {
        Checker.CheckPrimitiveType(typeof(T));
        var setValue = $"{value}";
        if (setValue != "") {
            Items[index] = setValue;
        }
    }
    public void Remove(int index) {
        Items.RemoveAt(index);
    }
    public void Clear() {
        Items.Clear();
    }

    public int Length() {
        return Items.Count;
    }
    public bool IsEmpty() {
        return Length() == 0;
    }
    public void ForEachIndex(Func<int, bool> callback) {
        var index = 0;
        foreach (var item in Items) {
            if (callback(index) == false) return;
            index++;
        }
    }

    // Conversions ------------------------------------------------------------------------------------------------------------------------
    public StringValues ToArray() {
        switch (Type) {
            case QUERY_ARRAY_TYPE.REPEATED_KEY:
                return ToArrayRepeatedKey();
            default:
                return [];
        }
    }
    public string ToString(string key) {
        switch (Type) {
            case QUERY_ARRAY_TYPE.REPEATED_KEY:
                return ToStringRepeatedKey(key);
            default:
                return "";
        }
    }
}

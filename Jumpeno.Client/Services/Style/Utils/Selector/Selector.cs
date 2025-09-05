namespace Jumpeno.Client.Utils;

public static class Selector {
    public static string ID(string id) => $"#{id}";
    public static string Class(string @class) => $".{@class}";
    public static string Attribute(string attribute) => $"[{attribute}]";
    public static string Attribute(string attribute, string value) => $"[{attribute}=\"{value}\"]";
}

namespace Jumpeno.Client.Constants;

public static class JSWindow {
    public static readonly string ClassName = typeof(JSWindow).Name;

    public static readonly string AddResizeEventListener = $"{ClassName}.AddResizeEventListener";
    public static readonly string RemoveResizeEventListener = $"{ClassName}.RemoveResizeEventListener";
    public static readonly string GetSize = $"{ClassName}.GetSize";
}

namespace Jumpeno.Client.Constants;

public static class JSNavigator {
    public static readonly string ClassName = typeof(JSNavigator).Name;

    // State ------------------------------------------------------------------------------------------------------------------------------
    public static readonly string State = $"{ClassName}.State";
    public static readonly string SetState = $"{ClassName}.SetState";

    // Media ------------------------------------------------------------------------------------------------------------------------------
    public static readonly string IsTouchDevice = $"{ClassName}.IsTouchDevice";
}

namespace Jumpeno.Client.Constants;

public static class JSScrollArea {
    public static readonly string ClassName = typeof(JSScrollArea).Name;

    public const string LIGHT_THEME = "os-theme-light";
    public const string DARK_THEME = "os-theme-dark";

    public static readonly string Activate = $"{ClassName}.Activate";
    public static readonly string Destroy = $"{ClassName}.Destroy";
    public static readonly string SetTheme = $"{ClassName}.SetTheme";
    public static readonly string Update = $"{ClassName}.Update";
    public static readonly string HideScrollbars = $"{ClassName}.HideScrollbars";
    public static readonly string ShowScrollbars = $"{ClassName}.ShowScrollbars";
    public static readonly string Scroll = $"{ClassName}.Scroll";
    public static readonly string ItemPosition = $"{ClassName}.ItemPosition";
    public static readonly string Position = $"{ClassName}.Position";
    public static readonly string AddScrollListener = $"{ClassName}.AddScrollListener";
    public static readonly string RemoveScrollListener = $"{ClassName}.RemoveScrollListener";
}

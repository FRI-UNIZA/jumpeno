namespace Jumpeno.Client.Constants;

public static class JSScrollArea {
    public static readonly string ClassName = nameof(JSScrollArea);

    public const string LIGHT_THEME = "os-theme-light";
    public const string DARK_THEME = "os-theme-dark";

    public static readonly string Activate = $"{ClassName}.{nameof(Activate)}";
    public static readonly string Destroy = $"{ClassName}.{nameof(Destroy)}";
    public static readonly string SetTheme = $"{ClassName}.{nameof(SetTheme)}";
    public static readonly string Update = $"{ClassName}.{nameof(Update)}";
    public static readonly string HideScrollbars = $"{ClassName}.{nameof(HideScrollbars)}";
    public static readonly string ShowScrollbars = $"{ClassName}.{nameof(ShowScrollbars)}";
    public static readonly string Scroll = $"{ClassName}.{nameof(Scroll)}";
    public static readonly string ItemPosition = $"{ClassName}.{nameof(ItemPosition)}";
    public static readonly string Position = $"{ClassName}.{nameof(Position)}";
    public static readonly string AddScrollListener = $"{ClassName}.{nameof(AddScrollListener)}";
    public static readonly string RemoveScrollListener = $"{ClassName}.{nameof(RemoveScrollListener)}";
}

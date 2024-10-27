namespace Jumpeno.Client.Constants;

public static class JSActionHandler {
    public static readonly string ClassName = typeof(JSActionHandler).Name;

    public static readonly string DisableAutocomplete = $"{ClassName}.DisableAutocomplete";
    public static readonly string PopAutocomplete = $"{ClassName}.PopAutocomplete";
    public static readonly string EnableAutocomplete = $"{ClassName}.EnableAutocomplete";
    public static readonly string BlurFocus = $"{ClassName}.BlurFocus";
    public static readonly string PopFocus = $"{ClassName}.PopFocus";
    public static readonly string RestoreFocus = $"{ClassName}.RestoreFocus";
    public static readonly string SaveActiveElement = $"{ClassName}.SaveActiveElement";
    public static readonly string GetRestoreID = $"{ClassName}.GetRestoreID";
    public static readonly string SetFocus = $"{ClassName}.SetFocus";
    public static readonly string FocusFirst = $"{ClassName}.FocusFirst";
    public static readonly string FocusLast = $"{ClassName}.FocusLast";
    public static readonly string Clear = $"{ClassName}.Clear";
    public static readonly string DisableKeyboardActions = $"{ClassName}.DisableKeyboardActions";
    public static readonly string EnableKeyboardActions = $"{ClassName}.EnableKeyboardActions";
    public static readonly string DisableTabs = $"{ClassName}.DisableTabs";
    public static readonly string PopTabs = $"{ClassName}.PopTabs";
    public static readonly string EnableTabs = $"{ClassName}.EnableTabs";
    public static readonly string EnableTabsForDescendants = $"{ClassName}.EnableTabsForDescendants";
    public static readonly string SetInert = $"{ClassName}.SetInert";
    public static readonly string RemoveInert = $"{ClassName}.RemoveInert";
    public static readonly string Click = $"{ClassName}.Click";
}

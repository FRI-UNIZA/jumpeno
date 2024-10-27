namespace Jumpeno.Client.Services;

public class ActionHandler {
    public static void DisableAutocomplete() { JS.InvokeVoid(JSActionHandler.DisableAutocomplete); }
    public static async Task DisableAutocompleteAsync() { await JS.InvokeVoidAsync(JSActionHandler.DisableAutocomplete); }
    public static void PopAutocomplete() { JS.InvokeVoid(JSActionHandler.PopAutocomplete); }
    public static async Task PopAutocompleteAsync() { await JS.InvokeVoidAsync(JSActionHandler.PopAutocomplete); }
    public static void EnableAutocomplete() { JS.InvokeVoid(JSActionHandler.EnableAutocomplete); }
    public static async Task EnableAutocompleteAsync() { await JS.InvokeVoidAsync(JSActionHandler.EnableAutocomplete); }
    public static void BlurFocus(string focusAfterID = "") { JS.InvokeVoid(JSActionHandler.BlurFocus, focusAfterID); }
    public static async Task BlurFocusAsync(string focusAfterID = "") { await JS.InvokeVoidAsync(JSActionHandler.BlurFocus, focusAfterID); }
    public static void PopFocus() { JS.InvokeVoid(JSActionHandler.PopFocus); }
    public static async Task PopFocusAsync() { await JS.InvokeVoidAsync(JSActionHandler.PopFocus); }
    public static async Task RestoreFocusAsync() { await JS.InvokeVoidAsync(JSActionHandler.RestoreFocus); }
    public static void SaveActiveElement() { JS.InvokeVoid(JSActionHandler.SaveActiveElement); }
    public static async Task SaveActiveElementAsync() { await JS.InvokeVoidAsync(JSActionHandler.SaveActiveElement); }
    public static string? GetRestoreID() { return JS.Invoke<string?>(JSActionHandler.GetRestoreID); }
    public static async Task<string?> GetRestoreIDAsync() { return await JS.InvokeAsync<string?>(JSActionHandler.GetRestoreID); }
    public static void SetFocus(string id) { JS.InvokeVoid(JSActionHandler.SetFocus, id); }
    public static async Task SetFocusAsync(string id) { await JS.InvokeVoidAsync(JSActionHandler.SetFocus, id); }
    public static void FocusFirst(string id) { JS.InvokeVoid(JSActionHandler.FocusFirst, id); }
    public static async Task FocusFirstAsync(string id) { await JS.InvokeVoidAsync(JSActionHandler.FocusFirst, id); }
    public static void FocusLast(string id) { JS.InvokeVoid(JSActionHandler.FocusLast, id); }
    public static async Task FocusLastAsync(string id) { await JS.InvokeVoidAsync(JSActionHandler.FocusLast, id); }
    public static void DisableKeyboardActions() { JS.InvokeVoid(JSActionHandler.DisableKeyboardActions); }
    public static async Task DisableKeyboardActionsAsync() { await JS.InvokeVoidAsync(JSActionHandler.DisableKeyboardActions); }
    public static void EnableKeyboardActions() { JS.InvokeVoid(JSActionHandler.EnableKeyboardActions); }
    public static async Task EnableKeyboardActionsAsync() { await JS.InvokeVoidAsync(JSActionHandler.EnableKeyboardActions); }
    public static void DisableTabs(string exceptID = "", string exceptData = "") { JS.InvokeVoid(JSActionHandler.DisableTabs, exceptID, exceptData); }
    public static async Task DisableTabsAsync(string exceptID = "", string exceptData = "") { await JS.InvokeVoidAsync(JSActionHandler.DisableTabs, exceptID, exceptData); }
    public static void PopTabs() { JS.InvokeVoid(JSActionHandler.PopTabs); }
    public static async Task PopTabsAsync() { await JS.InvokeVoidAsync(JSActionHandler.PopTabs); }
    public static void EnableTabs() { JS.InvokeVoid(JSActionHandler.EnableTabs); }
    public static async Task EnableTabsAsync() { await JS.InvokeVoidAsync(JSActionHandler.EnableTabs); }
    public static void EnableTabsForDescendants(string id) { JS.InvokeVoid(JSActionHandler.EnableTabsForDescendants, id); }
    public static async Task EnableTabsForDescendantsAsync(string id) { await JS.InvokeVoidAsync(JSActionHandler.EnableTabsForDescendants, id); }
    public static void SetInert(string selector) { JS.InvokeVoid(JSActionHandler.SetInert, selector); }
    public static async Task SetInertAsync(string selector) { await JS.InvokeVoidAsync(JSActionHandler.SetInert, selector); }
    public static void RemoveInert(string selector) { JS.InvokeVoid(JSActionHandler.RemoveInert, selector); }
    public static async Task RemoveInertAsync(string selector) { await JS.InvokeVoidAsync(JSActionHandler.RemoveInert, selector); }
    public static void Click(string id) { JS.InvokeVoid(JSActionHandler.Click, id); }
    public static async Task ClickAsync(string id) { await JS.InvokeVoidAsync(JSActionHandler.Click, id); }
}
namespace Jumpeno.Client.Services;

#pragma warning disable CS8618

public class JS {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static IJSInProcessRuntime Runtime => (IJSInProcessRuntime)AppEnvironment.GetService<IJSRuntime>();

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static void InvokeVoid(string identifier, params object?[]? args) => Runtime.InvokeVoid(identifier, args);

    public static async Task InvokeVoidAsync(string identifier, params object?[]? args) => await Runtime.InvokeVoidAsync(identifier, args);

    public static T Invoke<T>(string identifier, params object?[]? args) => Runtime.Invoke<T>(identifier, args);

    public static async Task<T> InvokeAsync<T>(string identifier, params object?[]? args) => await Runtime.InvokeAsync<T>(identifier, args);
}

namespace Jumpeno.Client.Services;

#pragma warning disable CS8618

public class JS {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static IJSInProcessRuntime Runtime;

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public static void Init(IJSRuntime runtime) {
        if (AppEnvironment.IsServer) throw new InvalidOperationException("JS can not be used on server!");
        if (Runtime is not null) throw new InvalidOperationException("JSRuntime already initialized!");
        Runtime = (IJSInProcessRuntime) runtime;
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static void InvokeVoid(string identifier, params object?[]? args) => Runtime.InvokeVoid(identifier, args);

    public static async Task InvokeVoidAsync(string identifier, params object?[]? args) => await Runtime.InvokeVoidAsync(identifier, args);

    public static T Invoke<T>(string identifier, params object?[]? args) => Runtime.Invoke<T>(identifier, args);

    public static async Task<T> InvokeAsync<T>(string identifier, params object?[]? args) => await Runtime.InvokeAsync<T>(identifier, args);
}

namespace Jumpeno.Client.Services;

#pragma warning disable CS8618

public class JS {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    private const string EvalFuncName = "eval";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static IJSInProcessRuntime Runtime => (IJSInProcessRuntime)AppEnvironment.GetService<IJSRuntime>();

    // Invoke -----------------------------------------------------------------------------------------------------------------------------
    public static void InvokeVoid(string identifier, params object?[]? args) => Runtime.InvokeVoid(identifier, args);

    public static async Task InvokeVoidAsync(string identifier, params object?[]? args) => await Runtime.InvokeVoidAsync(identifier, args);

    public static T Invoke<T>(string identifier, params object?[]? args) => Runtime.Invoke<T>(identifier, args);

    public static async Task<T> InvokeAsync<T>(string identifier, params object?[]? args) => await Runtime.InvokeAsync<T>(identifier, args);

    // Eval -------------------------------------------------------------------------------------------------------------------------------
    public static void EvalVoid(string code) => Runtime.InvokeVoid(EvalFuncName, code);

    public static async Task EvalVoidAsync(string code) => await Runtime.InvokeVoidAsync(EvalFuncName, code);

    public static T Eval<T>(string code) => Runtime.Invoke<T>(EvalFuncName, code);

    public static async Task<T> EvalAsync<T>(string code) => await Runtime.InvokeAsync<T>(EvalFuncName, code);
}

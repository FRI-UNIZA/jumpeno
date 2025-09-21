namespace Jumpeno.Server.Services;

public static class ServerEnvironment {
    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public static Microsoft.AspNetCore.Mvc.Rendering.RenderMode RenderMode() {
        return AppSettings.Prerender
            ? Microsoft.AspNetCore.Mvc.Rendering.RenderMode.WebAssemblyPrerendered
            : Microsoft.AspNetCore.Mvc.Rendering.RenderMode.WebAssembly;
    }

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static bool IsStaticPath(string path) => Path.HasExtension(path);

    public static void WithoutStaticPath(string path, Action callback) {
        if (IsStaticPath(path)) return;
        callback();
    }

    public static async Task WithoutStaticPath(string path, Func<Task> callback) {
        if (IsStaticPath(path)) return;
        await callback();
    }
}

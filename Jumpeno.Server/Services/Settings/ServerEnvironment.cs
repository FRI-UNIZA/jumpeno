namespace Jumpeno.Server.Services;

public static class ServerEnvironment {
    public static readonly string[] STATIC_FOLDERS = [
        "css", "font", "icons", "images", "js", "modules"
    ];

    public static Microsoft.AspNetCore.Mvc.Rendering.RenderMode RenderMode() {
        return AppSettings.Prerender
                ? Microsoft.AspNetCore.Mvc.Rendering.RenderMode.WebAssemblyPrerendered
                : Microsoft.AspNetCore.Mvc.Rendering.RenderMode.WebAssembly;
    }

    public static bool IsStaticPath(string path) {
        foreach (var folder in STATIC_FOLDERS) {
            if (path.StartsWith($"/{folder}")) return true;
        }
        return false;
    }
    public static void WithoutStaticPath(string path, Action callback) {
        if (IsStaticPath(path)) return;
        callback();
    }
    public static async Task WithoutStaticPath(string path, Func<Task> callback) {
        if (IsStaticPath(path)) return;
        await callback();
    }
}

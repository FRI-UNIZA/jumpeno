namespace Jumpeno.Client.Utils;

public static class ClientImport {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string DATA_IMPORT_CRITICAL = "data-import-critical";
    public const string DATA_SUCCESS = "data-success";
    public const string DATA_ERROR = "data-error";
    public const int AWAIT = 100; // ms

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static async Task Await() {
        // 1) Await fonts:
        await JS.EvalVoidAsync("document.fonts.ready");
        // 2) Await imports:
        await JS.EvalVoidAsync($$"""
            (async () => {
                while (true) {
                    const errors = document.querySelectorAll("[{{DATA_IMPORT_CRITICAL}}][{{DATA_ERROR}}]");
                    if (errors.length > 0) throw new Error("Critical import failed!");
                    const imports = document.querySelectorAll("[{{DATA_IMPORT_CRITICAL}}]");
                    const success = document.querySelectorAll("[{{DATA_IMPORT_CRITICAL}}][{{DATA_SUCCESS}}]");
                    if (imports.length === success.length) return;
                    await new Promise(resolve => setTimeout(resolve, {{AWAIT}}));
                }
            })()
        """);
    }
}

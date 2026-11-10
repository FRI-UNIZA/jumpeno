namespace Jumpeno.Client.Utils;

public static class ClientImport {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string DataImportCritical = "data-import-critical";
    public const string DataSuccess = "data-success";
    public const string DataError = "data-error";
    public const int AwaitTime = 100; // ms

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static async Task Await() {
        // 1) Await fonts:
        await JS.EvalVoidAsync("document.fonts.ready");
        // 2) Await imports:
        await JS.EvalVoidAsync($$"""
            (async () => {
                while (true) {
                    const errors = document.querySelectorAll("[{{DataImportCritical}}][{{DataError}}]");
                    if (errors.length > 0) throw new Error("Critical import failed!");
                    const imports = document.querySelectorAll("[{{DataImportCritical}}]");
                    const success = document.querySelectorAll("[{{DataImportCritical}}][{{DataSuccess}}]");
                    if (imports.length === success.length) return;
                    await new Promise(resolve => setTimeout(resolve, {{AwaitTime}}));
                }
            })()
        """);
    }
}

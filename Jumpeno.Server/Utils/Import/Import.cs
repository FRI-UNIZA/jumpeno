namespace Jumpeno.Server.Utils;

public static class Import {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string FunctionLoaded = "JSImportLoaded";
    public const string FunctionError = "JSImportError";

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public static IHtmlContent Init(string indent = "        ") => new HtmlString(
        "<script>\n" + indent +
        $"    const {FunctionLoaded} = element => element.setAttribute('{ClientImport.DataSuccess}', '');\n" + indent +
        $"    const {FunctionError} = element => element.setAttribute('{ClientImport.DataError}', '');\n" + indent +
        "</script>"
    );

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static IHtmlContent Module => new HtmlString("type=\"module\"");

    public static IHtmlContent Critical => new HtmlString(
        $"{ClientImport.DataImportCritical} onload=\"{FunctionLoaded}(this)\" onerror=\"{FunctionError}(this)\""
    );
}

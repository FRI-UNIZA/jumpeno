namespace Jumpeno.Server.Utils;

public static class Import {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string FUNCTION_LOADED = "JSImportLoaded";
    public const string FUNCTION_ERROR = "JSImportError";

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public static IHtmlContent Init(string indent = "        ") => new HtmlString(
        "<script>\n" + indent +
        $"    const {FUNCTION_LOADED} = element => element.setAttribute('{ClientImport.DATA_SUCCESS}', '');\n" + indent +
        $"    const {FUNCTION_ERROR} = element => element.setAttribute('{ClientImport.DATA_ERROR}', '');\n" + indent +
        "</script>"
    );

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static IHtmlContent Module => new HtmlString("type=\"module\"");

    public static IHtmlContent Critical => new HtmlString(
        $"{ClientImport.DATA_IMPORT_CRITICAL} onload=\"{FUNCTION_LOADED}(this)\" onerror=\"{FUNCTION_ERROR}(this)\""
    );
}

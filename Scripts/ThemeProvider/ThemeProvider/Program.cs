namespace ThemeProvider;

using System.Reflection;
using Microsoft.CodeAnalysis;
using Jumpeno.Shared.Utils;

public class Program {
    // Classes ----------------------------------------------------------------------------------------------------------------------------
    private const string CLASS_DARK = "DarkTheme";
    private const string CLASS_LIGHT = "LightTheme";

    // CSS --------------------------------------------------------------------------------------------------------------------------------
    public const string CLASSNAME_NO_THEME = "no-theme";
    public const string CLASSNAME_DARK_THEME = "dark-theme";
    public const string CLASSNAME_LIGHT_THEME = "light-theme";
    public const string CLASSNAME_THEME_UPDATED = "theme-updated";
    public const string CLASSNAME_THEME_TRANSITION_CONTAINER = "theme-transition-container";

    // Paths ------------------------------------------------------------------------------------------------------------------------------
    private static readonly string ROOT = $"{Directory.GetCurrentDirectory()}/..";
    private static readonly string CLASS_DIR = $"{ROOT}/Jumpeno.Client/Themes/Constants";
    private const string CLASS_NAMESPACE = "Jumpeno.Client.Constants";
    private static readonly string CSS_PATH = $"{ROOT}/Jumpeno.Client/wwwroot/css/theme.css";

    // Dependencies -----------------------------------------------------------------------------------------------------------------------
    private static readonly string USINGS = "using Jumpeno.Shared.Models;";
    private static readonly PortableExecutableReference[] REFERENCES = [
        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
        MetadataReference.CreateFromFile(Assembly.Load("System").Location),
        MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
        MetadataReference.CreateFromFile(Assembly.Load("System.Runtime.Extensions").Location),
        MetadataReference.CreateFromFile(Assembly.Load("System.Linq").Location),
        MetadataReference.CreateFromFile(Assembly.Load("System.Text.Json").Location)
    ];
    private static (string Path, string Usings)[] DEPENDENCIES = [
        ($"{ROOT}/Jumpeno.Client/Components/ScrollArea/Constants/SCROLLAREA_THEME.cs", ""),
        ($"{ROOT}/Jumpeno.Shared/Models/Graphics/RGBColor.cs", "using System; using System.Linq; using System.Text.Json.Serialization;"),
        ($"{ROOT}/Jumpeno.Client/Themes/Constants/BaseTheme.cs", USINGS)
    ];

    // Generators -------------------------------------------------------------------------------------------------------------------------
    private static string GenerateVariables(string className, string bodyClassName) {
        // 1) Create instance:
        var type = Reflex.CompileClass(CLASS_DIR, CLASS_NAMESPACE, className, USINGS, REFERENCES, DEPENDENCIES);
        var instance = Reflex.CreateInstance<object>(type);
        // 2) Create content:
        string content = $"body.{bodyClassName} {{";
        foreach (var property in Reflex.GetMembers(instance)) {
            content = $"{content}\n    --{@property.Name.ToLower().Replace("_", "-")}: {@property.Value};";
        }
        return $"{content}\n}}\n";
    }

    public static string GenerateThemeTransition() {        
        string content = "@keyframes fade-in-theme { 0% { opacity: 0; } 100% { opacity: 1; }}\n";
        content = $"{content}.theme-updated .theme-transition-container {{\n";
        content = $"{content}    animation: fade-in-theme calc(var(--transition-extra-slow) * 1ms) forwards;\n";
        content = $"{content}}}\n";
        content = $"{content}.setting-theme .theme-transition-container {{\n";
        content = $"{content}    display: none !important;\n";
        content = $"{content}}}\n";
        return content;
    }

    // Entry point ------------------------------------------------------------------------------------------------------------------------
    public static void Main(string[] args) {
        // 1) Create values:
        var noTheme = $"body.{CLASSNAME_NO_THEME} {{ opacity: 0; }}\n";
        var themeTransition = GenerateThemeTransition();
        var darkVars = GenerateVariables(CLASS_DARK, CLASSNAME_DARK_THEME);
        var lightVars = GenerateVariables(CLASS_LIGHT, CLASSNAME_LIGHT_THEME);
        // 2) Write to file:
        File.WriteAllText(CSS_PATH, $"{noTheme}\n{themeTransition}\n{darkVars}\n{lightVars}");
    }
}

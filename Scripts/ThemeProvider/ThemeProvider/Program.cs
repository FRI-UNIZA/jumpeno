namespace ThemeProvider;

using System.Reflection;
using Microsoft.CodeAnalysis;
using Jumpeno.Client.Utils;

public class Program {
    // Classes ----------------------------------------------------------------------------------------------------------------------------
    private const string NAMESPACE = "Jumpeno.Client.Constants";
    private const string CLASSNAME_BASE = "BaseTheme";
    private static readonly List<(string CLASSNAME, string CSS_CLASS)> THEMES = [
        ("LightTheme", "light-theme"),
        ("DarkTheme", "dark-theme")
    ];

    // Surfaces ---------------------------------------------------------------------------------------------------------------------------
    private const string SURFACE_SUFFIX = "surface";
    private const string SURFACE_DIVIDER = "--";
    private const string SURFACE_PRIMARY = "surface-primary";

    // Variables --------------------------------------------------------------------------------------------------------------------------
    private const string VARIABLE_PREFIX = "theme";

    // Paths ------------------------------------------------------------------------------------------------------------------------------
    private static readonly string ROOT = $"{Directory.GetCurrentDirectory()}/..";
    private static readonly string CLASS_DIR = $"{ROOT}/Jumpeno.Client/Themes/Themes";
    private static readonly string CSS_PATH = $"{ROOT}/Jumpeno.Client/wwwroot/css/theme.css";

    // Dependencies -----------------------------------------------------------------------------------------------------------------------
    private static readonly string USINGS = "using Jumpeno.Client.Models;";
    private static readonly PortableExecutableReference[] REFERENCES = [
        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
        MetadataReference.CreateFromFile(Assembly.Load("System").Location),
        MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
        MetadataReference.CreateFromFile(Assembly.Load("System.Runtime.Extensions").Location),
        MetadataReference.CreateFromFile(Assembly.Load("System.Linq").Location),
        MetadataReference.CreateFromFile(Assembly.Load("System.Text.Json").Location)
    ];
    private static readonly (string Path, string Usings)[] DEPENDENCIES = [
        ($"{ROOT}/Jumpeno.Client/Components/ScrollArea/Constants/SCROLLAREA_THEME.cs", ""),
        ($"{ROOT}/Jumpeno.Client/Utils/Graphics/Models/RGBColor.cs", "using System; using System.Linq; using System.Text.Json.Serialization;"),
        ($"{ROOT}/Jumpeno.Client/Utils/Graphics/Models/RGBAColor.cs", "using System; using System.Text.Json.Serialization;"),
        ($"{ROOT}/Jumpeno.Client/Themes/Themes/BaseTheme.cs", USINGS)
    ];

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    // Property:
    private static string TransformName(string name) => $"--{VARIABLE_PREFIX}-{name.ToLower().Replace("_", "-")}";
    private static string TransformValue(string className, string name, object? value) {
        return value == null ? throw new ArgumentNullException($"{className}.{name}") : $"{value}";
    }
    // Surface:
    private static string? SurfaceName(string name) {
        var pos = name.LastIndexOf("--");
        if (pos <= 0) return null;
        var surfaceName = name.Substring(pos + 2);
        return surfaceName;
    }
    private static string RemoveSurface(string name, string surface) => name.Substring(0, name.Length - $"{SURFACE_DIVIDER}{surface}".Length);
    private static string AddSurface(string name, string surface) => $"{name}{SURFACE_DIVIDER}{surface}";
    // CSS:
    private static string CSSRule(string name, string value) => $"{name}: {value}";
    private static string AddRule(string rule) => $"\n    {rule};";

    // Generators -------------------------------------------------------------------------------------------------------------------------    
    private static string GenerateConstants(string className) {
        // 1) Initialization:
        var type = Reflex.CompileClass(CLASS_DIR, NAMESPACE, className, USINGS, REFERENCES, DEPENDENCIES);
        var instance = Reflex.CreateInstance<object>(type);
        
        Dictionary<string, string> constants = new();

        // 2) Divide constants:
        foreach (var property in Reflex.GetMembers(instance)) {
            string propertyName = TransformName(property.Name);
            if (property.IsVirtual) continue;
            string propertyValue = TransformValue(CLASSNAME_BASE, property.Name, property.Value);
            constants[propertyName] = propertyValue;
        }

        // 3) Add constants:
        string content = "body {";
        foreach (var c in constants) {
            content += AddRule(CSSRule(c.Key, c.Value));
        }
        content += "\n}\n";

        // 4) Return result:
        return content;
    }

    private static string GenerateVariables(string className, string cssClass) {
        // 1) Initialization:
        var type = Reflex.CompileClass(CLASS_DIR, NAMESPACE, className, USINGS, REFERENCES, DEPENDENCIES);
        var instance = Reflex.CreateInstance<object>(type);
        Dictionary<string, Dictionary<string, string>> surfaces = new();

        // 2) Divide variables:
        foreach (var property in Reflex.GetMembers(instance)) {
            string propertyName = TransformName(property.Name);
            string propertyValue = TransformValue(className, property.Name, property.Value);
            // 2.1) Constants:
            if (!property.IsVirtual) continue;
            // 2.2) Variables:
            var surfaceName = SurfaceName(propertyName);
            if (surfaceName == null) {
                // 2.2.1) Common variables:
                if (!surfaces.ContainsKey(SURFACE_PRIMARY)) surfaces[SURFACE_PRIMARY] = new();
                surfaces[SURFACE_PRIMARY][propertyName] = propertyValue;
            } else {
                // 2.2.2) Surface variables:
                if (!surfaces.ContainsKey(surfaceName)) surfaces[surfaceName] = new();
                string propertySurfaceName = RemoveSurface(propertyName, surfaceName);
                surfaces[surfaceName][propertySurfaceName] = propertyValue;
            }
        }

        // 3) Add variables:
        string content = "";
        foreach (var s in surfaces) {
            content += $"body.{cssClass}.{s.Key},\n";
            content += $"body.{cssClass} .{s.Key} {{";
            foreach (var v in s.Value) {
                content += AddRule(CSSRule(v.Key, v.Value));
            }
            content += "\n}\n";
        }

        // 4) Return result:
        return content;
    }

    // Entry point ------------------------------------------------------------------------------------------------------------------------
    public static void Main(string[] args) {
        // 1) Init constants:
        string content = GenerateConstants(THEMES[0].CLASSNAME);
        // 2) Add themes:
        foreach (var THEME in THEMES) {
            content += "\n" + GenerateVariables(THEME.CLASSNAME, THEME.CSS_CLASS);
        }
        // 3) Write to file:
        File.WriteAllText(CSS_PATH, content);
    }
}

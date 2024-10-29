using System;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Jumpeno.Shared.Utils;

public class Program {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASSNAME_NO_THEME = "no-theme";
    public const string CLASSNAME_DARK_THEME = "dark-theme";
    public const string CLASSNAME_LIGHT_THEME = "light-theme";
    public const string CLASSNAME_THEME_UPDATED = "theme-updated";
    public const string CLASSNAME_THEME_TRANSITION_CONTAINER = "theme-transition-container";

    // Properties -------------------------------------------------------------------------------------------------------------------------
    private static string ClassDir = "";
    private static string ClassNamespace = "";
    private static string ClassNameDark = "";
    private static string ClassNameLight = "";
    private static string CSSPath = "";

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private static string GenerateVariables(string className, string bodyClassName) {
        string root = $"{Directory.GetCurrentDirectory()}/..";

        var type = Reflex.CompileClass(ClassDir, ClassNamespace, className, [
            $"{root}/Jumpeno.Client/Components/ScrollArea/Constants/SCROLLAREA_THEME.cs",
            $"{root}/Jumpeno.Client/Themes/Constants/BaseTheme.cs"
        ]);
        var instance = Reflex.CreateInstance<object>(type);

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

    // Starting point ---------------------------------------------------------------------------------------------------------------------
    public static void Main(string[] args) {
        ClassDir = args[0];
        ClassNamespace = args[1];
        ClassNameDark = args[2];
        ClassNameLight = args[3];
        CSSPath = args[4];

        var noTheme = $"body.{CLASSNAME_NO_THEME} {{ opacity: 0; }}\n";
        var themeTransition = GenerateThemeTransition();
        var darkVars = GenerateVariables(ClassNameDark, CLASSNAME_DARK_THEME);
        var lightVars = GenerateVariables(ClassNameLight, CLASSNAME_LIGHT_THEME);

        File.WriteAllText(CSSPath, $"{noTheme}\n{themeTransition}\n{darkVars}\n{lightVars}");
    }
}

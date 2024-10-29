namespace Jumpeno.Shared.Utils;

using System;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

public static class Reflex {
    public static Type CompileClass(string classDir, string classNamespace, string className, string[]? dependencies = null) {
        // Compile the code
        var references = new[] {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),           // System.Private.CoreLib.dll
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),          // System.Console.dll
            MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location)    // System.Runtime.dll
        };

        SyntaxTree[] syntaxTrees = [];
        if (dependencies is not null) {
            foreach (var dep in dependencies) {
                syntaxTrees = syntaxTrees.Append(CSharpSyntaxTree.ParseText(File.ReadAllText(dep))).ToArray();
            }
        }
        string code = File.ReadAllText($"{classDir}/{className}.cs");
        syntaxTrees = syntaxTrees.Append(CSharpSyntaxTree.ParseText(code)).ToArray();

        var compilation = CSharpCompilation.Create(
            "DynamicAssembly",
            syntaxTrees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (!result.Success) {
            throw new Exception("Compilation failed!");
        }

        ms.Seek(0, SeekOrigin.Begin);
        var assembly = Assembly.Load(ms.ToArray());

        // Get the dynamically created type
        var type = assembly.GetType($"{classNamespace}.{className}") ?? throw new Exception("Could not create type!");
        return type;
    }

    [return: NotNull]
    public static T CreateInstance<T>(Type type) {
        return (T) Activator.CreateInstance(type)! ?? throw new Exception("Instance could not be created!"); 
    }

    public static IEnumerable<(string Name, object Value)> GetMembers(object instance) {
        // Ensure model is not null
        if (instance == null) yield break;

        var type = instance.GetType();

        // Get all public properties
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties) {
            var value = property.GetValue(instance) ?? "null";
            yield return (property.Name, value);
        }

        // Get all public fields
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields) {
            var value = field.GetValue(instance) ?? "null";
            yield return (field.Name, value);
        }
    }
}

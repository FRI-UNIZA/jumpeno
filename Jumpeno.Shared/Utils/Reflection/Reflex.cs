namespace Jumpeno.Shared.Utils;

using System;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

public static class Reflex {
    // Compilation ------------------------------------------------------------------------------------------------------------------------
    public static Type CompileClass(
        string classDir, string classNamespace, string className, string usings,
        PortableExecutableReference[]? references = null,
        (string Path, string Usings)[]? dependencies = null
    ) {
        // Compile the code
        LinkedList<SyntaxTree> syntaxTrees = [];
        if (dependencies is not null) {
            foreach (var dep in dependencies) {
                syntaxTrees.AddLast(CSharpSyntaxTree.ParseText($"{dep.Usings}\n{File.ReadAllText(dep.Path)}"));
            }
        }
        string code = $"{usings}\n{File.ReadAllText($"{classDir}/{className}.cs")}";
        syntaxTrees.AddLast(CSharpSyntaxTree.ParseText(code));

        var compilation = CSharpCompilation.Create(
            "DynamicAssembly",
            syntaxTrees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        foreach (var d in result.Diagnostics) Console.Error.WriteLine(d);
        if (!result.Success) throw new Exception("Compilation failed!");

        ms.Seek(0, SeekOrigin.Begin);
        var assembly = Assembly.Load(ms.ToArray());

        // Get the dynamically created type
        var type = assembly.GetType($"{classNamespace}.{className}") ?? throw new Exception("Could not create type!");
        return type;
    }

    // Instance ---------------------------------------------------------------------------------------------------------------------------
    [return: NotNull]
    public static T CreateInstance<T>(Type type) {
        return (T) Activator.CreateInstance(type)! ?? throw new Exception("Instance could not be created!"); 
    }

    // Getters ----------------------------------------------------------------------------------------------------------------------------
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

    // Setters ----------------------------------------------------------------------------------------------------------------------------
    public static void SetField<T>(Type type, object instance, string name, T value) {
        FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;
        field?.SetValue(instance, value);
    }
}

namespace Jumpeno.Shared.Utils;

public static class IDGenerator {    
    public static string Generate(string prefix) => $"{prefix}-{Guid.NewGuid()}";
}

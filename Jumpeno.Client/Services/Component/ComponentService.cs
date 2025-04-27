namespace Jumpeno.Client.Services;

public static class ComponentService {
    public static string GenerateID(string prefix) => $"{prefix}-{Guid.NewGuid()}";
}

namespace Jumpeno.Client.Services;

public static class ComponentService {
    public static string GenerateID(string prefix) {
        var generatedID = $"{prefix}-{Guid.NewGuid()}";
        return generatedID;
    }
}

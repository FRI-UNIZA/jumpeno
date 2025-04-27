namespace Jumpeno.Client.Utils;

public class ActiveInputManager {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static readonly Dictionary<string, object> ViewModels = [];

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static void Add(string id, object viewModel) {
        ViewModels[id] = viewModel;
    }
    
    public static void Remove(string id) {
        ViewModels.Remove(id);
    }

    public static object? Get(string id) {
        ViewModels.TryGetValue(id, out var viewModel);
        return viewModel;
    }
}

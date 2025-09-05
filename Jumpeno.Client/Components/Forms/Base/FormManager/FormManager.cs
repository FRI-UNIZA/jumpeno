namespace Jumpeno.Client.Utils;

public class FormManager {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static readonly Dictionary<string, FormErrorViewModel> ViewModels = AppEnvironment.IsServer ? null! : [];

    // ID ---------------------------------------------------------------------------------------------------------------------------------
    public static string CreateFormID(string form, string id) => $"{form}_{id}";

    // Errors -----------------------------------------------------------------------------------------------------------------------------
    public static void SetError(string form, Error error) {
        if (AppEnvironment.IsServer) return;
        if (form == null || form == "") return;
        FormErrorViewModel? viewModel = Get(CreateFormID(form, error.ID));
        if (viewModel == null) return;
        viewModel.Set(I18N.T(error.Info, unsplit: true));
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static void Add(string id, FormErrorViewModel viewModel) {
        if (AppEnvironment.IsServer) return;
        ViewModels[id] = viewModel;
    }
    
    public static void Remove(string id) {
        if (AppEnvironment.IsServer) return;
        ViewModels.Remove(id);
    }

    public static FormErrorViewModel? Get(string id) {
        if (AppEnvironment.IsServer) return null;
        ViewModels.TryGetValue(id, out var viewModel);
        return viewModel;
    }
}

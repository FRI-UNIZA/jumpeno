namespace Jumpeno.Client.Utils;

public class FormManager {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static readonly Dictionary<string, Dictionary<string, FormErrorViewModel>> ViewModels = AppEnvironment.IsServer ? null! : [];

    // ID ---------------------------------------------------------------------------------------------------------------------------------
    public static string CreateFormID(string form, string id) => $"{form}_{id}";

    // Errors -----------------------------------------------------------------------------------------------------------------------------
    public static void SetError(string form, Error error) {
        // 1) Check environment and values:
        if (AppEnvironment.IsServer) return;
        if (form == null || form == "") return;
        // 2) Get ViewModel:
        FormErrorViewModel? viewModel = Get(form, error.ID);
        // 3) Set error:
        if (viewModel == null) return;
        viewModel.Set(I18N.T(error.Info, unsplit: true));
    }

    public static void ClearErrors(string form) {
        // 1) Check environment and values:
        if (AppEnvironment.IsServer) return;
        if (form == null || form == "") return;
        // 2) Get list:
        var list = GetList(form);
        // 3) Clear errors:
        foreach (var vm in list) {
            vm.Value.Clear();
        }
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static void Add(string form, string id, FormErrorViewModel viewModel) {
        // 1) Check client:
        if (AppEnvironment.IsServer) return;
        // 2) Get list:
        ViewModels.TryGetValue(form, out var list);
        if (list == null) {
            list = [];
            ViewModels[form] = list;
        }
        // 3) Add ViewModel:
        list[id] = viewModel;
    }
    
    public static void Remove(string form, string id) {
        // 1) Check client:
        if (AppEnvironment.IsServer) return;
        // 2) Get list:
        ViewModels.TryGetValue(form, out var list);
        // 3) Remove ViewModel:
        if (list == null) return;
        list.Remove(id);
        // 4) Remove list:
        if (list.Count > 0) return;
        ViewModels.Remove(form);
    }

    public static Dictionary<string, FormErrorViewModel> GetList(string form) {
        // 1) Check client:
        if (AppEnvironment.IsServer) return [];
        // 2) Get list:
        ViewModels.TryGetValue(form, out var list);
        // 3) Return list:
        return list ?? [];
    }

    public static FormErrorViewModel? Get(string form, string id) {
        // 1) Check client:
        if (AppEnvironment.IsServer) return null;
        // 2) Get list:
        ViewModels.TryGetValue(form, out var list);
        // 3) Get ViewModel:
        if (list == null) return null;
        list.TryGetValue(id, out var viewModel);
        // 4) Return ViewModel:
        return viewModel;
    }
}

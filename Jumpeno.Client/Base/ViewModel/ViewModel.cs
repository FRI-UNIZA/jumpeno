namespace Jumpeno.Client.ViewModels;

public class ViewModel<T> where T : Component {
    // View -------------------------------------------------------------------------------------------------------------------------------
    protected T? View;

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static bool Connect(ViewModel<T> viewModel, T view) {
        if (viewModel.View == view) return false;
        viewModel.View = view;
        return true;
    }
}

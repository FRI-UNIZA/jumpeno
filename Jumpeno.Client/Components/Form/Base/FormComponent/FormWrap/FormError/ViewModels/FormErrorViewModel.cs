namespace Jumpeno.Client.ViewModels;

public class FormErrorViewModel {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public FormViewModel FormViewModel { get; private set; }
    public bool HasError { get; private set; } = false;
    public string Message { get; private set; } = "";

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public FormErrorViewModel(FormViewModel formViewModel) {
        FormViewModel = formViewModel;
        FormManager.Add(FormViewModel.FormID, this);
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public void Set(string message) {
        if (FormViewModel.ErrorDelegate != null) {
            FormViewModel.ErrorDelegate.Invoke(message);
        } else {
            HasError = true;
            Message = message;
            FormViewModel.Notify();
        }
    }

    public void Clear() {
        if (FormViewModel.ErrorDelegate != null) return;
        HasError = false;
        FormViewModel.Notify();
    }
}

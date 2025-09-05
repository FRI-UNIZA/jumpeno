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

    // Private actions --------------------------------------------------------------------------------------------------------------------
    private void SetError(string message) {
        HasError = true;
        Message = message;
        FormViewModel.OnError?.Invoke(Message);
        FormViewModel.Notify();
    }

    // Public actions ---------------------------------------------------------------------------------------------------------------------
    public void Set(string message) {
        if (FormViewModel.ErrorDelegate != null) FormViewModel.ErrorDelegate.Invoke(message);
        else SetError(message);
    }

    public void SetForce(string message) => SetError(message);

    public void Clear() {
        HasError = false;
        FormViewModel.Notify();
    }
}

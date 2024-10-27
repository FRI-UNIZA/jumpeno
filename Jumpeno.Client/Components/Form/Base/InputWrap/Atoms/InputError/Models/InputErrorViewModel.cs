namespace Jumpeno.Client.Models;

public class InputErrorViewModel {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private bool IsError = false;
    public string LastError { get; private set; } = "";
    private readonly Action? Notify = null;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public bool HasError() { return IsError; }
    public string Error() { return IsError ? LastError : ""; }
    public void SetError(string error) {
        LastError = error;
        IsError = true;
        Notify?.Invoke();
    }
    public void ClearError() {
        IsError = false;
        Notify?.Invoke();
    }
}

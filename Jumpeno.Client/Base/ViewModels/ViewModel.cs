namespace Jumpeno.Client.ViewModels;

public class ViewModel<T> where T : Component {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private T? View;

    // View -------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Call in OnParametersSet to connect View with ViewModel.</summary>
    /// <param name="viewModel">ViewModel to connect to</param>
    /// <param name="view">View to connect with ViewModel</param>
    /// <returns>false if ViewModel is already connected, true otherwise</returns>
    public static bool Connect(ViewModel<T> viewModel, T view) {
        if (viewModel.View == view) return false;
        viewModel.View = view;
        return true;
    }

    // ViewModel --------------------------------------------------------------------------------------------------------------------------
    /// <summary>Notify connected view.</summary>
    protected void Notify() => View?.Notify();

    /// <summary>Notify connected view with message and data.</summary>
    /// <param name="message">message string</param>
    /// <param name="data">data to send</param>
    protected void Notify(string message, object? data = null) {
        if (View == null) return;
        Reflex.InvokeVoid(View, nameof(Notify), [message, data]);
    }

    /// <summary>Notify connected view with message and data asynchronously.</summary>
    /// <param name="message">message string</param>
    /// <param name="data">data to send</param>
    protected async Task NotifyAsync(string message, object? data = null) {
        if (View == null) return;
        await Reflex.InvokeVoidAsync(View, nameof(NotifyAsync), [message, data]);
    }
}

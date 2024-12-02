namespace Jumpeno.Client.Components;

public partial class MainErrorBoundary : IAsyncDisposable {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private ErrorBoundary? ErrorBoundary { get; set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override async Task OnInitializedAsync() {
        await Navigator.AddAfterEventListener(RecoveryListener);
    }
    
    public async ValueTask DisposeAsync() {
        await Navigator.RemoveAfterEventListener(RecoveryListener);
    }

    // Listeners --------------------------------------------------------------------------------------------------------------------------
    private void RecoveryListener(NavigationEvent e) {
        ErrorBoundary?.Recover();
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private static async Task Refresh() {
        await Navigator.NavigateTo(URL.Url(), replace: true, forceLoad: true);
    }
}

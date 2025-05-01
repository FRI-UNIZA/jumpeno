namespace Jumpeno.Client.Components;

public partial class MainErrorBoundary {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private ErrorBoundary? ErrorBoundary { get; set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override async Task OnComponentInitializedAsync() {
        await Navigator.AddAfterFinishEventListener(RecoveryListener);
    }

    protected override async ValueTask OnComponentDisposeAsync() {
        await Navigator.RemoveAfterFinishEventListener(RecoveryListener);
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

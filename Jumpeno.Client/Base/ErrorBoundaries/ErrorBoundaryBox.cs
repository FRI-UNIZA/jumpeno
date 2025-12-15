namespace Jumpeno.Client.Base;

public class ErrorBoundaryBox : ErrorBoundary {
    protected override Task OnErrorAsync(Exception exception) {
        if (AppEnvironment.IsDevelopment) {
            return base.OnErrorAsync(exception);
        } else {
            return Task.CompletedTask;
        }
    }
}

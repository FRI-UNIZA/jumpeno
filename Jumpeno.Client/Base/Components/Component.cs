namespace Jumpeno.Client.Base;

public class Component : ComponentBase {
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    private bool ParametersSet = false;
    protected sealed override void OnParametersSet() {
        OnParametersSet(!ParametersSet);
        ParametersSet = true;
    }

    private bool ParametersSetAsync = false;
    private readonly LockerSlim ParametersSetLock = new();
    protected sealed override async Task OnParametersSetAsync() {
        await ParametersSetLock.Exclusive(async () => {
            await OnParametersSetAsync(!ParametersSetAsync);
            ParametersSetAsync = true;
        });
    }

    // Lifecycle overrides ----------------------------------------------------------------------------------------------------------------
    protected virtual void OnParametersSet(bool firstTime) {}
    protected virtual Task OnParametersSetAsync(bool firstTime) => Task.CompletedTask;
}

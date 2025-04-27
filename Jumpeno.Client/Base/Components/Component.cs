namespace Jumpeno.Client.Base;

public class Component : ComponentBase {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public string Class { get; set; } = "";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    protected virtual CSSClass ComputeClass(string className = "") {
        var c = new CSSClass(className);
        c.Set(Class);
        return c;
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    private bool ParametersSet = false;
    protected sealed override void OnParametersSet() {
        OnParametersSet(!ParametersSet);
        ParametersSet = true;
    }

    private bool ParametersSetAsync = false;
    protected sealed override async Task OnParametersSetAsync() {
        var firstTime = !ParametersSetAsync;
        ParametersSetAsync = true;
        await OnParametersSetAsync(firstTime);
    }

    // Lifecycle overrides ----------------------------------------------------------------------------------------------------------------
    protected virtual void OnParametersSet(bool firstTime) {}
    protected virtual Task OnParametersSetAsync(bool firstTime) => Task.CompletedTask;
}

namespace Jumpeno.Client.Base;

public abstract class AuthComponent : Component {
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override sealed void OnInitialized() => base.OnInitialized();
    protected override sealed async Task OnInitializedAsync() {
        await Auth.Register(this);
        await base.OnInitializedAsync();
    }
    protected override sealed void OnParametersSet() => base.OnParametersSet();
    protected override sealed async Task OnParametersSetAsync() => await base.OnParametersSetAsync();
    protected override sealed bool ShouldRender() {
        if (Auth.Freezed(this)) return false;
        return base.ShouldRender();
    }
    protected override sealed void OnAfterRender(bool firstRender) => base.OnAfterRender(firstRender);
    protected override sealed async Task OnAfterRenderAsync(bool firstRender) => await base.OnAfterRenderAsync(firstRender);
    public override sealed async ValueTask DisposeAsync() {
        await Auth.Unregister(this);
        await base.DisposeAsync();
    }
}

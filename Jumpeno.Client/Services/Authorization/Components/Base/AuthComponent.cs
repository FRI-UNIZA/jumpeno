namespace Jumpeno.Client.Base;

public abstract class AuthComponent : Component {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    // Lifecycle:
    private readonly LockerSlim LifeLock = new();

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override sealed void OnInitialized() => base.OnInitialized();
    protected override sealed Task OnInitializedAsync() => LifeLock.TryExclusive(
        async () => {
            if (IsDisposing) return;
            await Auth.Register(this);
            await base.OnInitializedAsync();
        }
    );

    protected override sealed void OnParametersSet() => base.OnParametersSet();
    protected override sealed async Task OnParametersSetAsync() => await base.OnParametersSetAsync();

    protected override sealed bool ShouldRender() {
        if (Auth.Freezed(this)) return false;
        return base.ShouldRender();
    }
    protected override sealed void OnAfterRender(bool firstRender) => base.OnAfterRender(firstRender);
    protected override sealed async Task OnAfterRenderAsync(bool firstRender) => await base.OnAfterRenderAsync(firstRender);

    public override sealed async ValueTask DisposeAsync() => await LifeLock.TryExclusive(
        async () => {
            await Auth.Unregister(this);
            await base.DisposeAsync();
            LifeLock.DisposeUnsafe();
            GC.SuppressFinalize(this);
        }
    );
}

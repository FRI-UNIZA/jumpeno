namespace Jumpeno.Client.Components;

public class FormComponent<T> : Component where T : FormViewModel {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    // ViewModel:
    [Parameter]
    public required T ViewModel { get; set; }
    // Label:
    [Parameter]
    public required OneOf<string, List<string>> Label { get; set; }
    [Parameter]
    public bool HideLabel { get; set; } = false;
    // Style:
    [Parameter]
    public INPUT_SIZE Size { get; set; } = INPUT_SIZE.M;
    [Parameter]
    public INPUT_ALIGN Align { get; set; } = INPUT_ALIGN.LEFT;

    // CSS --------------------------------------------------------------------------------------------------------------------------------
    protected override CSSClass ComputeClass(string className = "") {
        var c = base.ComputeClass(className);
        c.Set(Size.String());
        c.Set(Align.String());
        if (ViewModel.Error.HasError) c.Set(FormError.CLASS_ERROR);
        if (ViewModel.Disabled) c.Set(FormError.CLASS_DISABLED);
        return c;
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override sealed void OnInitialized() => base.OnInitialized();
    protected override sealed async Task OnInitializedAsync() => await base.OnInitializedAsync();
    protected override sealed void OnParametersSet() {
        FormViewModel.SetNotify(ViewModel, StateHasChanged);
        base.OnParametersSet();
    }
    protected override sealed async Task OnParametersSetAsync() => await base.OnParametersSetAsync();
    protected override sealed bool ShouldRender() => base.ShouldRender();
    protected override sealed void OnAfterRender(bool firstRender) => base.OnAfterRender(firstRender);
    protected override sealed async Task OnAfterRenderAsync(bool firstRender) => await base.OnAfterRenderAsync(firstRender);
    public override sealed async ValueTask DisposeAsync() => await base.DisposeAsync();
}

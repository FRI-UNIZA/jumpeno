namespace Jumpeno.Client.Components;

public abstract partial class FormField<T> : IDisabledComponent {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "field";
    public const string CLASS_CONTENT = "field-content";
    public const string CLASS_AUTO_SIZE = "auto-size";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    // ViewModel:
    [Parameter]
    public required T ViewModel { get; set; }
    // Label:
    [Parameter]
    public virtual required OneOf<string, List<string>> Label { get; set; }
    // Style:
    [Parameter]
    public virtual FORM_VARIANT? Variant { get; set; } = FORM_VARIANT.PRIMARY;
    [Parameter]
    public virtual FORM_SIZE? Size { get; set; } = FORM_SIZE.M;
    [Parameter]
    public virtual FORM_ALIGN? Align { get; set; }
    [Parameter]
    public virtual FORM_ALIGN? ErrorAlign { get; set; }
    [Parameter]
    public virtual bool AutoSize { get; set; } = false;
    // Error display:
    [Parameter]
    public virtual bool NoError { get; set; } = false;
    [Parameter]
    public virtual bool NoMessage { get; set; } = false;
    // Disabled:
    [Parameter]
    public virtual bool Disabled { get; set; } = false;
    // Event propagation:
    [Parameter]
    public virtual bool Propagate { get; set; } = false;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(CLASS, Base)
        .SetVariant(Variant)
        .SetSize(Size)
        .Set(Align)
        .Set(CLASS_AUTO_SIZE, AutoSize)
        .Set(FormError.CLASS_ERROR, ViewModel.Error.HasError)
        .Set(Disabler.CLASS, Disabled)
        .Set(FormErrorType);
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

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    protected virtual FORM_ERROR_TYPE? FormErrorType => null;
    protected virtual RenderFragment? RenderField() => null;
}

namespace Jumpeno.Client.Components;

public class RadioOption<T> : FormField<RadioOptionViewModel<T>> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS_SELECTED = "selected";

    // Cascading parameters ---------------------------------------------------------------------------------------------------------------
    [CascadingParameter(Name = RadioComponent<T>.CASCADE_REF)]
    public required RadioComponent<T> Radio { get; set; }

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public override FORM_VARIANT? Variant { get; set; } = null;
    [Parameter]
    public override FORM_SIZE? Size { get; set; } = null;
    [Parameter]
    public override FORM_ALIGN? Align { get; set; } = null;
    [Parameter]
    public override FORM_ALIGN? ErrorAlign { get; set; } = null;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    protected bool IsSelected => Radio.ViewModel.Value?.DTO == ViewModel.DTO;
    protected virtual bool CustomAfterChange => false;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(Disabler.CLASS, Radio.Disabled || Disabled)
        .Set(CLASS_SELECTED, IsSelected);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override sealed void OnComponentInitialized() => OnRadioOptionInitialized();
    protected override sealed async Task OnComponentInitializedAsync() => await OnRadioOptionInitializedAsync();
    protected override sealed void OnComponentParametersSet(bool firstTime) {
        Variant ??= Radio.Variant;
        Size ??= Radio.Size;
        Align ??= Radio.Align;
        ErrorAlign ??= Radio.ErrorAlign;
        FormViewModel.SetNotify(ViewModel, Notify);
        FormViewModel.SetReact(ViewModel, () => {
            if (Radio.ViewModel.Value == ViewModel) Notify();
            else ViewModel.Error.Clear();
        });
        OnRadioOptionParametersSet(firstTime);
    }
    protected override sealed async Task OnComponentParametersSetAsync(bool firstTime) => await OnRadioOptionParametersSetAsync(firstTime);
    protected override sealed bool ShouldComponentRender() => ShouldRadioOptionRender();
    protected override sealed void OnComponentAfterRender(bool firstRender) => OnRadioOptionAfterRender(firstRender);
    protected override sealed async Task OnComponentAfterRenderAsync(bool firstRender) => await OnRadioOptionAfterRenderAsync(firstRender);
    protected override sealed void OnComponentDispose() => OnRadioOptionDispose();
    protected override sealed async ValueTask OnComponentDisposeAsync() => await OnRadioOptionDisposeAsync();

    // Lifecycle overrides ----------------------------------------------------------------------------------------------------------------
    protected virtual void OnRadioOptionInitialized() {}
    protected virtual Task OnRadioOptionInitializedAsync() => Task.CompletedTask;
    protected virtual void OnRadioOptionParametersSet(bool firstTime) {}
    protected virtual Task OnRadioOptionParametersSetAsync(bool firstTime) => Task.CompletedTask;
    protected virtual bool ShouldRadioOptionRender() => true;
    protected virtual void OnRadioOptionAfterRender(bool firstRender) {}
    protected virtual Task OnRadioOptionAfterRenderAsync(bool firstRender) => Task.CompletedTask;
    protected virtual void OnRadioOptionDispose() {}
    protected virtual ValueTask OnRadioOptionDisposeAsync() => ValueTask.CompletedTask;
    
    // Actions ----------------------------------------------------------------------------------------------------------------------------
    protected async Task Set() {
        if (Radio.Disabled == true || Disabled == true) return;
        // 1) Value:
        var before = Radio.ViewModel.Value;
        var after = ViewModel;
        // 2) Focus:
        ActionHandler.SetFocus(ViewModel.FormID);
        // 3) Change value:
        if (!Radio.ViewModel.SetValue(ViewModel)) return;
        // 4) Set events:
        if (!CustomAfterChange) {
            AnimationHandler.SetOnTransitionEndEvent(Selector.ID(ViewModel.FormID), Radio.ViewModel.OnAfterChange, new(before, after));
        }
        // 5) Call events:
        await Radio.ViewModel.OnChange.Invoke(new(before, Radio.ViewModel.Value));
    }
}

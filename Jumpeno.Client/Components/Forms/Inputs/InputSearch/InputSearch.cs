namespace Jumpeno.Client.Components;

public partial class InputSearch {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string CLASS = "input-search";
    // Default:
    public const string DEFAULT_NAME = "search";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public string Name { get; set; } = DEFAULT_NAME;
    [Parameter]
    public RenderFragment? Icon { get; set; } = null;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentParametersSet(bool firstTime) {
        FormViewModel.SetNotify(ViewModel, StateHasChanged);
    }
}

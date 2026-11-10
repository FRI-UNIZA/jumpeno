namespace Jumpeno.Client.Components;

public partial class InputSearch {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string ClassName = "input-search";
    // Default:
    public const string DefaultName = "search";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public string Name { get; set; } = DefaultName;
    [Parameter]
    public RenderFragment? Icon { get; set; } = null;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentParametersSet(bool firstTime) {
        FormViewModel.SetNotify(ViewModel, StateHasChanged);
    }
}

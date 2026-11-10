namespace Jumpeno.Client.Components;

public partial class CheckBoxComponent {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string ClassName = "checkbox";
    public const string ClassElement = "checkbox-element";
    public const string ClassIcon = "checkbox-icon";
    public const string ClassDescription = "checkbox-description";
    public const string ClassActiveDescription = "active-description";
    public const string ClassChecked = "checked";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public RenderFragment? Description { get; set; } = null;
    [Parameter]
    public bool ActiveDescription { get; set; } = false;
    [Parameter]
    public CheckBoxPosition? Position { get; set; } = CheckBoxPosition.Start;
    [Parameter]
    public RenderFragment? Icon { get; set; } = null;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() {
        return base.ComputeClass()
        .Set(ClassName, Base)
        .Set(ClassActiveDescription, ActiveDescription)
        .Set(Position)
        .Set(ClassChecked, ViewModel.Value);
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private async Task ChangeValue() {
        if (Disabled) return;
        // 1) Value:
        var value = !ViewModel.Value;
        // 2) Focus:
        ActionHandler.SetFocus(ViewModel.FormID);
        // 3) Change value:
        ViewModel.SetValue(value);
        // 4) Set events:
        AnimationHandler.SetOnTransitionEndEvent(Selector.ID(ViewModel.FormID), ViewModel.OnAfterChange, new(value));
        // 5) Call events:
        await ViewModel.OnChange.Invoke(new(value));
    }
}

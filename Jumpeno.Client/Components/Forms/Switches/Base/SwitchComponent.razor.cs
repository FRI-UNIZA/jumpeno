namespace Jumpeno.Client.Components;

public partial class SwitchComponent {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string CLASS = "switch";
    public const string CLASS_SWITCH_ELEMENT = "switch-element";
    public const string CLASS_SWITCH_DESCRIPTION = "switch-description";
    public const string CLASS_ACTIVE_DESCRIPTION = "active-description";
    public const string CLASS_CHECKED = "checked";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public RenderFragment? Description { get; set; } = null;
    [Parameter]
    public bool ActiveDescription { get; set; } = false;
    [Parameter]
    public SWITCH_POSITION? Position { get; set; } = SWITCH_POSITION.START;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(CLASS, Base)
        .Set(CLASS_ACTIVE_DESCRIPTION, ActiveDescription)
        .Set(Position)
        .Set(CLASS_CHECKED, ViewModel.Value);
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

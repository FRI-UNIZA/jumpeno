namespace Jumpeno.Client.ViewModels;

public class SwitchViewModel(SwitchViewModelParams p) : FormViewModel(p.Form, p.ID, p.OnError) {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public bool Value { get; private set; } = p.DefaultValue;
    public void SetValue(bool value) { if (value != Value) { Value = value; Error.Clear(); } }

    // Events -----------------------------------------------------------------------------------------------------------------------------
    public EventDelegate<SwitchEvent> OnChange { get; set; } = p.OnChange ?? EventDelegate<SwitchEvent>.EMPTY;
    public EventDelegate<SwitchEvent> OnAfterChange { get; set; } = p.OnAfterChange ?? EventDelegate<SwitchEvent>.EMPTY;
}

namespace Jumpeno.Client.ViewModels;

public class CheckBoxViewModel(CheckBoxViewModelParams p) : FormViewModel(p.Form, p.ID, p.OnError) {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public bool Value { get; private set; } = p.DefaultValue;
    public void SetValue(bool value) { if (value != Value) { Value = value; Error.Clear(); } }

    // Events -----------------------------------------------------------------------------------------------------------------------------
    public EventDelegate<CheckBoxEvent> OnChange { get; set; } = p.OnChange ?? EventDelegate<CheckBoxEvent>.EMPTY;
    public EventDelegate<CheckBoxEvent> OnAfterChange { get; set; } = p.OnAfterChange ?? EventDelegate<CheckBoxEvent>.EMPTY;
}

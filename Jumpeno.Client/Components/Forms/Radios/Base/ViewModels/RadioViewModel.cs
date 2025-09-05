namespace Jumpeno.Client.ViewModels;

public class RadioViewModel<T> : FormViewModel {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public RadioOptionViewModel<T>? Value { get; private set; }
    public bool SetValue(RadioOptionViewModel<T>? value) {
        if (value == Value) return false;
        var previous = Value;
        Value = value;
        Error.Clear();
        previous?.React();
        Value?.React();
        return true;
    }

    // Events -----------------------------------------------------------------------------------------------------------------------------
    public EventDelegate<RadioEvent<T>> OnChange { get; set; }
    public EventDelegate<RadioEvent<T>> OnAfterChange { get; set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public RadioViewModel(RadioViewModelParams<T> p) : base(p.Form, p.ID, p.OnError) {
        Value = p.DefaultValue;
        ErrorDelegate = message => {
            if (Value == null) Error.SetForce(message);
            else Value?.Error?.Set(message);
        };
        OnChange = p.OnChange ?? EventDelegate<RadioEvent<T>>.EMPTY;
        OnAfterChange = p.OnAfterChange ?? EventDelegate<RadioEvent<T>>.EMPTY;
    }
}

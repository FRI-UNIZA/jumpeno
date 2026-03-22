namespace Jumpeno.Client.ViewModels;

public class TextAreaViewModel : FormViewModel {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public readonly INPUT_TEXT_MODE TextMode;
    public readonly bool Trim;
    public readonly Predicate<string>? TextCheck;
    public readonly int? MaxLength;
    public readonly int? Rows;
    public readonly bool AutoResize;
    // Value:
    public readonly string? Placeholder;
    public readonly string DefaultValue;
    public readonly string ClearValue;
    public string Value { get; private set; }

    public void SetValue(string value) {
        string previous = Value;
        Value = ConstrainedValue(value);
        if (!Value.Equals(previous)) Error.Clear();
        React();
    }
    public void Clear() => SetValue(ClearValue);

    // Events -----------------------------------------------------------------------------------------------------------------------------
    public EventDelegate<InputEvent<string>> OnInput { get; set; }
    public EventDelegate<InputEvent<string>> OnClear { get; set; }
    public EventDelegate<InputEvent<string>> OnChange { get; set; }
    public EventDelegate<InputEvent<string>> OnEnter { get; set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public TextAreaViewModel(TextAreaViewModelParams p) : base(p.Form, p.ID, p.OnError) {
        TextMode = p.TextMode;
        Trim = p.Trim;
        TextCheck = p.TextCheck;
        if (p.MaxLength is not null) {
            Checker.CheckGreaterOrEqualTo((int) p.MaxLength, 0);
            MaxLength = p.MaxLength;
        }
        Rows = p.Rows;
        AutoResize = p.AutoResize;
        Placeholder = p.Placeholder;
        DefaultValue = ConstrainedValue(p.DefaultValue);
        ClearValue = ConstrainedValue(p.ClearValue);
        Value = DefaultValue;
        OnInput = p.OnInput ?? new(e => {});
        OnClear = p.OnClear ?? new(e => {});
        OnChange = p.OnChange ?? new(e => {});
        OnEnter = p.OnEnter ?? new(e => {});
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public string ApplyTextMode(string value) => TextMode switch {
        INPUT_TEXT_MODE.LOWERCASE => value.ToLower(),
        INPUT_TEXT_MODE.UPPERCASE => value.ToUpper(),
        _ => value
    };

    public string ApplyTrim(string value) => Trim ? value.Trim() : value;

    private string ConstrainedValue(string value) {
        try {
            if (MaxLength is not null && value.Length > MaxLength)
                value = value.Substring(0, (int) MaxLength);
        } catch {
            value = ClearValue;
        }
        return ApplyTrim(ApplyTextMode(value));
    }
}

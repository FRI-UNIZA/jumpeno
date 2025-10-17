namespace Jumpeno.Client.Components;

using System.Numerics;

public partial class InputComponent<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string CLASS = "input";
    public const string CLASS_CONTAINER = "input-container";
    public const string CLASS_INPUT_ELEMENT = "input-element";
    // Delimiter:
    public const NUMBER_DELIMITER DEFAULT_DELIMITER = NUMBER_DELIMITER.COMMA;

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public string Name { get; set; } = "";
    [Parameter]
    public bool AllowClear { get; set; } = false;
    [Parameter]
    public bool Autocomplete { get; set; } = false;
    [Parameter]
    public NUMBER_DELIMITER Delimiter { get; set; } = DEFAULT_DELIMITER;
    [Parameter]
    public RenderFragment? Icon { get; set; } = null;
    [Parameter]
    public RenderFragment? IconAfter { get; set; } = null;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private InputViewModel<T>? PreviousViewModel = null;
    // Value:
    private string DELIMITER = DEFAULT_DELIMITER.String();
    private string InputValue = "";
    private string FinalValue = "";
    // Lock:
    private readonly LockerSlim InputLock = new();

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    protected string ComputeType() {
        if (ViewModel.Secret) return "password";
        return "text";
    }

    protected string? ComputeInputMode() {
        switch (ViewModel.Type) {
            case INPUT_TYPE.LONG:
            case INPUT_TYPE.DOUBLE:
                return "text";
        }
        return null;
    }

    protected string ComputeAutocomplete() {
        switch (ViewModel.Type) {
            case INPUT_TYPE.LONG:
            case INPUT_TYPE.DOUBLE:
                return "off";
        }
        return Autocomplete ? "on" : "off";
    }

    protected int ComputeTabindexClear() => ValueIsClear() ? -1 : 0;

    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(CLASS, Base)
        .Set($"text-mode-{ViewModel.TextMode.StringLower()}")
        .Set("allow-clear", AllowClear)
        .Set("value-default", ValueIsClear())
        .Set("icon-before", Icon is not null)
        .Set("icon-after", IconAfter is not null);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentParametersSet(bool firstTime) {
        DELIMITER = Delimiter.String();
        if (ViewModel == PreviousViewModel) return;
        PreviousViewModel = ViewModel;
        FormViewModel.SetReact(ViewModel, () => {
            UpdateInputValue();
            StateHasChanged();
        });
        UpdateInputValue();
    }

    protected override void OnComponentDispose() => InputLock.Dispose();
    
    // Events -----------------------------------------------------------------------------------------------------------------------------
    private async Task InvokeUpdate(Action? setter = null) {
        var previousText = FinalValue;
        var previousValue = ViewModel.Value;
        if (setter != null) {
            setter();
        } else {
            ViewModel.Clear();
            ActionHandler.SetFocus(ViewModel.FormID);
            await ViewModel.OnClear.Invoke(new(previousText, FinalValue, previousValue, ViewModel.Value));
        }
        if (!ViewModel.Value!.Equals(previousValue)) {
            await ViewModel.OnChange.Invoke(new(previousText, FinalValue, previousValue, ViewModel.Value));
        }
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private bool ValueIsClear() => FinalValue.Equals($"{ViewModel.ClearValue}");
    
    private void UpdateInputValue() { InputValue = FormatValue(ViewModel.Value); FinalValue = InputValue; }

    private string FormatValue(T value) {
        var val = $"{value}";
        if (ViewModel.Type == INPUT_TYPE.DOUBLE) {
            val = val.Replace(".", DELIMITER);
            val = val.Replace(",", DELIMITER);

            var index = val.IndexOf(DELIMITER);
            if (index < 0) val = $"{val}{DELIMITER}0";
            if (index == val.Length - 1) val = $"{val}0";
            index = val.IndexOf(DELIMITER);
            var length = val.Length;
            for (int i = index + 1; i < index + 1 + ViewModel.Decimals; i++) {
                if (i >= length) val = $"{val}{0}";
            }
        }
        return val;
    }

    private async Task HandleInput(string? value) {
        await InputLock.TryExclusive(async () => {
            var lastValue = FinalValue;
            var position = ActionHandler.InputCursorPosition(ViewModel.FormID);
            try {
                if (value is null) {
                    throw new InvalidDataException();
                }
                if (ViewModel.MaxLength is not null) {
                    if (ViewModel.Type == INPUT_TYPE.DOUBLE) {
                        var index = value.IndexOf(DELIMITER);
                        if (index >= 0) {
                            var val = value.Substring(0, index);
                            if (val.Length > ViewModel.MaxLength) {
                                throw new InvalidDataException();
                            }
                        } else if (value.Length > ViewModel.MaxLength) {
                            throw new InvalidDataException();
                        }
                    } else if (value.Length > ViewModel.MaxLength) {
                        throw new InvalidDataException();
                    }
                }

                if (ViewModel.Type == INPUT_TYPE.TEXT) {
                    if (ViewModel.TextCheck != null && !ViewModel.TextCheck(value)) {
                        throw new InvalidDataException();
                    }
                    InputValue = value;
                    FinalValue = (string)(object) ViewModel.ApplyTextMode((T)(object)value)!;
                } else if (ViewModel.Type == INPUT_TYPE.LONG || ViewModel.Type == INPUT_TYPE.DOUBLE) {
                    var isDecimal = ViewModel.Type == INPUT_TYPE.DOUBLE;

                    if (value.StartsWith("00") || value.StartsWith("-00")) {
                        throw new InvalidDataException();
                    }

                    bool isOnlyPositive = isDecimal ? (double)(object) ViewModel.MinValue! >= 0 : (long)(object) ViewModel.MinValue! >= 0;
                    if (isOnlyPositive && value.StartsWith("-")) {
                        throw new InvalidDataException();
                    }

                    var delimiterIndex = value.IndexOf(DELIMITER);
                    if (isDecimal && value.IndexOf(DELIMITER) != value.LastIndexOf(DELIMITER)) {
                        throw new InvalidDataException();
                    }

                    if (isDecimal && delimiterIndex >= 0) {
                        if (value.Length - (delimiterIndex + 1) > ViewModel.Decimals) {
                            throw new InvalidDataException();
                        }
                    }

                    bool isPositive = true;
                    var numVal = value;
                    if (value == "" || value == "-" || (isDecimal && value == DELIMITER)) {
                        InputValue = value;
                        FinalValue = InputValue;
                        throw new InvalidDataException();
                    } else if (value.StartsWith("-")) {
                        isPositive = false;
                        numVal = value.Substring(1);
                    }

                    var min = (int) '0';
                    var max = (int) '9';
                    for (int i = 0; i < numVal.Length; i++) {
                        if (isDecimal && numVal[i] == DELIMITER[0]) continue;
                        var val = (int) numVal[i];
                        if (val < min || val > max) {
                            throw new InvalidDataException();
                        }
                    }
                    InputValue = isPositive ? $"{numVal}" : $"-{numVal}";
                    FinalValue = InputValue;
                }
            } catch {
                return;
            }
            ViewModel.Error.Clear();
            await ViewModel.OnInput.Invoke(new(
                lastValue, FinalValue,
                ViewModel.Value, ViewModel.Value
            ));
        });
    }

    private async Task HandleChange() {
        var val = FinalValue;
        var changedValue = ViewModel.Value;
        try {
            if (ViewModel.Type == INPUT_TYPE.TEXT) {
                changedValue = (T)(object) val;
            } else if (ViewModel.Type == INPUT_TYPE.LONG || ViewModel.Type == INPUT_TYPE.DOUBLE) {

                var isDecimal = ViewModel.Type == INPUT_TYPE.DOUBLE;

                if (val == "" || val == "-" || (isDecimal && (val == DELIMITER || val == $"-{DELIMITER}"))) {
                    val = isDecimal ? Precision.ToStringDouble((double)(object) ViewModel.ClearValue!) : $"{ViewModel.ClearValue}";
                }
                var isPositive = true;
                if (val.StartsWith('-')) {
                    isPositive = false;
                    val = val.Substring(1);
                }

                if (val.StartsWith('0')) {
                    var i = 0;
                    while (i < val.Length) {
                        if (val[i] != '0') break;
                        i++;
                    }
                    if (i == val.Length) val = "0";
                    else val = val.Substring(i);
                }
                if (!isPositive && val != "0") val = $"-{val}";
                if (isDecimal) {
                    if (val[0] == DELIMITER[0]) val = $"0{val}";
                    if (val[val.Length - 1] == DELIMITER[0]) val = $"{val}0";
                    if (val.StartsWith($"-{DELIMITER}")) val = $"-0{val.Substring(1)}";
                }

                InputValue = val;
                FinalValue = InputValue;
                if (isDecimal) {
                    val = val.Replace(DELIMITER[0], '.');

                    var value = double.Parse(val, CultureInfo.InvariantCulture);
                    var minValue = (double)(object) ViewModel.MinValue!;
                    var maxValue = (double)(object) ViewModel.MaxValue!;

                    if (value < minValue) changedValue = (T)(object) minValue;
                    else if (maxValue < value) changedValue = (T)(object) maxValue;
                    else changedValue = (T)(object) value;

                    if ((double)(object) changedValue! == -0D) {
                        InputValue = $"0{DELIMITER}0";
                        FinalValue = InputValue;
                        changedValue = (T)(object) 0D;
                    }
                } else {
                    var safeValue = BigInteger.Parse(val);
                    var minValue = (long)(object) ViewModel.MinValue!;
                    var maxValue = (long)(object) ViewModel.MaxValue!;

                    if (safeValue < minValue) changedValue = (T)(object) minValue;
                    else if (maxValue < safeValue) changedValue = (T)(object) maxValue;
                    else changedValue = (T)(object)(long) safeValue;
                }
            }
        } catch { changedValue = ViewModel.ClearValue; }
        await InvokeUpdate(() => ViewModel.SetValue(changedValue));
    }

    private async Task HandleKeyDown(KeyboardEventArgs e) {
        if (e.Key == KEYBOARD.ENTER) await ViewModel.OnEnter.Invoke(new(
            FinalValue, FinalValue,
            ViewModel.Value, ViewModel.Value
        ));
    }
}

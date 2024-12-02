namespace Jumpeno.Client.Components;

using System.Numerics;
using System.Reflection;

public partial class InputBase<T> : IDisposable {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS_INPUT_BASE = "input-base";
    public const string CLASS_INPUT_BASE_COMPONENT = "input-base-component";
    public const string CLASS_INPUT_BASE_INPUT = "input-base-input";
    public const NUMBER_DELIMITER DEFAULT_DELIMITER = NUMBER_DELIMITER.COMMA;

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required InputViewModel<T> ViewModel { get; set; }
    [Parameter]
    public bool HideLabel { get; set; } = false;
    [Parameter]
    public INPUT_ALIGN Align { get; set; } = INPUT_ALIGN.LEFT;
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
    [Parameter]
    public EventDelegate<string> OnInput { get; set; } = EventDelegate<string>.EMPTY;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private string DELIMITER = DEFAULT_DELIMITER.StringValue();
    private string TempValue = "";
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

    protected CSSClass ComputeClassBase() {
        var c = ComputeClass(CLASS_INPUT_BASE);
        c.Set($"align-{Align.ToString().ToLower()}");
        c.Set($"text-mode-{ViewModel.TextMode.ToString().ToLower()}");
        return c;
    }

    protected CSSClass ComputeClassComponent() {
        var c = new CSSClass(CLASS_INPUT_BASE_COMPONENT);
        if (AllowClear) c.Set("allow-clear");
        if (ValueIsDefault()) c.Set("value-default");
        if (Icon is not null) c.Set("icon-before");
        if (IconAfter is not null) c.Set("icon-after");
        return c;
    }

    protected int ComputeTabindexClear() {
        return ValueIsDefault() ? -1 : 0;
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnParametersSet(bool firstTime) {
        ActiveInputManager.Add(ViewModel.ID, ViewModel);
        DELIMITER = Delimiter.StringValue();
        ViewModel.GetType().GetField("Notify", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(ViewModel, () => {
            UpdateTempValue();
            StateHasChanged();
        });
        UpdateTempValue();
    }

    public void Dispose() {
        ActiveInputManager.Remove(ViewModel.ID);
    }

    // Methods static ---------------------------------------------------------------------------------------------------------------------
    public static InputViewModel<T>? ActiveViewModel(string id) {
        return (InputViewModel<T>?) ActiveInputManager.Get(id);
    }

    public static void TrySetError(Error error, bool translated = true) {
        InputErrorViewModel? errorVM = InputViewModel<object>.ErrorViewModel(ActiveInputManager.Get(error.ID));
        if (errorVM is null || errorVM.HasError()) return;
        errorVM.SetError(translated ? error.Message : I18N.T(error.Message));
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private bool ValueIsDefault() {
        return TempValue.Equals($"{ViewModel.DefaultValue}");
    }
    
    private void UpdateTempValue() {
        TempValue = FormatValue(ViewModel.Value);
    }

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
        try {
            if (value is null) return;
            if (ViewModel.MaxLength is not null) {
                if (ViewModel.Type == INPUT_TYPE.DOUBLE) {
                    var index = value.IndexOf(DELIMITER);
                    if (index >= 0) {
                        var val = value.Substring(0, index);
                        if (val.Length > ViewModel.MaxLength) return;
                    } else if (value.Length > ViewModel.MaxLength) {
                        return;
                    }
                } else if (value.Length > ViewModel.MaxLength) return;
            }

            if (ViewModel.Type == INPUT_TYPE.TEXT) {
                if (ViewModel.TextCheck != null && !ViewModel.TextCheck(value)) return;
                TempValue = (string)(object) ViewModel.ApplyTextMode((T)(object)value)!;
            } else if (ViewModel.Type == INPUT_TYPE.LONG || ViewModel.Type == INPUT_TYPE.DOUBLE) {
                var isDecimal = ViewModel.Type == INPUT_TYPE.DOUBLE;

                if (value.StartsWith("00") || value.StartsWith("-00")) return;

                bool isOnlyPositive = isDecimal ? (double)(object) ViewModel.MinValue! >= 0 : (long)(object) ViewModel.MinValue! >= 0;
                if (isOnlyPositive && value.StartsWith("-")) return;

                var delimiterIndex = value.IndexOf(DELIMITER);
                if (isDecimal && value.IndexOf(DELIMITER) != value.LastIndexOf(DELIMITER)) {
                    return;
                }

                if (isDecimal && delimiterIndex >= 0) {
                    if (value.Length - (delimiterIndex + 1) > ViewModel.Decimals) return; 
                }

                bool isPositive = true;
                if (value == "" || value == "-" || (isDecimal && value == DELIMITER)) {
                    TempValue = value;
                    return;
                } else if (value.StartsWith("-")) {
                    isPositive = false;
                    value = value.Substring(1);
                }

                var min = (int) '0';
                var max = (int) '9';
                for (int i = 0; i < value.Length; i++) {
                    if (isDecimal && value[i] == DELIMITER[0]) continue;
                    var val = (int) value[i];
                    if (val < min || val > max) {
                        return;
                    }
                }
                TempValue = isPositive ? $"{value}" : $"-{value}";
            }
        } catch { return; }
        ViewModel.Error.ClearError();
        await OnInput.Invoke(TempValue);
    }

    private async Task HandleChange() {
        var val = TempValue;
        var changedValue = ViewModel.Value;
        try {
            if (ViewModel.Type == INPUT_TYPE.TEXT) {
                changedValue = (T)(object) val;
            } else if (ViewModel.Type == INPUT_TYPE.LONG || ViewModel.Type == INPUT_TYPE.DOUBLE) {

                var isDecimal = ViewModel.Type == INPUT_TYPE.DOUBLE;

                if (val == "" || val == "-" || (isDecimal && (val == DELIMITER || val == $"-{DELIMITER}"))) {
                    val = isDecimal ? Precision.ToStringDouble((double)(object) ViewModel.DefaultValue!) : $"{ViewModel.DefaultValue}";
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

                TempValue = val;
                if (isDecimal) {
                    val = val.Replace(DELIMITER[0], '.');

                    var value = double.Parse(val, CultureInfo.InvariantCulture);
                    var minValue = (double)(object) ViewModel.MinValue!;
                    var maxValue = (double)(object) ViewModel.MaxValue!;

                    if (value < minValue) changedValue = (T)(object) minValue;
                    else if (maxValue < value) changedValue = (T)(object) maxValue;
                    else changedValue = (T)(object) value;

                    if ((double)(object) changedValue! == -0D) {
                        TempValue = $"0{DELIMITER}0";
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
        } catch { changedValue = ViewModel.DefaultValue; }
        await ViewModel.SetValue(changedValue);
    }
}

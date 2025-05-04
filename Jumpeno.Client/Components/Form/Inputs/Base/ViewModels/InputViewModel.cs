namespace Jumpeno.Client.ViewModels;

public class InputViewModel<T> {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public readonly string Form;
    public readonly string ID;
    public readonly string FormID;
    public static string CreateFormID(string form, string id) => $"{form}.{id}";
    public readonly INPUT_TYPE Type;

    public readonly string Name;
    public readonly string Label;
    public readonly string? Placeholder;
    private bool _Secret;
    public bool Secret {
        get { return _Secret; }
        set {
            _Secret = value;
            Notify?.Invoke();
        }
    }

    public readonly INPUT_TEXT_MODE TextMode;
    public readonly bool Trim;
    public readonly Predicate<string>? TextCheck;
    public readonly int? MaxLength;
    public readonly int Decimals;
    public readonly T MinValue;
    public readonly T MaxValue;

    public readonly T DefaultValue;
    public T Value { get; private set; }
    public readonly EventDelegate<T> OnChange;
    public readonly EmptyDelegate OnEnter;

    public readonly InputErrorViewModel Error;
    public static InputErrorViewModel? ErrorViewModel(dynamic? viewModel) => viewModel?.Error;

    private readonly Action? Notify = null;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    private InputViewModel(InputViewModelParams<T> @params) {
        Form = @params.Form is null ? IDGenerator.Generate(nameof(Form).ToLower()) : @params.Form;
        ID = @params.ID is null ? IDGenerator.Generate(nameof(InputBase<object>)) : @params.ID;
        FormID = CreateFormID(Form, ID);
        Type = InitType(@params);
        Name = @params.Name;
        Label = @params.Label;
        Placeholder = @params.Placeholder;
        Secret = @params.Secret;
        TextMode = @params.TextMode;
        Trim = @params.Trim;
        TextCheck = @params.TextCheck;
        if (@params.MaxLength is not null) {
            Checker.CheckGreaterOrEqualTo((int) @params.MaxLength, 0);
            MaxLength = @params.MaxLength;
        }
        Decimals = InitDecimals(@params);
        var boundaries = GetBoundaries(@params);
        MinValue = boundaries[0].Value;
        MaxValue = boundaries[1].Value;

        DefaultValue = ConstrainedValue(@params.DefaultValue);
        Value = DefaultValue;
        OnChange = @params.OnChange is null ? new(v => {}) : @params.OnChange;
        OnEnter = @params.OnEnter is null ? new(() => {}) : @params.OnEnter;
        
        Error = new InputErrorViewModel();
    }
    public InputViewModel(InputViewModelTextParams @params) : this((InputViewModelParams<T>)(object) @params) {}
    public InputViewModel(InputViewModelLongParams @params) : this((InputViewModelParams<T>)(object) @params) {}
    public InputViewModel(InputViewModelDoubleParams @params) : this((InputViewModelParams<T>)(object) @params) {}

    // Initializers -----------------------------------------------------------------------------------------------------------------------
    private static INPUT_TYPE InitType(InputViewModelParams<T> @params) {
        var paramsType = @params.GetType();
        if (paramsType == typeof(InputViewModelTextParams)) return INPUT_TYPE.TEXT;
        else if (paramsType == typeof(InputViewModelLongParams)) return INPUT_TYPE.LONG;
        else if (paramsType == typeof(InputViewModelDoubleParams)) return INPUT_TYPE.DOUBLE;
        else return INPUT_TYPE.TEXT;
    }
    private static int InitDecimals(InputViewModelParams<T> @params) {
        if (@params.GetType() == typeof(InputViewModelDoubleParams)) {
            var decimals = ((InputViewModelDoubleParams)(object) @params).Decimals;
            Checker.CheckGreaterOrEqualTo(decimals, 1);
            return decimals;
        }
        return 0;
    }
    private static Boundary<T>[] CreateNumberBoundaries(
        OneOf<InputViewModelLongParams, InputViewModelDoubleParams> @params
    ) {
        OneOf<long, double> MinValue;
        OneOf<long, double> MaxValue;
        bool isMaxLengthError;
        bool isDecimalError = false;
        bool isBoundaryError;

        if (@params.IsT0) {
            MinValue = @params.AsT0.MinValue;
            MaxValue = @params.AsT0.MaxValue;

            isMaxLengthError = @params.AsT0.MaxLength is not null && $"{MinValue.AsT0}".Length > @params.AsT0.MaxLength || $"{MaxValue.AsT0}".Length > @params.AsT0.MaxLength;
            isBoundaryError = MaxValue.AsT0 < MinValue.AsT0;
        } else {
            MinValue = @params.AsT1.MinValue;
            MaxValue = @params.AsT1.MaxValue;
            
            var minParts = Precision.SplitDouble(MinValue.AsT1);
            var maxParts = Precision.SplitDouble(MaxValue.AsT1);
            isMaxLengthError =  @params.AsT1.MaxLength is not null && $"{minParts[0]}".Length > @params.AsT1.MaxLength || $"{maxParts[0]}".Length > @params.AsT1.MaxLength;
            isDecimalError = $"{minParts[1]}".Length > @params.AsT1.Decimals || $"{maxParts[1]}".Length > @params.AsT1.Decimals;

            isBoundaryError = MaxValue.AsT1 < MinValue.AsT1;
        }

        if (isMaxLengthError) throw new InvalidDataException("Boundary does not match MaxLength condition!");
        if (isDecimalError) throw new InvalidDataException("Boundary does not match Decimals condition!");
        if (isBoundaryError) throw new InvalidDataException("Max value must be greater or equal to min value!");

        return [
            new(MinValue.IsT0 ? (T)(object) MinValue.AsT0 : (T)(object) MinValue.AsT1, false),
            new(MaxValue.IsT0 ? (T)(object) MaxValue.AsT0 : (T)(object) MaxValue.AsT1, false)
        ];
    }
    private static Boundary<T>[] GetBoundaries(InputViewModelParams<T> @params) {
        var propType = @params.GetType();        
        if (propType == typeof(InputViewModelLongParams)) {
            return CreateNumberBoundaries((InputViewModelLongParams)(object) @params);
        } else if (propType == typeof(InputViewModelDoubleParams)) {
            return CreateNumberBoundaries((InputViewModelDoubleParams)(object) @params);
        }
        return [new(default!, false), new(default!, false)];
    }
    public T ApplyTextMode(T value) {
        if (Type == INPUT_TYPE.TEXT) {
            switch (TextMode) {
                case INPUT_TEXT_MODE.LOWERCASE:
                    return (T)(object)value!.ToString()!.ToLower();
                case INPUT_TEXT_MODE.UPPERCASE:
                    return (T)(object)value!.ToString()!.ToUpper();
            }
        }
        return value;
    }
    public T ApplyTrim(T value) {
        if (Type == INPUT_TYPE.TEXT && Trim) {
            return (T)(object) value!.ToString()!.Trim();
        }
        return value;
    }
    private T ConstrainedValue(T value) {
        var valString = $"{value}";
        T valResult;
        try {
            if (Type == INPUT_TYPE.LONG) {
                var val = long.Parse(valString);
                var minValue = (long)(object) MinValue!;
                var maxValue = (long)(object) MaxValue!;

                if (val < minValue) { val = minValue; valString = $"{val}"; }
                if (val > maxValue) { val = maxValue; valString = $"{val}"; }
                if (MaxLength is not null && valString.Length > MaxLength) {
                    valString = valString.Substring(0, (int) MaxLength);
                    val = long.Parse(valString);
                }

                valResult = (T)(object) val;

            } else if (Type == INPUT_TYPE.DOUBLE) {
                var val = Precision.ParseDouble(valString);
                var minValue = (double)(object) MinValue!;
                var maxValue = (double)(object) MaxValue!;

                if (val < minValue) { val = minValue; valString = Precision.ToStringDouble(val); }
                if (val > maxValue) { val = maxValue; valString = Precision.ToStringDouble(val); }
                
                var parts = Precision.SplitDouble(valString);
                if (MaxLength is not null && parts[0].Length > MaxLength) {
                    parts[0] = parts[0].Substring(parts[0].Length - (int) MaxLength);
                    valString = $"{parts[0]}.{parts[1]}";
                    val = Precision.ParseDouble(valString);
                }
                if (parts[1].Length > Decimals) {
                    parts[1] = parts[1].Substring(0, Decimals);
                    valString = $"{parts[0]}.{parts[1]}";
                    val = Precision.ParseDouble(valString);
                }

                valResult = (T)(object) val;
            } else {
                var val = valString;
                if (MaxLength is not null && valString.Length > MaxLength) val = val.Substring(0, (int) MaxLength);
                valResult = (T)(object) val;
            }
        } catch {
            valResult = DefaultValue;
        }
        return ApplyTrim(ApplyTextMode(valResult));
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task SetValue(T value, bool skipEvent = false) {
        T previous = Value;
        Value = ConstrainedValue(value);
        if (!Value!.Equals(previous)) {
            Error.ClearError();
            if (!skipEvent) {
                await OnChange.Invoke(Value);
            }
        }
        Notify?.Invoke();
    }

    public async Task Clear() {
        await SetValue(DefaultValue);
    }
}

namespace Jumpeno.Client.ViewModels;

public class InputViewModel<T> : FormViewModel {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public readonly InputType Type;
    // Input:
    public readonly InputTextMode TextMode;
    public readonly bool Trim;
    public readonly Predicate<string>? TextCheck;
    public readonly int? MaxLength;
    public readonly int Decimals;
    public readonly T MinValue;
    public readonly T MaxValue;
    // Value:
    public readonly string? Placeholder;
    public readonly T DefaultValue;
    public readonly T ClearValue;
    public T Value { get; private set; }
    public void SetValue(T value) {
        T previous = Value;
        Value = ConstrainedValue(value);
        if (!Value!.Equals(previous)) {
            Error.Clear();
        }
        React();
    }
    public void Clear() => SetValue(ClearValue);
    // Secret:
    private bool secret;
    public bool Secret {
        get { return secret; }
        set { secret = value; Notify(); }
    }

    // Events -----------------------------------------------------------------------------------------------------------------------------
    public EventDelegate<InputEvent<T>> OnInput { get; set; }
    public EventDelegate<InputEvent<T>> OnClear { get; set; }
    public EventDelegate<InputEvent<T>> OnChange { get; set; }
    public EventDelegate<InputEvent<T>> OnEnter { get; set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    private InputViewModel(InputViewModelParams<T> p) : base(p.Form, p.ID, p.OnError) {
        Type = InitType(p);
        Placeholder = p.Placeholder;
        Secret = p.Secret;
        TextMode = p.TextMode;
        Trim = p.Trim;
        TextCheck = p.TextCheck;
        if (p.MaxLength is not null) {
            Checker.CheckGreaterOrEqualTo((int) p.MaxLength, 0);
            MaxLength = p.MaxLength;
        }
        Decimals = InitDecimals(p);
        var boundaries = GetBoundaries(p);
        MinValue = boundaries[0].Value;
        MaxValue = boundaries[1].Value;

        DefaultValue = ConstrainedValue(p.DefaultValue);
        ClearValue = ConstrainedValue(p.ClearValue);
        Value = DefaultValue;
        OnInput = p.OnInput ?? new(e => {});
        OnClear = p.OnClear ?? new(e => {});
        OnChange = p.OnChange ?? new(e => {});
        OnEnter = p.OnEnter ?? new(e => {});
    }
    public InputViewModel(InputViewModelTextParams @params) : this((InputViewModelParams<T>)(object) @params) {}
    public InputViewModel(InputViewModelLongParams @params) : this((InputViewModelParams<T>)(object) @params) {}
    public InputViewModel(InputViewModelDoubleParams @params) : this((InputViewModelParams<T>)(object) @params) {}

    // Initializers -----------------------------------------------------------------------------------------------------------------------
    private static InputType InitType(InputViewModelParams<T> @params) {
        var paramsType = @params.GetType();
        if (paramsType == typeof(InputViewModelTextParams)) return InputType.Text;
        else if (paramsType == typeof(InputViewModelLongParams)) return InputType.Long;
        else if (paramsType == typeof(InputViewModelDoubleParams)) return InputType.Double;
        else return InputType.Text;
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
        OneOf<long, double> minValue;
        OneOf<long, double> maxValue;
        bool isMaxLengthError;
        bool isDecimalError = false;
        bool isBoundaryError;

        if (@params.IsT0) {
            minValue = @params.AsT0.MinValue;
            maxValue = @params.AsT0.MaxValue;

            isMaxLengthError = @params.AsT0.MaxLength is not null && $"{minValue.AsT0}".Length > @params.AsT0.MaxLength || $"{maxValue.AsT0}".Length > @params.AsT0.MaxLength;
            isBoundaryError = maxValue.AsT0 < minValue.AsT0;
        } else {
            minValue = @params.AsT1.MinValue;
            maxValue = @params.AsT1.MaxValue;
            
            var minParts = Precision.SplitDouble(minValue.AsT1);
            minParts[1] = minParts[1].Substring(0, Math.Min(@params.AsT1.Decimals, minParts[1].Length));
            var maxParts = Precision.SplitDouble(maxValue.AsT1);
            maxParts[1] = maxParts[1].Substring(0, Math.Min(@params.AsT1.Decimals, maxParts[1].Length));
            isMaxLengthError =  @params.AsT1.MaxLength is not null && $"{minParts[0]}".Length > @params.AsT1.MaxLength || $"{maxParts[0]}".Length > @params.AsT1.MaxLength;
            isDecimalError = $"{minParts[1]}".Length > @params.AsT1.Decimals || $"{maxParts[1]}".Length > @params.AsT1.Decimals;

            isBoundaryError = maxValue.AsT1 < minValue.AsT1;
        }

        if (isMaxLengthError) throw new InvalidDataException("Boundary does not match MaxLength condition!");
        if (isDecimalError) throw new InvalidDataException("Boundary does not match Decimals condition!");
        if (isBoundaryError) throw new InvalidDataException("Max value must be greater or equal to min value!");

        return [
            new(minValue.IsT0 ? (T)(object) minValue.AsT0 : (T)(object) minValue.AsT1, false),
            new(maxValue.IsT0 ? (T)(object) maxValue.AsT0 : (T)(object) maxValue.AsT1, false)
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
        if (Type == InputType.Text) {
            switch (TextMode) {
                case InputTextMode.LowerCase:
                    return (T)(object)value!.ToString()!.ToLower();
                case InputTextMode.UpperCase:
                    return (T)(object)value!.ToString()!.ToUpper();
            }
        }
        return value;
    }
    public T ApplyTrim(T value) {
        if (Type == InputType.Text && Trim) {
            return (T)(object) value!.ToString()!.Trim();
        }
        return value;
    }
    private T ConstrainedValue(T value) {
        var valString = $"{value}";
        T valResult;
        try {
            if (Type == InputType.Long) {
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

            } else if (Type == InputType.Double) {
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
            valResult = ClearValue;
        }
        return ApplyTrim(ApplyTextMode(valResult));
    }
}

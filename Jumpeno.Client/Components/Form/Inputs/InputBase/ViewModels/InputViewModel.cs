namespace Jumpeno.Client.ViewModels;

public class InputViewModel<T> {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public readonly string ID;
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

    public readonly InputErrorViewModel Error;
    private readonly Action? Notify = null;
    
    // Initializers -----------------------------------------------------------------------------------------------------------------------
    private static INPUT_TYPE InitType(InputViewModelProps<T> props) {
        var propsType = props.GetType();
        if (propsType == typeof(InputViewModelTextProps)) return INPUT_TYPE.TEXT;
        else if (propsType == typeof(InputViewModelLongProps)) return INPUT_TYPE.LONG;
        else if (propsType == typeof(InputViewModelDoubleProps)) return INPUT_TYPE.DOUBLE;
        else return INPUT_TYPE.TEXT;
    }
    private static int InitDecimals(InputViewModelProps<T> props) {
        if (props.GetType() == typeof(InputViewModelDoubleProps)) {
            var decimals = ((InputViewModelDoubleProps)(object) props).Decimals;
            Checker.CheckGreaterOrEqualTo(decimals, 1);
            return decimals;
        }
        return 0;
    }
    private static Boundary<T>[] CreateNumberBoundaries(
        OneOf<InputViewModelLongProps, InputViewModelDoubleProps> props
    ) {
        OneOf<long, double> MinValue;
        OneOf<long, double> MaxValue;
        bool isMaxLengthError;
        bool isDecimalError = false;
        bool isBoundaryError;

        if (props.IsT0) {
            MinValue = props.AsT0.MinValue;
            MaxValue = props.AsT0.MaxValue;

            isMaxLengthError = props.AsT0.MaxLength is not null && $"{MinValue.AsT0}".Length > props.AsT0.MaxLength || $"{MaxValue.AsT0}".Length > props.AsT0.MaxLength;
            isBoundaryError = MaxValue.AsT0 < MinValue.AsT0;
        } else {
            MinValue = props.AsT1.MinValue;
            MaxValue = props.AsT1.MaxValue;
            
            var minParts = Precision.SplitDouble(MinValue.AsT1);
            var maxParts = Precision.SplitDouble(MaxValue.AsT1);
            isMaxLengthError =  props.AsT1.MaxLength is not null && $"{minParts[0]}".Length > props.AsT1.MaxLength || $"{maxParts[0]}".Length > props.AsT1.MaxLength;
            isDecimalError = $"{minParts[1]}".Length > props.AsT1.Decimals || $"{maxParts[1]}".Length > props.AsT1.Decimals;

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
    private static Boundary<T>[] GetBoundaries(InputViewModelProps<T> props) {
        var propType = props.GetType();        
        if (propType == typeof(InputViewModelLongProps)) {
            return CreateNumberBoundaries((InputViewModelLongProps)(object) props);
        } else if (propType == typeof(InputViewModelDoubleProps)) {
            return CreateNumberBoundaries((InputViewModelDoubleProps)(object) props);
        }
        return [new (default!, false), new (default!, false)];
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

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    private InputViewModel(InputViewModelProps<T> props) {
        ID = props.ID is null ? ComponentService.GenerateID(InputBase<T>.CLASS_INPUT_BASE) : props.ID;
        Type = InitType(props);
        Name = props.Name;
        Label = props.Label;
        Placeholder = props.Placeholder;
        Secret = props.Secret;
        TextMode = props.TextMode;
        Trim = props.Trim;
        TextCheck = props.TextCheck;
        if (props.MaxLength is not null) {
            Checker.CheckGreaterOrEqualTo((int) props.MaxLength, 0);
            MaxLength = props.MaxLength;
        }
        Decimals = InitDecimals(props);
        var boundaries = GetBoundaries(props);
        MinValue = boundaries[0].Value;
        MaxValue = boundaries[1].Value;

        DefaultValue = ConstrainedValue(props.DefaultValue);
        Value = DefaultValue;
        OnChange = props.OnChange is null ? new(v => {}) : props.OnChange;
        
        Error = new InputErrorViewModel();
    }
    public InputViewModel(InputViewModelTextProps props) : this((InputViewModelProps<T>)(object) props) {}
    public InputViewModel(InputViewModelLongProps props) : this((InputViewModelProps<T>)(object) props) {}
    public InputViewModel(InputViewModelDoubleProps props) : this((InputViewModelProps<T>)(object) props) {}

    // Methods ----------------------------------------------------------------------------------------------------------------------------
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

    public void Clear() {
        Value = DefaultValue;
        Error.ClearError();
        Notify?.Invoke();
    }
}

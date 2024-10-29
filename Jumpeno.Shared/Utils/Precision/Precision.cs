namespace Jumpeno.Shared.Utils;

using System.Globalization;

public static class Precision {
    // Double -----------------------------------------------------------------------------------------------------------------------------
    public static string NormDouble(string value) {
        var val = value;
        val = val.Replace(",", ".");
        var index = val.IndexOf('.');
        if (index >= 0) {
            if (index == val.Length - 1) val = $"{val}0";
        } else val = $"{val}.0";
        return val;
    }
    public static string NormDouble(double value) {
        return NormDouble($"{value}");
    }

    public static double ParseDouble(string value) {
        return double.Parse(NormDouble(value), CultureInfo.InvariantCulture);
    }
    public static string ToStringDouble(double value) {
        return NormDouble($"{value}");
    }
    
    public static string SetDelimiter(string value, string delimiter) {
        var val = NormDouble(value);
        return val.Replace(".", delimiter);
    }
    public static string SetDelimiter(double value, string delimiter) {
        return SetDelimiter($"{value}", delimiter);
    }
    
    public static string[] SplitDouble(string value) {
        var val = NormDouble(value);
        return val.Split('.');
    }
    public static string[] SplitDouble(double value) {
        return SplitDouble($"{value}");
    }

    public static string TruncDouble(string value, int decimals) {
        var val = NormDouble(value);
        var parts = SplitDouble(val);

        if (decimals > parts[1].Length) return val;

        if (decimals > 0) parts[1] = parts[1].Substring(0, decimals);
        else {
            decimals = -decimals;
            if (decimals >= parts[0].Length) return "0.0";
            parts[0] = parts[0].Substring(0, parts[0].Length - decimals);
            for (int i = 0; i < decimals; i++) {
                parts[0] = $"{parts[0]}{0}";
            }
            parts[1] = "0";
        }

        return $"{parts[0]}.{parts[1]}";
    }
    public static double TruncDouble(double value, int decimals) {
        return ParseDouble(TruncDouble($"{value}", decimals));
    }

    public static double GetNextDouble(double value) {
        // Handle special cases for infinity and NaN
        if (double.IsNaN(value) || value == double.PositiveInfinity) {
            return value;
        }
        if (value == double.NegativeInfinity) {
            return double.MinValue;
        }

        // Convert double to its bit representation
        long bits = BitConverter.DoubleToInt64Bits(value);

        // If the number is positive, incrementing the bit representation gives the next higher value
        // If it's negative, decrementing the bit representation gives the next higher value
        if (value > 0) {
            bits++;
        } else if (value < 0) {
            bits--;
        } else {
            // Handle the case for zero (0.0 and -0.0)
            bits++;
        }

        // Convert the incremented bit representation back to double
        return BitConverter.Int64BitsToDouble(bits);
    }
    public static double GetPreviousDouble(double value) {
        // Handle special cases for infinity and NaN
        if (double.IsNaN(value) || value == double.NegativeInfinity) {
            return value;
        }
        if (value == double.PositiveInfinity) {
            return double.MaxValue;
        }

        // Convert double to its bit representation
        long bits = BitConverter.DoubleToInt64Bits(value);

        // If the number is positive, decrementing the bit representation gives the next lower value
        // If it's negative, incrementing the bit representation gives the next lower value
        if (value > 0) {
            bits--;
        } else if (value < 0) {
            bits++;
        } else {
            // Handle the case for zero (0.0 and -0.0)
            bits--;
        }

        // Convert the decremented bit representation back to double
        return BitConverter.Int64BitsToDouble(bits);
    }

    // Long -------------------------------------------------------------------------------------------------------------------------------
    public static long GetPreviousLong(long value) {
        if (value == long.MinValue) return value;
        return value - 1;
    }
    public static long GetNextLong(long value) {
        if (value == long.MaxValue) return value;
        return value + 1;
    }
}

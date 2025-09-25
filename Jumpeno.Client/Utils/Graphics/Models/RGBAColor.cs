namespace Jumpeno.Client.Models;

public record RGBAColor : RGBColor {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public float A { get; protected set; }

    // Validation -------------------------------------------------------------------------------------------------------------------------
    public static bool IsAlpha(float value) => 0 <= value && value <= 1;
    public static float CheckAlpha(float value) => IsAlpha(value) ? value : throw new ArgumentException("Alpha not in range <0, 1>");

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    public RGBAColor(byte r, byte g, byte b, float a = 1) : base(r, g, b) { CheckAlpha(a); A = a; }
    public RGBAColor(byte[] parts, float a = 1) : this(parts[0], parts[1], parts[2], a) {}
    public RGBAColor(string color) {
        var parts = color.Split(',');
        R = byte.Parse(parts[0]);
        G = byte.Parse(parts[1]);
        B = byte.Parse(parts[2]);
        A = float.Parse(parts[3]);
        CheckAlpha(A);
    }
    public RGBAColor(RGBColor color, float a = 1) : this(color.R, color.G, color.B, a) {}

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static RGBAColor Blend(RGBAColor color1, double percentage, RGBAColor color2) {
        CheckPercentage(percentage);
        return new(
            (byte) Math.Min(color1.R * percentage + color2.R * (1 - percentage), byte.MaxValue),
            (byte) Math.Min(color1.G * percentage + color2.G * (1 - percentage), byte.MaxValue),
            (byte) Math.Min(color1.B * percentage + color2.B * (1 - percentage), byte.MaxValue),
            (byte) Math.Min(color1.A * percentage + color2.A * (1 - percentage), byte.MaxValue)
        );
    }
    public RGBAColor Blend(double percentage, RGBAColor color) => Blend(this, percentage, color);

    // Operators --------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => $"rgba({R}, {G}, {B}, {$"{A}".Replace(',', '.')})";
    public static implicit operator string(RGBAColor color) => color.ToString();
}

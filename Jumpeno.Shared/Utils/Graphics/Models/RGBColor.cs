namespace Jumpeno.Shared.Models;

public record RGBColor {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public byte R { get; protected set; }
    public byte G { get; protected set; }
    public byte B { get; protected set; }

    // Validation -------------------------------------------------------------------------------------------------------------------------
    public static bool IsPercentage(double value) => 0 <= value && value <= 1;
    public static double CheckPercentage(double value) => IsPercentage(value) ? value : throw new ArgumentException("Percentage not in range <0, 1>");

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    public RGBColor(byte r, byte g, byte b) { R = r; G = g; B = b; }
    public RGBColor(byte[] parts) : this(parts[0], parts[1], parts[2]) {}
    public RGBColor(string color) : this(color.Split(',').Select(byte.Parse).ToArray()) {}
    protected RGBColor() {}

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static RGBColor Blend(RGBColor color1, double percentage, RGBColor color2) {
        CheckPercentage(percentage);
        return new(
            (byte) Math.Min(color1.R * percentage + color2.R * (1 - percentage), byte.MaxValue),
            (byte) Math.Min(color1.G * percentage + color2.G * (1 - percentage), byte.MaxValue),
            (byte) Math.Min(color1.B * percentage + color2.B * (1 - percentage), byte.MaxValue)
        );
    }
    public RGBColor Blend(double percentage, RGBColor color) => Blend(this, percentage, color);

    // Operators --------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => $"{R}, {G}, {B}";
    public static implicit operator string(RGBColor color) => color.ToString();
}

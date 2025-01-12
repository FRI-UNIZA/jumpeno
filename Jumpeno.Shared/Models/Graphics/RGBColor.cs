namespace Jumpeno.Shared.Models;

public record RGBColor {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public byte R { get; private set; }
    public byte G { get; private set; }
    public byte B { get; private set; }    

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    public RGBColor(byte r, byte g, byte b) => InitParts(r, g, b);
    public RGBColor(byte[] parts) => InitParts(parts[0], parts[1], parts[2]);
    public RGBColor(string color) : this(color.Split(',').Select(byte.Parse).ToArray()) {}

    // Initializers -----------------------------------------------------------------------------------------------------------------------
    private void InitParts(byte r, byte g, byte b) { R = r; G = g; B = b; }

    // Operators --------------------------------------------------------------------------------------------------------------------------
    public static RGBColor Blend(RGBColor color1, double percentage, RGBColor color2) {
        if (percentage < 0 || 1 < percentage) throw new Exception("Percentage not in range <0, 1>");
        return new(
            (byte) Math.Min(color1.R * percentage + color2.R * (1 - percentage), byte.MaxValue),
            (byte) Math.Min(color1.G * percentage + color2.G * (1 - percentage), byte.MaxValue),
            (byte) Math.Min(color1.B * percentage + color2.B * (1 - percentage), byte.MaxValue)
        );
    }
    public RGBColor Blend(double percentage, RGBColor color) => Blend(this, percentage, color);

    // Operators --------------------------------------------------------------------------------------------------------------------------
    public byte[] ToArray() => [R, G, B];
    public override string ToString() => $"{R}, {G}, {B}";
    public static implicit operator string(RGBColor color) => color.ToString();
}

namespace Jumpeno.Client.Constants;

public class DarkTheme : BaseTheme {
    // Themes -----------------------------------------------------------------------------------------------------------------------------
    public override SCROLLAREA_THEME SCROLL_THEME { get; } = SCROLLAREA_THEME.OS_THEME_DARK;
    // Danger surface ---------------------------------------------------------------------------------------------------------------------
    public override RGBColor COLOR_DANGER { get; } = new(83, 0, 0);
    public override RGBColor COLOR_DANGER_HIGHLIGHT { get; } = new(143, 0, 0);
    public override RGBColor COLOR_DANGER_SURFACE { get; } = new(255, 186, 186);
    public override RGBColor COLOR_DANGER_SURFACE_HIGHLIGHT { get; } = new(255, 219, 219);
    public override RGBColor COLOR_DANGER_ACCENT { get; } = new(255, 0, 0);
    public override RGBColor COLOR_DANGER_ACCENT_HIGHLIGHT { get; } = new(253, 118, 118);
    // Success surface --------------------------------------------------------------------------------------------------------------------
    public override RGBColor COLOR_SUCCESS { get; } = new(82, 196, 26);
    public override RGBColor COLOR_SUCCESS_HIGHLIGHT { get; } = new(0, 255, 0);
    // Primary surface --------------------------------------------------------------------------------------------------------------------
    public override RGBColor COLOR_PRIMARY { get; } = new(42, 33, 56);
    public override RGBColor COLOR_PRIMARY_HIGHLIGHT { get; } = new(122, 113, 136);
    public override RGBColor COLOR_PRIMARY_ACCENT { get; } = new(42, 33, 56);
    public override RGBColor COLOR_PRIMARY_ACCENT_HIGHLIGHT { get; } = new(122, 113, 136);
    public override RGBColor COLOR_PRIMARY_SURFACE { get; } = new(255, 255, 83);
    public override RGBColor COLOR_PRIMARY_SURFACE_HIGHLIGHT { get; } = new(225, 225, 53);
    public override RGBColor COLOR_PRIMARY_BOX { get; } = new(250, 250, 175);
    public override RGBColor COLOR_PRIMARY_SHADE { get; } = new(220, 220, 220);
    public override RGBColor COLOR_PRIMARY_SHADE_INVERT { get; } = new(60, 60, 60);
    public override RGBColor COLOR_PRIMARY_SHADE_STRONG { get; } = new(190, 190, 190);
    public override RGBColor COLOR_PRIMARY_SHADE_STRONG_INVERT { get; } = new(90, 90, 90);
    // Secondary surface ------------------------------------------------------------------------------------------------------------------
    public override RGBColor COLOR_SECONDARY { get; } = new(236, 240, 241);
    public override RGBColor COLOR_SECONDARY_HIGHLIGHT { get; } = new(255, 255, 255);
    public override RGBColor COLOR_SECONDARY_ACCENT { get; } = new(255, 215, 0);
    public override RGBColor COLOR_SECONDARY_ACCENT_HIGHLIGHT { get; } = new(255, 239, 0);
    public override RGBColor COLOR_SECONDARY_SURFACE { get; } = new(42, 33, 56);
    public override RGBColor COLOR_SECONDARY_SURFACE_HIGHLIGHT { get; } = new(72, 63, 86);
    public override RGBColor COLOR_SECONDARY_BOX { get; } = new(57, 48, 71);
    public override RGBColor COLOR_SECONDARY_SHADE { get; } = new(170, 170, 170);
    public override RGBColor COLOR_SECONDARY_SHADE_INVERT { get; } = new(236, 240, 241);
    public override RGBColor COLOR_SECONDARY_SHADE_STRONG { get; } = new(140, 140, 140);
    public override RGBColor COLOR_SECONDARY_SHADE_STRONG_INVERT { get; } = new(206, 210, 211);
    // Floating surface -------------------------------------------------------------------------------------------------------------------
        // Colors -------------------------------------------------------------------------------------------------------------------------
        public override RGBColor COLOR_FLOATING { get; } = new(42, 33, 56);
        public override RGBColor COLOR_FLOATING_HIGHLIGHT { get; } = new(122, 113, 136);
        public override RGBColor COLOR_FLOATING_ACCENT { get; } = new(42, 33, 56);
        public override RGBColor COLOR_FLOATING_ACCENT_HIGHLIGHT { get; } = new(122, 113, 136);
        public override RGBColor COLOR_FLOATING_SURFACE { get; } = new(255, 255, 255);
        public override RGBColor COLOR_FLOATING_SURFACE_HIGHLIGHT { get; } = new(225, 225, 225);
        public override RGBColor COLOR_FLOATING_BOX { get; } = new(245, 245, 245);
        public override RGBColor COLOR_FLOATING_SHADE { get; } = new(220, 220, 220);
        public override RGBColor COLOR_FLOATING_SHADE_INVERT { get; } = new(60, 60, 60);
        public override RGBColor COLOR_FLOATING_SHADE_STRONG { get; } = new(190, 190, 190);
        public override RGBColor COLOR_FLOATING_SHADE_STRONG_INVERT { get; } = new(90, 90, 90);
        // Shades -------------------------------------------------------------------------------------------------------------------------
        public override string BOX_SHADOW_FLOATING_LEVEL_1 { get; } = $"0 0 10px rgba(var(--color-base), 0.1)";
        public override string BOX_SHADOW_FLOATING_LEVEL_2 { get; } = $"0 0 30px 0 rgba(var(--color-base), 0.5)";
    // Selection surface ------------------------------------------------------------------------------------------------------------------
    public override RGBColor COLOR_SELECTION { get; } = new(42, 33, 56);
    public override RGBColor COLOR_SELECTION_BACKGROUND { get; } = new(255, 239, 0);
}

namespace Jumpeno.Client.Constants;

// Mandatory using statement for generator script:
using Jumpeno.Client.Constants;

public class DarkTheme: BaseTheme {
    // Themes -----------------------------------------------------------------------------------------------------------------------------
    public override SCROLLAREA_THEME SCROLL_THEME { get; } = SCROLLAREA_THEME.OS_THEME_DARK;
    // Danger surface ---------------------------------------------------------------------------------------------------------------------
    public override string COLOR_DANGER { get; } = "83, 0, 0";
    public override string COLOR_DANGER_HIGHLIGHT { get; } = "143, 0, 0";
    public override string COLOR_DANGER_SURFACE { get; } = "255, 186, 186";
    public override string COLOR_DANGER_SURFACE_HIGHLIGHT { get; } = "255, 219, 219";
    public override string COLOR_DANGER_ACCENT { get; } = "255, 0, 0";
    public override string COLOR_DANGER_ACCENT_HIGHLIGHT { get; } = "253, 118, 118";
    // Primary surface --------------------------------------------------------------------------------------------------------------------
    public override string COLOR_PRIMARY { get; } = "42, 33, 56";
    public override string COLOR_PRIMARY_HIGHLIGHT { get; } = "122, 113, 136";
    public override string COLOR_PRIMARY_ACCENT { get; } = "42, 33, 56";
    public override string COLOR_PRIMARY_ACCENT_HIGHLIGHT { get; } = "122, 113, 136";
    public override string COLOR_PRIMARY_SURFACE { get; } = "255, 255, 83";
    public override string COLOR_PRIMARY_SURFACE_HIGHLIGHT { get; } = "225, 225, 53";
    public override string COLOR_PRIMARY_SHADE { get; } = "220, 220, 220";
    public override string COLOR_PRIMARY_SHADE_INVERT { get; } = "60, 60, 60";
    public override string COLOR_PRIMARY_SHADE_STRONG { get; } = "190, 190, 190";
    public override string COLOR_PRIMARY_SHADE_STRONG_INVERT { get; } = "90, 90, 90";
    // Secondary surface ------------------------------------------------------------------------------------------------------------------
    public override string COLOR_SECONDARY { get; } = "236, 240, 241";
    public override string COLOR_SECONDARY_HIGHLIGHT { get; } = "255, 255, 255";
    public override string COLOR_SECONDARY_ACCENT { get; } = "255, 215, 0";
    public override string COLOR_SECONDARY_ACCENT_HIGHLIGHT { get; } = "255, 239, 0";
    public override string COLOR_SECONDARY_SURFACE { get; } = "42, 33, 56";
    public override string COLOR_SECONDARY_SURFACE_HIGHLIGHT { get; } = "72, 63, 86";
    public override string COLOR_SECONDARY_SHADE { get; } = "170, 170, 170";
    public override string COLOR_SECONDARY_SHADE_INVERT { get; } = "236, 240, 241";
    public override string COLOR_SECONDARY_SHADE_STRONG { get; } = "140, 140, 140";
    public override string COLOR_SECONDARY_SHADE_STRONG_INVERT { get; } = "206, 210, 211";
    // Floating surface -------------------------------------------------------------------------------------------------------------------
        // Colors -------------------------------------------------------------------------------------------------------------------------
        public override string COLOR_FLOATING { get; } = "42, 33, 56";
        public override string COLOR_FLOATING_HIGHLIGHT { get; } = "122, 113, 136";
        public override string COLOR_FLOATING_ACCENT { get; } = "42, 33, 56";
        public override string COLOR_FLOATING_ACCENT_HIGHLIGHT { get; } = "122, 113, 136";
        public override string COLOR_FLOATING_SURFACE { get; } = "255, 255, 255";
        public override string COLOR_FLOATING_SURFACE_HIGHLIGHT { get; } = "225, 225, 225";
        public override string COLOR_FLOATING_SHADE { get; } = "220, 220, 220";
        public override string COLOR_FLOATING_SHADE_INVERT { get; } = "60, 60, 60";
        public override string COLOR_FLOATING_SHADE_STRONG { get; } = "190, 190, 190";
        public override string COLOR_FLOATING_SHADE_STRONG_INVERT { get; } = "90, 90, 90";
        // Shades -------------------------------------------------------------------------------------------------------------------------
        public override string BOX_SHADOW_FLOATING_LEVEL_1 { get; } = "0 0 10px rgba(var(--color-base), 0.1)";
        public override string BOX_SHADOW_FLOATING_LEVEL_2 { get; } = "0 0 30px 0 rgba(var(--color-base), 0.5)";
    // Selection surface ------------------------------------------------------------------------------------------------------------------
    public override string COLOR_SELECTION { get; } = "42, 33, 56";
    public override string COLOR_SELECTION_BACKGROUND { get; } = "255, 239, 0";
}

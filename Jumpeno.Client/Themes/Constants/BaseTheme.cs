namespace Jumpeno.Client.Constants;

#pragma warning disable CS8618

public class BaseTheme {
    // Themes -----------------------------------------------------------------------------------------------------------------------------
    public virtual SCROLLAREA_THEME SCROLL_THEME { get; }

    // Base surface -----------------------------------------------------------------------------------------------------------------------
    public RGBColor COLOR_BASE { get; } = new(0, 0, 0);
    public RGBColor COLOR_BASE_HIGHLIGHT { get; } = new(40, 40, 40);
    public RGBColor COLOR_BASE_INVERT { get; } = new(255, 255, 255);
    public RGBColor COLOR_BASE_INVERT_HIGHLIGHT { get; } = new(220, 220, 220);
    public RGBColor COLOR_BASE_ERROR { get; } = new(255, 0, 0);
    public RGBColor COLOR_BASE_SUCCESS { get; } = new(0, 255, 0);
    // Danger surface ---------------------------------------------------------------------------------------------------------------------
    public virtual RGBColor COLOR_DANGER { get; }
    public virtual RGBColor COLOR_DANGER_HIGHLIGHT { get; }
    public virtual RGBColor COLOR_DANGER_SURFACE { get; }
    public virtual RGBColor COLOR_DANGER_SURFACE_HIGHLIGHT { get; }
    public virtual RGBColor COLOR_DANGER_ACCENT { get; }
    public virtual RGBColor COLOR_DANGER_ACCENT_HIGHLIGHT { get; }
    // Primary surface --------------------------------------------------------------------------------------------------------------------
    public virtual RGBColor COLOR_PRIMARY { get; }
    public virtual RGBColor COLOR_PRIMARY_HIGHLIGHT { get; }
    public virtual RGBColor COLOR_PRIMARY_ACCENT { get; }
    public virtual RGBColor COLOR_PRIMARY_ACCENT_HIGHLIGHT { get; }
    public virtual RGBColor COLOR_PRIMARY_SURFACE { get; }
    public virtual RGBColor COLOR_PRIMARY_SURFACE_HIGHLIGHT { get; }
    public virtual RGBColor COLOR_PRIMARY_BOX { get; }
    public virtual RGBColor COLOR_PRIMARY_SHADE { get; }
    public virtual RGBColor COLOR_PRIMARY_SHADE_INVERT { get; }
    public virtual RGBColor COLOR_PRIMARY_SHADE_STRONG { get; }
    public virtual RGBColor COLOR_PRIMARY_SHADE_STRONG_INVERT { get; }
    // Secondary surface ------------------------------------------------------------------------------------------------------------------
    public virtual RGBColor COLOR_SECONDARY { get; }
    public virtual RGBColor COLOR_SECONDARY_HIGHLIGHT { get; }
    public virtual RGBColor COLOR_SECONDARY_ACCENT { get; }
    public virtual RGBColor COLOR_SECONDARY_ACCENT_HIGHLIGHT { get; }
    public virtual RGBColor COLOR_SECONDARY_SURFACE { get; }
    public virtual RGBColor COLOR_SECONDARY_SURFACE_HIGHLIGHT { get; }
    public virtual RGBColor COLOR_SECONDARY_BOX { get; }
    public virtual RGBColor COLOR_SECONDARY_SHADE { get; }
    public virtual RGBColor COLOR_SECONDARY_SHADE_INVERT { get; }
    public virtual RGBColor COLOR_SECONDARY_SHADE_STRONG { get; }
    public virtual RGBColor COLOR_SECONDARY_SHADE_STRONG_INVERT { get; }
    // Floating surface -------------------------------------------------------------------------------------------------------------------
        // Colors -------------------------------------------------------------------------------------------------------------------------
            public virtual RGBColor COLOR_FLOATING { get; }
            public virtual RGBColor COLOR_FLOATING_HIGHLIGHT { get; }
            public virtual RGBColor COLOR_FLOATING_ACCENT { get; }
            public virtual RGBColor COLOR_FLOATING_ACCENT_HIGHLIGHT { get; }
            public virtual RGBColor COLOR_FLOATING_SURFACE { get; }
            public virtual RGBColor COLOR_FLOATING_SURFACE_HIGHLIGHT { get; }
            public virtual RGBColor COLOR_FLOATING_BOX { get; }
            public virtual RGBColor COLOR_FLOATING_SHADE { get; }
            public virtual RGBColor COLOR_FLOATING_SHADE_INVERT { get; }
            public virtual RGBColor COLOR_FLOATING_SHADE_STRONG { get; }
            public virtual RGBColor COLOR_FLOATING_SHADE_STRONG_INVERT { get; }
        // Shades -------------------------------------------------------------------------------------------------------------------------
            public virtual string BOX_SHADOW_FLOATING_LEVEL_1 { get; }
            public virtual string BOX_SHADOW_FLOATING_LEVEL_2 { get; }
    // Selection surface ------------------------------------------------------------------------------------------------------------------
    public virtual RGBColor COLOR_SELECTION { get; }
    public virtual RGBColor COLOR_SELECTION_BACKGROUND { get; }

    // Fonts ------------------------------------------------------------------------------------------------------------------------------
    public string FONT_PRIMARY { get; } = "Montserrat, sans-serif";

    // Sizes (no unit == px) --------------------------------------------------------------------------------------------------------------
    public int SIZE_CONTAINER_MAX_WIDTH { get; } = 1340;
    public int SIZE_CONTAINER_PADDING_MOBILE { get; } = 16;
    public int SIZE_CONTAINER_PADDING_TABLET { get; } = 28;
    public int SIZE_CONTAINER_PADDING_DESKTOP { get; } = 40;
    public int SIZE_HEADER_HEIGHT { get; } = 75;
    public int SIZE_FOOTER_HEIGHT_MOBILE { get; } = 170;
    public int SIZE_FOOTER_HEIGHT_TABLET { get; } = 190;
    public int SIZE_FOOTER_HEIGHT_DESKTOP { get; } = 210;

    // Transitions (ms) -------------------------------------------------------------------------------------------------------------------
    public int TRANSITION_BOLT { get; } = 0;
    public int TRANSITION_SEMI_BOLT { get; } = 50;
    public int TRANSITION_ULTRA_FAST { get; } = 100;
    public int TRANSITION_SEMI_ULTRA_FAST { get; } = 150;
    public int TRANSITION_FAST { get; } = 200;
    public int TRANSITION_SEMI_FAST { get; } = 250;
    public int TRANSITION_NORMAL { get; } = 300;
    public int TRANSITION_SEMI_SLOW { get; } = 350;
    public int TRANSITION_SLOW { get; } = 400;
    public int TRANSITION_SEMI_EXTRA_SLOW { get; } = 450;
    public int TRANSITION_EXTRA_SLOW { get; } = 500;

    // Z-index ----------------------------------------------------------------------------------------------------------------------------
    public int Z_INDEX_FORM_ERROR { get; } = 100;
    public int Z_INDEX_DROPDOWN { get; } = 1000;
    public int Z_INDEX_MENU { get; } = 1001;
    public int Z_INDEX_MODAL { get; } = 1002;
    public int Z_INDEX_PAGE_LOADER { get; } = 1000000;
    public int Z_INDEX_SERVER_PAGE_LOADER { get; } = 1000001;
    public int Z_INDEX_NOTIFICATION { get; } = 1000002;
    public int Z_INDEX_CONSOLE_UI { get; } = 1000003;

    // Breakpoints (px) -------------------------------------------------------------------------------------------------------------------
    public double BREAKPOINT_MOBILE_SM { get; } = 480;
    public double BREAKPOINT_MOBILE { get; } = 768;
    public double BREAKPOINT_TABLET_SM { get; } = 992;
    public double BREAKPOINT_TABLET { get; } = 1200;
}

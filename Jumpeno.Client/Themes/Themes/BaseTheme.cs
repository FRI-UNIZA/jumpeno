namespace Jumpeno.Client.Constants;

#pragma warning disable CS8618
#pragma warning disable CA1822

public class BaseTheme {
    // Fonts ------------------------------------------------------------------------------------------------------------------------------
    public string FONT_PRIMARY => "Montserrat, sans-serif";

    // Scrollbars -------------------------------------------------------------------------------------------------------------------------
    public virtual SCROLLAREA_THEME SCROLL_THEME { get; }
    public virtual SCROLLAREA_THEME SCROLL_THEME_MENU { get; }
    
    // Selection --------------------------------------------------------------------------------------------------------------------------
    public virtual RGBColor COLOR_SELECTION { get; }
    public virtual RGBColor COLOR_SELECTION_BACKGROUND { get; }

    // Base surface -----------------------------------------------------------------------------------------------------------------------
    public virtual RGBColor COLOR_BASE { get; }
    public virtual RGBColor COLOR_BASE_HIGHLIGHT { get; }
    public virtual RGBColor COLOR_BASE_INVERT { get; }
    public virtual RGBColor COLOR_BASE_INVERT_HIGHLIGHT { get; }
    public virtual RGBColor COLOR_BASE_ERROR { get; }
    public virtual RGBColor COLOR_BASE_SUCCESS { get; }
    // Danger surface ---------------------------------------------------------------------------------------------------------------------
    public virtual RGBColor COLOR_DANGER { get; }
    public virtual RGBColor COLOR_DANGER_HIGHLIGHT { get; }
    public virtual RGBColor COLOR_DANGER_SURFACE { get; }
    public virtual RGBColor COLOR_DANGER_SURFACE_HIGHLIGHT { get; }
    public virtual RGBColor COLOR_DANGER_ACCENT { get; }
    public virtual RGBColor COLOR_DANGER_ACCENT_HIGHLIGHT { get; }
    // Success surface ---------------------------------------------------------------------------------------------------------------------
    public virtual RGBColor COLOR_SUCCESS { get; }
    public virtual RGBColor COLOR_SUCCESS_HIGHLIGHT { get; }
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

    // PageLoader -------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor COLOR_SERVER_PAGE_LOADER_TITLE { get; }

    // NavMenu ----------------------------------------------------------------------------------------------------------------------------
    public virtual string SHADOW_NAV_MENU { get; }

    // Modal ------------------------------------------------------------------------------------------------------------------------------
    public virtual string SHADOW_MODAL_DIALOG { get; }
    public virtual string SHADOW_MODAL_ENDING { get; }

    // Select culture ---------------------------------------------------------------------------------------------------------------------
    public virtual string BORDER_SELECT_CULTURE { get; }
    public virtual string BORDER_SELECT_CULTURE_TRANSITION { get; }
    public virtual string SHADOW_SELECT_CULTURE { get; }
    public virtual string SHADOW_SELECT_CULTURE_TRANSITION { get; }

    // Profile ----------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor COLOR_PROFILE_BACKGROUND { get; }
    // Dropdown ---------------------------------------------------------------------------------------------------------------------------
    public virtual string SHADOW_DROPDOWN_OPTIONS { get; }
    // Admin dropdown ---------------------------------------------------------------------------------------------------------------------
    public virtual string SHADOW_ADMIN_DROPDOWN_BUTTON { get; }
    public virtual string SHADOW_ADMIN_DROPDOWN_BUTTON_HIGHLIGHT { get; }
    // User dropdown ----------------------------------------------------------------------------------------------------------------------
    public virtual string SHADOW_USER_DROPDOWN_BUTTON { get; }
    public virtual string SHADOW_USER_DROPDOWN_BUTTON_HIGHLIGHT { get; }

    // Box --------------------------------------------------------------------------------------------------------------------------------
    public virtual string SHADOW_BOX_DEFAULT { get; }

    // Manual -----------------------------------------------------------------------------------------------------------------------------
    public virtual string SHADOW_MANUAL { get; }

    // Button primary ---------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor COLOR_BUTTON_PRIMARY { get; }
    public virtual RGBAColor COLOR_BUTTON_PRIMARY_SURFACE { get; }
    public virtual RGBAColor COLOR_BUTTON_PRIMARY_SURFACE_HIGHLIGHT { get; }
    public virtual string SHADOW_BUTTON_PRIMARY { get; }
    public virtual string SHADOW_BUTTON_PRIMARY_TRANSITION { get; }
    // Button secondary ---------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor COLOR_BUTTON_SECONDARY { get; }
    public virtual RGBAColor COLOR_BUTTON_SECONDARY_SURFACE { get; }
    public virtual RGBAColor COLOR_BUTTON_SECONDARY_SURFACE_HIGHLIGHT { get; }
    public virtual string SHADOW_BUTTON_SECONDARY { get; }
    public virtual string SHADOW_BUTTON_SECONDARY_TRANSITION { get; }
    // Button tertiary --------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor COLOR_BUTTON_TERTIARY { get; }
    public virtual RGBAColor COLOR_BUTTON_TERTIARY_SURFACE { get; }
    public virtual RGBAColor COLOR_BUTTON_TERTIARY_SURFACE_HIGHLIGHT { get; }
    public virtual string SHADOW_BUTTON_TERTIARY { get; }
    public virtual string SHADOW_BUTTON_TERTIARY_TRANSITION { get; }
    // Button quaternary ------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor COLOR_BUTTON_QUATERNARY { get; }
    public virtual RGBAColor COLOR_BUTTON_QUATERNARY_SURFACE { get; }
    public virtual RGBAColor COLOR_BUTTON_QUATERNARY_SURFACE_HIGHLIGHT { get; }
    public virtual string SHADOW_BUTTON_QUATERNARY { get; }
    public virtual string SHADOW_BUTTON_QUATERNARY_TRANSITION { get; }

    // Sizes (no unit == px) --------------------------------------------------------------------------------------------------------------
    public int SIZE_CONTAINER_MAX_WIDTH => 1340;
    public int SIZE_CONTAINER_PADDING_MOBILE => 16;
    public int SIZE_CONTAINER_PADDING_TABLET => 28;
    public int SIZE_CONTAINER_PADDING_DESKTOP => 40;
    public int SIZE_HEADER_HEIGHT => 75;
    public int SIZE_FOOTER_HEIGHT_MOBILE => 170;
    public int SIZE_FOOTER_HEIGHT_TABLET => 190;
    public int SIZE_FOOTER_HEIGHT_DESKTOP => 210;

    // Transitions (ms) -------------------------------------------------------------------------------------------------------------------
    public int TRANSITION_BOLT => 0;
    public int TRANSITION_SEMI_BOLT => 50;
    public int TRANSITION_ULTRA_FAST => 100;
    public int TRANSITION_SEMI_ULTRA_FAST => 150;
    public int TRANSITION_FAST => 200;
    public int TRANSITION_SEMI_FAST => 250;
    public int TRANSITION_NORMAL => 300;
    public int TRANSITION_SEMI_SLOW => 350;
    public int TRANSITION_SLOW => 400;
    public int TRANSITION_SEMI_EXTRA_SLOW => 450;
    public int TRANSITION_EXTRA_SLOW => 500;

    // Z-index ----------------------------------------------------------------------------------------------------------------------------
    public int Z_INDEX_FORM_ERROR => 100;
    public int Z_INDEX_DROPDOWN => 1000;
    public int Z_INDEX_MENU => 1001;
    public int Z_INDEX_MODAL => 1002;
    public int Z_INDEX_PAGE_LOADER => 1000000;
    public int Z_INDEX_SERVER_PAGE_LOADER => 1000001;
    public int Z_INDEX_NOTIFICATION => 1000002;
    public int Z_INDEX_CONSOLE_UI => 1000003;

    // Breakpoints (px) -------------------------------------------------------------------------------------------------------------------
    public double BREAKPOINT_MOBILE_SM => 480;
    public double BREAKPOINT_MOBILE => 768;
    public double BREAKPOINT_TABLET_SM => 992;
    public double BREAKPOINT_TABLET => 1200;
}

namespace Jumpeno.Client.Constants;

public class DarkTheme : BaseTheme {
    // Scrollbars -------------------------------------------------------------------------------------------------------------------------
    public override SCROLLAREA_THEME SCROLL_THEME => SCROLLAREA_THEME.OS_THEME_DARK;
    public override SCROLLAREA_THEME SCROLL_THEME_MENU => SCROLLAREA_THEME.OS_THEME_LIGHT;

    // Selection --------------------------------------------------------------------------------------------------------------------------
    public override RGBColor COLOR_SELECTION => new(42, 33, 56);
    public override RGBColor COLOR_SELECTION_BACKGROUND => new(255, 239, 0);

    // Base surface -----------------------------------------------------------------------------------------------------------------------
    public override RGBColor COLOR_BASE => new(0, 0, 0);
    public override RGBColor COLOR_BASE_HIGHLIGHT => new(40, 40, 40);
    public override RGBColor COLOR_BASE_INVERT => new(255, 255, 255);
    public override RGBColor COLOR_BASE_INVERT_HIGHLIGHT => new(220, 220, 220);
    public override RGBColor COLOR_BASE_ERROR => new(255, 0, 0);
    public override RGBColor COLOR_BASE_SUCCESS => new(0, 255, 0);
    // Danger surface ---------------------------------------------------------------------------------------------------------------------
    public override RGBColor COLOR_DANGER => new(83, 0, 0);
    public override RGBColor COLOR_DANGER_HIGHLIGHT => new(143, 0, 0);
    public override RGBColor COLOR_DANGER_SURFACE => new(255, 186, 186);
    public override RGBColor COLOR_DANGER_SURFACE_HIGHLIGHT => new(255, 219, 219);
    public override RGBColor COLOR_DANGER_ACCENT => new(255, 0, 0);
    public override RGBColor COLOR_DANGER_ACCENT_HIGHLIGHT => new(253, 118, 118);
    // Success surface --------------------------------------------------------------------------------------------------------------------
    public override RGBColor COLOR_SUCCESS => new(82, 196, 26);
    public override RGBColor COLOR_SUCCESS_HIGHLIGHT => new(0, 255, 0);
    // Primary surface --------------------------------------------------------------------------------------------------------------------
    public override RGBColor COLOR_PRIMARY => new(42, 33, 56);
    public override RGBColor COLOR_PRIMARY_HIGHLIGHT => new(122, 113, 136);
    public override RGBColor COLOR_PRIMARY_ACCENT => new(42, 33, 56);
    public override RGBColor COLOR_PRIMARY_ACCENT_HIGHLIGHT => new(122, 113, 136);
    public override RGBColor COLOR_PRIMARY_SURFACE => new(255, 255, 83);
    public override RGBColor COLOR_PRIMARY_SURFACE_HIGHLIGHT => new(225, 225, 53);
    public override RGBColor COLOR_PRIMARY_BOX => new(250, 250, 175);
    public override RGBColor COLOR_PRIMARY_SHADE => new(220, 220, 220);
    public override RGBColor COLOR_PRIMARY_SHADE_INVERT => new(60, 60, 60);
    public override RGBColor COLOR_PRIMARY_SHADE_STRONG => new(190, 190, 190);
    public override RGBColor COLOR_PRIMARY_SHADE_STRONG_INVERT => new(90, 90, 90);
    // Secondary surface ------------------------------------------------------------------------------------------------------------------
    public override RGBColor COLOR_SECONDARY => new(236, 240, 241);
    public override RGBColor COLOR_SECONDARY_HIGHLIGHT => new(255, 255, 255);
    public override RGBColor COLOR_SECONDARY_ACCENT => new(255, 215, 0);
    public override RGBColor COLOR_SECONDARY_ACCENT_HIGHLIGHT => new(255, 239, 0);
    public override RGBColor COLOR_SECONDARY_SURFACE => new(42, 33, 56);
    public override RGBColor COLOR_SECONDARY_SURFACE_HIGHLIGHT => new(72, 63, 86);
    public override RGBColor COLOR_SECONDARY_BOX => new(57, 48, 71);
    public override RGBColor COLOR_SECONDARY_SHADE => new(170, 170, 170);
    public override RGBColor COLOR_SECONDARY_SHADE_INVERT => new(236, 240, 241);
    public override RGBColor COLOR_SECONDARY_SHADE_STRONG => new(140, 140, 140);
    public override RGBColor COLOR_SECONDARY_SHADE_STRONG_INVERT => new(206, 210, 211);
    // Floating surface -------------------------------------------------------------------------------------------------------------------
    public override RGBColor COLOR_FLOATING => new(42, 33, 56);
    public override RGBColor COLOR_FLOATING_HIGHLIGHT => new(122, 113, 136);
    public override RGBColor COLOR_FLOATING_ACCENT => new(42, 33, 56);
    public override RGBColor COLOR_FLOATING_ACCENT_HIGHLIGHT => new(122, 113, 136);
    public override RGBColor COLOR_FLOATING_SURFACE => new(255, 255, 255);
    public override RGBColor COLOR_FLOATING_SURFACE_HIGHLIGHT => new(225, 225, 225);
    public override RGBColor COLOR_FLOATING_BOX => new(245, 245, 245);
    public override RGBColor COLOR_FLOATING_SHADE => new(220, 220, 220);
    public override RGBColor COLOR_FLOATING_SHADE_INVERT => new(60, 60, 60);
    public override RGBColor COLOR_FLOATING_SHADE_STRONG => new(190, 190, 190);
    public override RGBColor COLOR_FLOATING_SHADE_STRONG_INVERT => new(90, 90, 90);

    // PageLoader -------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor COLOR_SERVER_PAGE_LOADER_TITLE => new(236, 240, 241);

    // NavMenu ----------------------------------------------------------------------------------------------------------------------------
    public override string SHADOW_NAV_MENU => "0 0 20px rgba(0, 0, 0, 0.8)";

    // Modal ------------------------------------------------------------------------------------------------------------------------------
    public override string SHADOW_MODAL_DIALOG => "0 0 30px 0 rgba(0, 0, 0, 0.5)";
    public override string SHADOW_MODAL_ENDING => "0 0 10px rgba(0, 0, 0, 0.1)";

    // Select culture ---------------------------------------------------------------------------------------------------------------------
    public override string BORDER_SELECT_CULTURE => $"1px solid transparent";
    public override string BORDER_SELECT_CULTURE_TRANSITION => $"1px solid transparent";
    public override string SHADOW_SELECT_CULTURE => $"0 2px 6px rgba({COLOR_BASE}, 0.4)";
    public override string SHADOW_SELECT_CULTURE_TRANSITION => $"0 2px 10px rgba({COLOR_BASE}, 0.8)";

    // Profile ----------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor COLOR_PROFILE_BACKGROUND => new(42, 33, 56);
    // Dropdown ---------------------------------------------------------------------------------------------------------------------------
    public override string SHADOW_DROPDOWN_OPTIONS => "0 0 16px rgba(0, 0, 0, 0.3)";
    // Admin dropdown ---------------------------------------------------------------------------------------------------------------------
    public override string SHADOW_ADMIN_DROPDOWN_BUTTON => "0 0 2px 2px rgba(42, 33, 56, 1)";
    public override string SHADOW_ADMIN_DROPDOWN_BUTTON_HIGHLIGHT => "0 0 2px 2px rgba(255, 255, 255, 0.8)";
    // User dropdown ----------------------------------------------------------------------------------------------------------------------
    public override string SHADOW_USER_DROPDOWN_BUTTON => "0 0 2px 2px rgba(42, 33, 56, 1)";
    public override string SHADOW_USER_DROPDOWN_BUTTON_HIGHLIGHT => "0 0 2px 2px rgba(255, 255, 255, 0.8)";

    // Box --------------------------------------------------------------------------------------------------------------------------------
    public override string SHADOW_BOX_DEFAULT => "0 6px 16px 0 rgba(0, 0, 0, 0.2)";

    // Manual -----------------------------------------------------------------------------------------------------------------------------
    public override string SHADOW_MANUAL => "0 12px 26px 5px rgba(0, 0, 0, 0.4)";

    // Button primary ---------------------------------------------------------------------------------------------------------------------
    public override RGBAColor COLOR_BUTTON_PRIMARY => new(236, 240, 241);
    public override RGBAColor COLOR_BUTTON_PRIMARY_SURFACE => new(42, 33, 56);
    public override RGBAColor COLOR_BUTTON_PRIMARY_SURFACE_HIGHLIGHT => new(72, 63, 86);
    public override string SHADOW_BUTTON_PRIMARY => "0px 2px 4px rgba(0, 0, 0, 0.55)";
    public override string SHADOW_BUTTON_PRIMARY_TRANSITION => "0px 2px 4px rgba(0, 0, 0, 0.5)";
    // Button secondary ---------------------------------------------------------------------------------------------------------------------
    public override RGBAColor COLOR_BUTTON_SECONDARY => new(42, 33, 56);
    public override RGBAColor COLOR_BUTTON_SECONDARY_SURFACE => new(255, 215, 0);
    public override RGBAColor COLOR_BUTTON_SECONDARY_SURFACE_HIGHLIGHT => new(255, 239, 0);
    public override string SHADOW_BUTTON_SECONDARY => "0px 2px 4px rgba(0, 0, 0, 0.35)";
    public override string SHADOW_BUTTON_SECONDARY_TRANSITION => "0px 2px 4px rgba(0, 0, 0, 0.3)";
    // Button tertiary --------------------------------------------------------------------------------------------------------------------
    public override RGBAColor COLOR_BUTTON_TERTIARY => new(42, 33, 56);
    public override RGBAColor COLOR_BUTTON_TERTIARY_SURFACE => new(230, 230, 230);
    public override RGBAColor COLOR_BUTTON_TERTIARY_SURFACE_HIGHLIGHT => new(236, 236, 236);
    public override string SHADOW_BUTTON_TERTIARY => SHADOW_BUTTON_SECONDARY;
    public override string SHADOW_BUTTON_TERTIARY_TRANSITION => SHADOW_BUTTON_SECONDARY_TRANSITION;
    // Button quaternary ------------------------------------------------------------------------------------------------------------------
    public override RGBAColor COLOR_BUTTON_QUATERNARY => new(236, 240, 241);
    public override RGBAColor COLOR_BUTTON_QUATERNARY_SURFACE => new(42, 33, 56);
    public override RGBAColor COLOR_BUTTON_QUATERNARY_SURFACE_HIGHLIGHT => new(72, 63, 86);
    public override string SHADOW_BUTTON_QUATERNARY => SHADOW_BUTTON_PRIMARY;
    public override string SHADOW_BUTTON_QUATERNARY_TRANSITION => SHADOW_BUTTON_PRIMARY_TRANSITION;
}

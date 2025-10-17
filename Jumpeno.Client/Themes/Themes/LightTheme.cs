namespace Jumpeno.Client.Constants;

public class LightTheme : BaseTheme {
// NOTE: Surface --------------------------------------------------------------------------------------------------------------------------
    // Primary ----------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor SURFACE_BACKGROUND__SURFACE_PRIMARY => new(255, 255, 83);
    public override RGBAColor SURFACE_BACKGROUND__SURFACE_PRIMARY_COLLAPSE => new(255, 255, 255);
    public override RGBAColor SURFACE_BACKGROUND__SURFACE_PRIMARY_BOX => new(250, 250, 175);
    public override RGBAColor SURFACE_BACKGROUND__SURFACE_PRIMARY_BOX_COLLAPSE => new(255, 255, 255);
    public override RGBAColor SURFACE_BACKGROUND__SURFACE_PRIMARY_GLASS => new(SURFACE_BACKGROUND__SURFACE_PRIMARY_BOX, 0.6f);
    public override RGBAColor SURFACE_BACKGROUND__SURFACE_PRIMARY_GLASS_COLLAPSE => new(255, 255, 255);

    // Secondary --------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor SURFACE_BACKGROUND__SURFACE_SECONDARY => new(255, 255, 255);

    // Floating ---------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor SURFACE_BACKGROUND__SURFACE_FLOATING => new(255, 255, 255);
    public override RGBAColor SURFACE_BACKGROUND__SURFACE_FLOATING_COLLAPSE => new(241, 241, 241);
    public override RGBAColor SURFACE_BACKGROUND__SURFACE_FLOATING_ADDITIONAL => new(220, 220, 220);
    public override RGBAColor SURFACE_BACKGROUND__SURFACE_FLOATING_ADDITIONAL_COLLAPSE => new(241, 241, 241);

// NOTE: Status ---------------------------------------------------------------------------------------------------------------------------
    // Danger -----------------------------------------------------------------------------------------------------------------------------
    // Box:
    public override RGBAColor STATUS_DANGER_BOX_COLOR => new(83, 0, 0);
    public override RGBAColor STATUS_DANGER_BOX_BACKGROUND => new(255, 186, 186);
    public override RGBAColor STATUS_DANGER_BOX_OUTLINE_COLOR => new(255, 0, 0);
    // Box [highlight]:
    public override RGBAColor STATUS_DANGER_BOX_HIGHLIGHT_COLOR => new(143, 0, 0);
    public override RGBAColor STATUS_DANGER_BOX_HIGHLIGHT_BACKGROUND => new(255, 219, 219);
    public override RGBAColor STATUS_DANGER_BOX_HIGHLIGHT_OUTLINE_COLOR => new(253, 118, 118);

    // Accent:
    public override RGBAColor STATUS_DANGER_ACCENT_COLOR => new(255, 77, 79);
    // Accent [highlight]:
    public override RGBAColor STATUS_DANGER_ACCENT_HIGHLIGHT_COLOR => new(255, 0, 4);

    // Neon:
    public override RGBAColor STATUS_DANGER_NEON_COLOR => new(255, 0, 0);

    // Success ----------------------------------------------------------------------------------------------------------------------------
    // Box:
    public override RGBAColor STATUS_SUCCESS_BOX_COLOR => new(21, 87, 36);
    public override RGBAColor STATUS_SUCCESS_BOX_BACKGROUND => new(212, 237, 218);
    public override RGBAColor STATUS_SUCCESS_BOX_OUTLINE_COLOR => new(82, 196, 26);
    // Box [highlight]:
    public override RGBAColor STATUS_SUCCESS_BOX_HIGHLIGHT_COLOR => new(41, 107, 56);
    public override RGBAColor STATUS_SUCCESS_BOX_HIGHLIGHT_BACKGROUND => new(233, 242, 235);
    public override RGBAColor STATUS_SUCCESS_BOX_HIGHLIGHT_OUTLINE_COLOR => new(102, 216, 46);

    // Accent:
    public override RGBAColor STATUS_SUCCESS_ACCENT_COLOR => new(82, 196, 26);
    // Accent [highlight]:
    public override RGBAColor STATUS_SUCCESS_ACCENT_HIGHLIGHT_COLOR => new(72, 217, 0);

    // Neon:
    public override RGBAColor STATUS_SUCCESS_NEON_COLOR => new(0, 255, 0);

    // Warning ----------------------------------------------------------------------------------------------------------------------------
    // Box:
    public override RGBAColor STATUS_WARNING_BOX_COLOR => new(135, 92, 5);
    public override RGBAColor STATUS_WARNING_BOX_BACKGROUND => new(247, 225, 183);
    public override RGBAColor STATUS_WARNING_BOX_OUTLINE_COLOR => new(250, 173, 20);
    // Box [highlight]:
    public override RGBAColor STATUS_WARNING_BOX_HIGHLIGHT_COLOR => new(145, 107, 31);
    public override RGBAColor STATUS_WARNING_BOX_HIGHLIGHT_BACKGROUND => new(252, 239, 213);
    public override RGBAColor STATUS_WARNING_BOX_HIGHLIGHT_OUTLINE_COLOR => new(255, 191, 0);

    // Accent:
    public override RGBAColor STATUS_WARNING_ACCENT_COLOR => new(250, 173, 20);
    // Accent [highlight]:
    public override RGBAColor STATUS_WARNING_ACCENT_HIGHLIGHT_COLOR => new(255, 191, 0);

    // Neon:
    public override RGBAColor STATUS_WARNING_NEON_COLOR => new(255, 255, 0);

    // Info -------------------------------------------------------------------------------------------------------------------------------
    // Box:
    public override RGBAColor STATUS_INFO_BOX_COLOR => new(12, 63, 110);
    public override RGBAColor STATUS_INFO_BOX_BACKGROUND => new(191, 224, 255);
    public override RGBAColor STATUS_INFO_BOX_OUTLINE_COLOR => new(24, 144, 255);
    // Box [highlight]:
    public override RGBAColor STATUS_INFO_BOX_HIGHLIGHT_COLOR => new(15, 77, 135);
    public override RGBAColor STATUS_INFO_BOX_HIGHLIGHT_BACKGROUND => new(217, 234, 250);
    public override RGBAColor STATUS_INFO_BOX_HIGHLIGHT_OUTLINE_COLOR => new(24, 170, 255);

    // Accent:
    public override RGBAColor STATUS_INFO_ACCENT_COLOR => new(24, 144, 255);
    // Accent [highlight]:
    public override RGBAColor STATUS_INFO_ACCENT_HIGHLIGHT_COLOR => new(24, 170, 255);

    // Neon:
    public override RGBAColor STATUS_INFO_NEON_COLOR => new(0, 251, 255);

// NOTE: Layout ---------------------------------------------------------------------------------------------------------------------------
    // Body -------------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor BODY_BACKGROUND => SURFACE_BACKGROUND__SURFACE_SECONDARY;
    // Scrollbars:
    public override SCROLLAREA_THEME BODY_SCROLL_THEME => SCROLLAREA_THEME.OS_THEME_DARK;
    // Selection:
    public override RGBAColor BODY_SELECTION_COLOR => new(42, 33, 56);
    public override RGBAColor BODY_SELECTION_BACKGROUND => new(255, 239, 0);
    // Backdrop:
    public override RGBAColor BODY_BACKDROP => new(0, 0, 0, 0.5f);

    // QR code ----------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor QR_CODE_BACKGROUND => new(255, 255, 255);

    // NavMenu ----------------------------------------------------------------------------------------------------------------------------
    public override string NAV_MENU_BOX_SHADOW => "0 0 20em rgba(0, 0, 0, 0.3)";

    // NavMenu [mobile]:
    public override RGBAColor NAV_MENU_MOBILE_BUTTON_COLOR => new(82, 69, 103);
    public override SCROLLAREA_THEME NAV_MENU_MOBILE_SCROLL_THEME => SCROLLAREA_THEME.OS_THEME_DARK;

    // NavMenu [mobile][highlight]:
    public override RGBAColor NAV_MENU_MOBILE_BUTTON_HIGHLIGHT_COLOR => new(132, 124, 145);

    // NavMenu [mobile][focus]:
    public override RGBAColor NAV_MENU_MOBILE_BUTTON_FOCUS_BACKGROUND => new(42, 33, 56, 0.08f);
    public override string NAV_MENU_MOBILE_BUTTON_FOCUS_BOX_SHADOW => "0 1em 6em rgba(0, 0, 0, 0.4)";

// NOTE: Pages ----------------------------------------------------------------------------------------------------------------------------
    // Game -------------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor GAME_STATE_COLOR => new(255, 255, 255);
    public override string GAME_STATE_TEXT_SHADOW => "0.09em 0.045em 0.24em rgba(0, 0, 0, 0.8)";

    // GameScreen:
    public override RGBAColor GAME_SCREEN_CANVAS_BACKGROUND => new(0, 0, 0);
    public override string GAME_SCREEN_CANVAS_BOX_SHADOW => "0 0.0025em 0.025em 0.01em rgba(0, 0, 0, 0.8)";

    // GameScreen > Control:
    public override RGBAColor GAME_SCREEN_CONTROL_COLOR => new(105, 99, 115);
    public override RGBAColor GAME_SCREEN_CONTROL_BACKGROUND => new(255, 255, 255);
    public override string GAME_SCREEN_CONTROL_BOX_SHADOW => "0 0.04em 0.16em 0.02em rgba(0, 0, 0, 0.2)";
    
    // GameScreen > Control [pressed]:
    public override RGBAColor GAME_SCREEN_CONTROL_PRESSED_COLOR => new(105, 99, 115);
    public override RGBAColor GAME_SCREEN_CONTROL_PRESSED_BACKGROUND => new(255, 255, 255, 0.7f);
    public override string GAME_SCREEN_CONTROL_PRESSED_BOX_SHADOW => "0 0.04em 0.16em 0.02em rgba(0, 0, 0, 0.14)";

    // Lobby:
    public override string LOBBY_BOX_SHADOW => "0 0.006em 0.018em 0 rgba(0, 0, 0, 0.2)";
    public override RGBAColor LOBBY_EMPTY_COLOR => new(0, 0, 0, 0.4f);
    public override RGBAColor LOBBY_LINE_BACKGROUND => new(255, 215, 0, 0.7f);
    public override string LOBBY_PRESENCE_BOX_SHADOW => "0.02em 0.02em 0.05em rgba(0, 0, 0, 0.7)";
    public override RGBAColor LOBBY_DASH_COLOR => new(0, 0, 0, 0.14f);
    
    // Manual -----------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor MANUAL_COLOR => new(255, 255, 255);
    public override string MANUAL_TEXT_SHADOW => "0.5em 0.1em 0.53em rgba(0, 0, 0, 0.4)";
    public override RGBAColor MANUAL_BACKGROUND => new(42, 144, 244);
    public override RGBAColor MANUAL_BACKGROUND_TRANSITION => new(73, 175, 255);
    public override string MANUAL_BOX_SHADOW => "0 12em 20em 3em rgba(0, 0, 0, 0.25)";

// NOTE: Box ------------------------------------------------------------------------------------------------------------------------------
    // Box [box] --------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor BOX_BACKGROUND__SURFACE_PRIMARY_BOX => SURFACE_BACKGROUND__SURFACE_PRIMARY_BOX;
    public override RGBAColor BOX_BACKGROUND__SURFACE_PRIMARY_GLASS => SURFACE_BACKGROUND__SURFACE_PRIMARY_GLASS;
    public override string BOX_BOX_SHADOW__SURFACE_PRIMARY_BOX => "0 6em 16em 0 rgba(0, 0, 0, 0.15)";
    public override string BOX_BOX_SHADOW__SURFACE_PRIMARY_GLASS => "0 6em 16em 0 rgba(0, 0, 0, 0.15)";

// NOTE: Buttons --------------------------------------------------------------------------------------------------------------------------
    // Button [primary] -------------------------------------------------------------------------------------------------------------------
    public override RGBAColor BUTTON_PRIMARY_COLOR => new(42, 33, 56);
    public override RGBAColor BUTTON_PRIMARY_BACKGROUND => new(255, 255, 255);
    public override string BUTTON_PRIMARY_BOX_SHADOW => "0 2em 4em rgba(0, 0, 0, 0.35)";

    // Button [primary][highlight]:
    public override RGBAColor BUTTON_PRIMARY_HIGHLIGHT_COLOR => new(42, 33, 56);
    public override RGBAColor BUTTON_PRIMARY_HIGHLIGHT_BACKGROUND => new(235, 235, 235);
    public override string BUTTON_PRIMARY_HIGHLIGHT_BOX_SHADOW => "0 2em 4em rgba(0, 0, 0, 0.3)";

    // Button [secondary] -----------------------------------------------------------------------------------------------------------------
    public override RGBAColor BUTTON_SECONDARY_COLOR => new(42, 33, 56);
    public override RGBAColor BUTTON_SECONDARY_BACKGROUND => new(255, 215, 0);
    public override string BUTTON_SECONDARY_BOX_SHADOW => "0 2em 4em rgba(0, 0, 0, 0.35)";

    // Button [secondary][highlight]:
    public override RGBAColor BUTTON_SECONDARY_HIGHLIGHT_COLOR => new(42, 33, 56);
    public override RGBAColor BUTTON_SECONDARY_HIGHLIGHT_BACKGROUND => new(255, 239, 0);
    public override string BUTTON_SECONDARY_HIGHLIGHT_BOX_SHADOW => "0 2em 4em rgba(0, 0, 0, 0.3)";

    // Button [tertiary] ------------------------------------------------------------------------------------------------------------------
    public override RGBAColor BUTTON_TERTIARY_COLOR => new(42, 33, 56);
    public override RGBAColor BUTTON_TERTIARY_BACKGROUND => new(230, 230, 230);
    public override string BUTTON_TERTIARY_BOX_SHADOW => BUTTON_SECONDARY_BOX_SHADOW;

    // Button [tertiary][highlight]:
    public override RGBAColor BUTTON_TERTIARY_HIGHLIGHT_COLOR => new(42, 33, 56);
    public override RGBAColor BUTTON_TERTIARY_HIGHLIGHT_BACKGROUND => new(236, 236, 236);
    public override string BUTTON_TERTIARY_HIGHLIGHT_BOX_SHADOW => BUTTON_SECONDARY_HIGHLIGHT_BOX_SHADOW;

    // Button [quaternary] ----------------------------------------------------------------------------------------------------------------
    public override RGBAColor BUTTON_QUATERNARY_COLOR => new(236, 240, 241);
    public override RGBAColor BUTTON_QUATERNARY_BACKGROUND => new(42, 33, 56);
    public override string BUTTON_QUATERNARY_BOX_SHADOW => BUTTON_PRIMARY_BOX_SHADOW;

    // Button [quaternary][highlight]:
    public override RGBAColor BUTTON_QUATERNARY_HIGHLIGHT_COLOR => new(236, 240, 241);
    public override RGBAColor BUTTON_QUATERNARY_HIGHLIGHT_BACKGROUND => new(72, 63, 86);
    public override string BUTTON_QUATERNARY_HIGHLIGHT_BOX_SHADOW => BUTTON_PRIMARY_HIGHLIGHT_BOX_SHADOW;

    // Button [danger] --------------------------------------------------------------------------------------------------------------------
    public override RGBAColor BUTTON_DANGER_COLOR => new(255, 255, 255);
    public override RGBAColor BUTTON_DANGER_BACKGROUND => new(217, 26, 29);
    public override string BUTTON_DANGER_BOX_SHADOW => BUTTON_PRIMARY_BOX_SHADOW;

    // Button [danger][highlight]:
    public override RGBAColor BUTTON_DANGER_HIGHLIGHT_COLOR => new(255, 255, 255);
    public override RGBAColor BUTTON_DANGER_HIGHLIGHT_BACKGROUND => new(237, 43, 46);
    public override string BUTTON_DANGER_HIGHLIGHT_BOX_SHADOW => BUTTON_PRIMARY_HIGHLIGHT_BOX_SHADOW;

    // Button [disabled] ------------------------------------------------------------------------------------------------------------------
    public override RGBAColor BUTTON_DISABLED_COLOR => new(190, 190, 190);
    public override RGBAColor BUTTON_DISABLED_BACKGROUND => new(240, 240, 240);
    public override string BUTTON_DISABLED_BOX_SHADOW => "0 2em 2em rgba(0, 0, 0, 0.2)";

    // MenuButton -------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor MENU_BUTTON_COLOR => new(42, 33, 56);
    public override RGBAColor MENU_BUTTON_BACKGROUND => new(0, 0, 0, 0.0f);
    public override string MENU_BUTTON_BOX_SHADOW => "none";

    // MenuButton [highlight]:
    public override RGBAColor MENU_BUTTON_HIGHLIGHT_COLOR => new(42, 33, 56);
    public override RGBAColor MENU_BUTTON_HIGHLIGHT_BACKGROUND => new(42, 33, 56, 0.055f);
    public override string MENU_BUTTON_HIGHLIGHT_BOX_SHADOW => "0em 2em 10em rgba(0, 0, 0, 0.02)";

    // MenuButton [active]:
    public override RGBAColor MENU_BUTTON_ACTIVE_COLOR => new(42, 33, 56);
    public override RGBAColor MENU_BUTTON_ACTIVE_BACKGROUND => new(0, 0, 0, 0.0f);
    public override string MENU_BUTTON_ACTIVE_BOX_SHADOW => "0em 2em 7em rgba(0, 0, 0, 0.35)";

    // MenuButton [active][highlight]:
    public override RGBAColor MENU_BUTTON_ACTIVE_HIGHLIGHT_COLOR => new(42, 33, 56);
    public override RGBAColor MENU_BUTTON_ACTIVE_HIGHLIGHT_BACKGROUND => new(0, 0, 0, 0.0f);
    public override string MENU_BUTTON_ACTIVE_HIGHLIGHT_BOX_SHADOW => "0em 2em 9em rgba(0, 0, 0, 0.5)";

    // MenuButton [mobile] ----------------------------------------------------------------------------------------------------------------
    public override RGBAColor MENU_BUTTON_MOBILE_COLOR => new(42, 33, 56);
    public override RGBAColor MENU_BUTTON_MOBILE_BACKGROUND => new(0, 0, 0, 0.0f);
    public override string MENU_BUTTON_MOBILE_BOX_SHADOW => "none";

    // MenuButton [mobile][hover]:
    public override RGBAColor MENU_BUTTON_MOBILE_HOVER_COLOR => new(122, 113, 136);
    public override RGBAColor MENU_BUTTON_MOBILE_HOVER_BACKGROUND => new(0, 0, 0, 0.0f);
    public override string MENU_BUTTON_MOBILE_HOVER_BOX_SHADOW => "none";

    // MenuButton [mobile][focus]:
    public override RGBAColor MENU_BUTTON_MOBILE_FOCUS_COLOR => new(42, 33, 56);
    public override RGBAColor MENU_BUTTON_MOBILE_FOCUS_BACKGROUND => new(42, 33, 56, 0.055f);
    public override string MENU_BUTTON_MOBILE_FOCUS_BOX_SHADOW => "0em 2em 10em rgba(0, 0, 0, 0.02)";

    // MenuButton [mobile][active]:
    public override RGBAColor MENU_BUTTON_MOBILE_ACTIVE_COLOR => new(42, 33, 56);
    public override RGBAColor MENU_BUTTON_MOBILE_ACTIVE_BACKGROUND => new(0, 0, 0, 0.0f);
    public override string MENU_BUTTON_MOBILE_ACTIVE_BOX_SHADOW => "0em 2em 7em rgba(0, 0, 0, 0.35)";

    // MenuButton [mobile][active][hover]:
    public override RGBAColor MENU_BUTTON_MOBILE_ACTIVE_HOVER_COLOR => new(42, 33, 56);
    public override RGBAColor MENU_BUTTON_MOBILE_ACTIVE_HOVER_BACKGROUND => new(0, 0, 0, 0.0f);
    public override string MENU_BUTTON_MOBILE_ACTIVE_HOVER_BOX_SHADOW => "0em 2em 7em rgba(0, 0, 0, 0.35)";

    // MenuButton [mobile][active][focus]:
    public override RGBAColor MENU_BUTTON_MOBILE_ACTIVE_FOCUS_COLOR => new(42, 33, 56);
    public override RGBAColor MENU_BUTTON_MOBILE_ACTIVE_FOCUS_BACKGROUND => new(0, 0, 0, 0.0f);
    public override string MENU_BUTTON_MOBILE_ACTIVE_FOCUS_BOX_SHADOW => "0em 2em 9em rgba(0, 0, 0, 0.5)";

// NOTE: Collapse -------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor COLLAPSE_TEXT_COLOR => new(42, 33, 56);
    public override RGBAColor COLLAPSE_ICON_COLOR => new(42, 33, 56);
    public override RGBAColor COLLAPSE_ICON_BACKGROUND => new(235, 235, 235);
    public virtual RGBAColor COLLAPSE_ICON_BACKGROUND__SURFACE_FLOATING_COLLAPSE => new(220, 220, 220);
    public virtual RGBAColor COLLAPSE_ICON_BACKGROUND__SURFACE_FLOATING_ADDITIONAL_COLLAPSE => new(220, 220, 220);
    public override RGBAColor COLLAPSE_BACKGROUND => SURFACE_BACKGROUND__SURFACE_PRIMARY_COLLAPSE;
    public virtual RGBAColor COLLAPSE_BACKGROUND__SURFACE_FLOATING_COLLAPSE => SURFACE_BACKGROUND__SURFACE_FLOATING_COLLAPSE;
    public virtual RGBAColor COLLAPSE_BACKGROUND__SURFACE_FLOATING_ADDITIONAL_COLLAPSE => SURFACE_BACKGROUND__SURFACE_FLOATING_ADDITIONAL_COLLAPSE;
    public override string COLLAPSE_FOCUS_BOX_SHADOW => "0 0 10em rgb(204, 204, 204)";
    public virtual string COLLAPSE_FOCUS_BOX_SHADOW__SURFACE_FLOATING_COLLAPSE => "0 0 10em rgb(190, 190, 190)";
    public virtual string COLLAPSE_FOCUS_BOX_SHADOW__SURFACE_FLOATING_ADDITIONAL_COLLAPSE => "0 0 10em rgb(160, 160, 160)";

// NOTE: DropDowns ------------------------------------------------------------------------------------------------------------------------
    // DropDown ---------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor DROPDOWN_OPTIONS_BACKGROUND => new(255, 255, 255);
    public override RGBAColor DROPDOWN_OPTIONS_DIVIDER_COLOR => new(0, 0, 0, 0.1f);
    public override string DROPDOWN_OPTIONS_BOX_SHADOW => "0 0 12em rgba(0, 0, 0, 0.2)";
    public override string DROPDOWN_MARK_BOX_SHADOW => "0 0 8em 2em rgba(0, 0, 0, 0.2)";

    // DropDown [highlight]:
    public override RGBAColor DROPDOWN_OPTIONS_HIGHLIGHT_BACKGROUND => new(241, 241, 241);

    // AdminDropDown ----------------------------------------------------------------------------------------------------------------------
    public override RGBAColor DROPDOWN_ADMIN_BACKGROUND => new(102, 102, 102);
    public override RGBAColor DROPDOWN_ADMIN_BORDER_COLOR => new(255, 255, 255, 0.9f);
    public override string DROPDOWN_ADMIN_BOX_SHADOW => "0 0 3em 1em rgba(0, 0, 0, 0.5)";

    // AdminDropDown [highlight]:
    public override RGBAColor DROPDOWN_ADMIN_HIGHLIGHT_BORDER_COLOR => new(255, 255, 255);
    public override string DROPDOWN_ADMIN_HIGHLIGHT_BOX_SHADOW => "0 0 3em 2em rgba(0, 0, 0, 0.4)";

    // UserDropDown -----------------------------------------------------------------------------------------------------------------------
    public override RGBAColor DROPDOWN_USER_BACKGROUND => new(102, 102, 102);
    public override RGBAColor DROPDOWN_USER_BORDER_COLOR => new(255, 255, 255, 0.9f);
    public override string DROPDOWN_USER_BOX_SHADOW => "0 0 3em 2em rgba(42, 33, 56, 0.5)";

    // UserDropDown [highlight]:
    public override RGBAColor DROPDOWN_USER_HIGHLIGHT_BORDER_COLOR => new(255, 255, 255);
    public override string DROPDOWN_USER_HIGHLIGHT_BOX_SHADOW => "0 0 3em 3em rgba(0, 0, 0, 0.5)";

// NOTE: Forms ----------------------------------------------------------------------------------------------------------------------------
    // Form [primary] ---------------------------------------------------------------------------------------------------------------------
    public override RGBAColor FORM_PRIMARY_COLOR => new(42, 33, 56);
    public override RGBAColor FORM_PRIMARY_PLACEHOLDER_COLOR => new(182, 178, 189);
    public override RGBAColor FORM_PRIMARY_ICON_COLOR => new(148, 148, 148);
    public override RGBAColor FORM_PRIMARY_DESCRIPTION_COLOR => new(42, 33, 56);
    public override RGBAColor FORM_PRIMARY_BACKGROUND => new(255, 255, 255);
    public override RGBAColor FORM_PRIMARY_BORDER_COLOR => new(204, 204, 204);
    public override string FORM_PRIMARY_TEXT_SHADOW => "none";
    public override string FORM_PRIMARY_BOX_SHADOW => "none";

    // Form [primary][highlight]:
    public override RGBAColor FORM_PRIMARY_HIGHLIGHT_COLOR => new(42, 33, 56);
    public override RGBAColor FORM_PRIMARY_HIGHLIGHT_PLACEHOLDER_COLOR => new(182, 178, 189);
    public override RGBAColor FORM_PRIMARY_HIGHLIGHT_ICON_COLOR => new(148, 148, 148);
    public override RGBAColor FORM_PRIMARY_HIGHLIGHT_DESCRIPTION_COLOR => new(42, 33, 56);
    public override RGBAColor FORM_PRIMARY_HIGHLIGHT_BACKGROUND => new(255, 255, 255);
    public override RGBAColor FORM_PRIMARY_HIGHLIGHT_BORDER_COLOR => new(204, 204, 204);
    public override string FORM_PRIMARY_HIGHLIGHT_TEXT_SHADOW => "none";
    public override string FORM_PRIMARY_HIGHLIGHT_BOX_SHADOW => "0 0 5em rgba(0, 0, 0, 0.4)";

    // Form [primary][disabled]:
    public override RGBAColor FORM_PRIMARY_DISABLED_COLOR => new(190, 190, 190);
    public override RGBAColor FORM_PRIMARY_DISABLED_PLACEHOLDER_COLOR => new(190, 190, 190);
    public override RGBAColor FORM_PRIMARY_DISABLED_ICON_COLOR => new(190, 190, 190);
    public override RGBAColor FORM_PRIMARY_DISABLED_DESCRIPTION_COLOR => new(180, 180, 180);
    public virtual RGBAColor FORM_PRIMARY_DISABLED_DESCRIPTION_COLOR__SURFACE_FLOATING_COLLAPSE => new(160, 160, 160);
    public virtual RGBAColor FORM_PRIMARY_DISABLED_DESCRIPTION_COLOR__SURFACE_FLOATING_ADDITIONAL => new(140, 140, 140);
    public virtual RGBAColor FORM_PRIMARY_DISABLED_DESCRIPTION_COLOR__SURFACE_FLOATING_ADDITIONAL_COLLAPSE => new(160, 160, 160);
    public override RGBAColor FORM_PRIMARY_DISABLED_BACKGROUND => new(240, 240, 240);
    public override RGBAColor FORM_PRIMARY_DISABLED_BORDER_COLOR => new(214, 214, 214);
    public override string FORM_PRIMARY_DISABLED_TEXT_SHADOW => "none";
    public override string FORM_PRIMARY_DISABLED_BOX_SHADOW => "none";

    // Form [primary] > Icon [highlight]:
    public override RGBAColor FORM_PRIMARY_ICON_HIGHLIGHT_COLOR => new(70, 70, 70);
    
    // Form [primary] > Clear:
    public override RGBAColor FORM_PRIMARY_CLEAR_COLOR => new(42, 33, 56);
    public override RGBAColor FORM_PRIMARY_CLEAR_BACKGROUND => new(230, 230, 230);
    public override string FORM_PRIMARY_CLEAR_BOX_SHADOW => "none";
    
    // Form [primary] > Clear [highlight]:
    public override RGBAColor FORM_PRIMARY_CLEAR_HIGHLIGHT_COLOR => new(0, 0, 0);
    public override RGBAColor FORM_PRIMARY_CLEAR_HIGHLIGHT_BACKGROUND => new(216, 216, 216);
    public override string FORM_PRIMARY_CLEAR_HIGHLIGHT_BOX_SHADOW => "none";

// NOTE: Forms > CheckBoxes ---------------------------------------------------------------------------------------------------------------
    // CheckBox [primary] -----------------------------------------------------------------------------------------------------------------
    // CheckBox [primary][checked]:
    public override RGBAColor CHECKBOX_PRIMARY_CHECKED_MARK_COLOR => new(255, 255, 255);
    public override RGBAColor CHECKBOX_PRIMARY_CHECKED_BACKGROUND => new(42, 33, 56);
    public virtual RGBAColor CHECKBOX_PRIMARY_CHECKED_BACKGROUND__SURFACE_SECONDARY => new(235, 195, 0);

    // CheckBox [primary][disabled][checked]:
    public override RGBAColor CHECKBOX_PRIMARY_DISABLED_CHECKED_MARK_COLOR => new(240, 240, 240);
    public override RGBAColor CHECKBOX_PRIMARY_DISABLED_CHECKED_BACKGROUND => new(180, 180, 180);

// NOTE: Forms > Radios -------------------------------------------------------------------------------------------------------------------
    // Radio [primary] --------------------------------------------------------------------------------------------------------------------
    // Radio [primary][selected]:
    public override RGBAColor RADIO_PRIMARY_SELECTED_MARK_COLOR => new(42, 33, 56);
    public override RGBAColor RADIO_PRIMARY_SELECTED_BACKGROUND => new(255, 255, 255);

    // Radio [primary][disabled][selected]:
    public override RGBAColor RADIO_PRIMARY_DISABLED_SELECTED_MARK_COLOR => new(180, 180, 180);
    public override RGBAColor RADIO_PRIMARY_DISABLED_SELECTED_BACKGROUND => new(240, 240, 240);

    // RadioButton [primary] --------------------------------------------------------------------------------------------------------------
    public override RGBAColor RADIO_BUTTON_PRIMARY_COLOR => new(42, 33, 56);
    public override RGBAColor RADIO_BUTTON_PRIMARY_BACKGROUND => new(252, 252, 252);
    public override string RADIO_BUTTON_PRIMARY_BOX_SHADOW => "0 0 6em rgba(0, 0, 0, 0.2), 0 0 12em 4em rgba(0, 0, 0, 0.03) inset";

    // RadioButton [primary][selected]:
    public override RGBAColor RADIO_BUTTON_PRIMARY_SELECTED_COLOR => new(40, 40, 40);
    public override RGBAColor RADIO_BUTTON_PRIMARY_SELECTED_BACKGROUND => new(255, 215, 0);
    public override string RADIO_BUTTON_PRIMARY_SELECTED_BOX_SHADOW => "0 0 6em 4em rgba(0, 0, 0, 0.3)";
    public override RGBAColor RADIO_BUTTON_PRIMARY_SELECTED_OUTLINE_COLOR => new(255, 255, 255);

    // RadioButton [primary][highlight]:
    public override RGBAColor RADIO_BUTTON_PRIMARY_HIGHLIGHT_COLOR => new(0, 0, 0);
    public override RGBAColor RADIO_BUTTON_PRIMARY_HIGHLIGHT_BACKGROUND => new(255, 255, 255);
    public override string RADIO_BUTTON_PRIMARY_HIGHLIGHT_BOX_SHADOW => "0 0 6em rgba(0, 0, 0, 0.2)";

    // RadioButton [primary][highlight][selected]:
    public override RGBAColor RADIO_BUTTON_PRIMARY_HIGHLIGHT_SELECTED_COLOR => new(20, 20, 20);
    public override RGBAColor RADIO_BUTTON_PRIMARY_HIGHLIGHT_SELECTED_BACKGROUND => new(255, 239, 0);
    public override string RADIO_BUTTON_PRIMARY_HIGHLIGHT_SELECTED_BOX_SHADOW => "0 0 6em 4em rgba(0, 0, 0, 0.3)";
    public override RGBAColor RADIO_BUTTON_PRIMARY_HIGHLIGHT_SELECTED_OUTLINE_COLOR => new(255, 255, 255);
    
    // RadioButton [primary][disabled]:
    public override RGBAColor RADIO_BUTTON_PRIMARY_DISABLED_COLOR => new(190, 190, 190);
    public override RGBAColor RADIO_BUTTON_PRIMARY_DISABLED_BACKGROUND => new(240, 240, 240);
    public override string RADIO_BUTTON_PRIMARY_DISABLED_BOX_SHADOW => "0 0 6em rgba(0, 0, 0, 0.15)";

    // RadioButton [primary][disabled][selected]:
    public override RGBAColor RADIO_BUTTON_PRIMARY_DISABLED_SELECTED_COLOR => new(130, 130, 130);
    public override RGBAColor RADIO_BUTTON_PRIMARY_DISABLED_SELECTED_BACKGROUND => new(210, 210, 210);
    public override string RADIO_BUTTON_PRIMARY_DISABLED_SELECTED_BOX_SHADOW => "0 0 6em 4em rgba(0, 0, 0, 0.2)";
    public override RGBAColor RADIO_BUTTON_PRIMARY_DISABLED_SELECTED_OUTLINE_COLOR => new(255, 255, 255);

// NOTE: Forms > Selects ------------------------------------------------------------------------------------------------------------------
    // Select [primary] -------------------------------------------------------------------------------------------------------------------
    public override RGBAColor SELECT_OPTION_COLOR => new(42, 33, 56);
    public override RGBAColor SELECT_OPTION_BACKGROUND => new(255, 255, 255);
    public override RGBAColor SELECT_OPTION_DIVIDER_COLOR => new(233, 233, 233);

    // Select [primary][highlight]:
    public override RGBAColor SELECT_OPTION_HIGHLIGHT_COLOR => new(42, 33, 56);
    public override RGBAColor SELECT_OPTION_HIGHLIGHT_BACKGROUND => new(233, 233, 233);

    // Select [active]:
    public override RGBAColor SELECT_OPTION_ACTIVE_COLOR => new(255, 255, 255);
    public override RGBAColor SELECT_OPTION_ACTIVE_BACKGROUND => new(148, 148, 148);

    // Select [active][highlight]:
    public override RGBAColor SELECT_OPTION_ACTIVE_HIGHLIGHT_COLOR => new(255, 255, 255);
    public override RGBAColor SELECT_OPTION_ACTIVE_HIGHLIGHT_BACKGROUND => new(170, 170, 170);

    // SelectCulture ----------------------------------------------------------------------------------------------------------------------
    public override RGBAColor SELECT_CULTURE_COLOR => new(42, 33, 56);
    public override RGBAColor SELECT_CULTURE_BACKGROUND => new(0, 0, 0, 0.0f);
    public override RGBAColor SELECT_CULTURE_BORDER_COLOR => new(0, 0, 0, 0.2f);
    public override string SELECT_CULTURE_BOX_SHADOW => "none";

    // SelectCulture [highlight]:
    public override RGBAColor SELECT_CULTURE_HIGHLIGHT_COLOR => new(42, 33, 56);
    public override RGBAColor SELECT_CULTURE_HIGHLIGHT_BACKGROUND => new(0, 0, 0, 0.0f);
    public override RGBAColor SELECT_CULTURE_HIGHLIGHT_BORDER_COLOR => new(0, 0, 0, 0.2f);
    public override string SELECT_CULTURE_HIGHLIGHT_BOX_SHADOW => "0 1em 6em rgba(0, 0, 0, 0.3)";

// NOTE: Forms > SelectsMulti -------------------------------------------------------------------------------------------------------------
    // SelectMulti [primary] --------------------------------------------------------------------------------------------------------------
    public override RGBAColor SELECT_MULTI_PRIMARY_COUNT_COLOR => new(255, 255, 255);
    public override RGBAColor SELECT_MULTI_PRIMARY_COUNT_BACKGROUND => new(190, 190, 190);
    public override string SELECT_MULTI_PRIMARY_COUNT_TEXT_SHADOW => "1px 1px 2px rgba(0, 0, 0, 0.4)";
    public override string SELECT_MULTI_PRIMARY_COUNT_BOX_SHADOW => "1px 0 3px rgba(0, 0, 0, 0.5)";
    public override RGBAColor SELECT_MULTI_PRIMARY_COUNT_PLUS_COLOR => new(255, 255, 255);
    public override string SELECT_MULTI_PRIMARY_COUNT_PLUS_TEXT_SHADOW => "1px 1px 2px rgba(0, 0, 0, 0.4)";

    // SelectMulti [primary][disabled]:
    public override RGBAColor SELECT_MULTI_PRIMARY_DISABLED_COUNT_COLOR => new(240, 240, 240);
    public override RGBAColor SELECT_MULTI_PRIMARY_DISABLED_COUNT_BACKGROUND => new(220, 220, 220);
    public override string SELECT_MULTI_PRIMARY_DISABLED_COUNT_TEXT_SHADOW => "1px 1px 2px rgba(0, 0, 0, 0.1)";
    public override string SELECT_MULTI_PRIMARY_DISABLED_COUNT_BOX_SHADOW => "1px 0 3px rgba(0, 0, 0, 0.2)";
    public override RGBAColor SELECT_MULTI_PRIMARY_DISABLED_COUNT_PLUS_COLOR => new(240, 240, 240);
    public override string SELECT_MULTI_PRIMARY_DISABLED_COUNT_PLUS_TEXT_SHADOW => "1px 1px 2px rgba(0, 0, 0, 0.1)";

// NOTE: Forms > Switches -----------------------------------------------------------------------------------------------------------------
    // Switch [primary] -------------------------------------------------------------------------------------------------------------------
    public override RGBAColor SWITCH_PRIMARY_BACKGROUND => new(200, 200, 200);
    public virtual RGBAColor SWITCH_PRIMARY_BACKGROUND__SURFACE_FLOATING_COLLAPSE => new(180, 180, 180);
    public override RGBAColor SWITCH_PRIMARY_BULLET_BACKGROUND => new(255, 255, 255);

    // Switch [primary][checked]:
    public override RGBAColor SWITCH_PRIMARY_CHECKED_BACKGROUND => new(42, 33, 56);
    public virtual RGBAColor SWITCH_PRIMARY_CHECKED_BACKGROUND__SURFACE_SECONDARY => new(235, 195, 0);

    // Switch [primary][focus]:
    public override RGBAColor SWITCH_PRIMARY_FOCUS_OUTLINE_COLOR => new(255, 255, 255);
    public override string SWITCH_PRIMARY_FOCUS_OUTLINE_SHADOW => "0 0 2em rgb(0, 0, 0)";

    // Switch [primary][disabled]:
    public override RGBAColor SWITCH_PRIMARY_DISABLED_BACKGROUND => new(220, 220, 220);
    public virtual RGBAColor SWITCH_PRIMARY_DISABLED_BACKGROUND__SURFACE_FLOATING_COLLAPSE => new(200, 200, 200);
    public virtual RGBAColor SWITCH_PRIMARY_DISABLED_BACKGROUND__SURFACE_FLOATING_ADDITIONAL => new(180, 180, 180);
    public virtual RGBAColor SWITCH_PRIMARY_DISABLED_BACKGROUND__SURFACE_FLOATING_ADDITIONAL_COLLAPSE => new(200, 200, 200);
    public override RGBAColor SWITCH_PRIMARY_DISABLED_BULLET_BACKGROUND => new(240, 240, 240);

    // Switch [primary][disabled][checked]:
    public override RGBAColor SWITCH_PRIMARY_DISABLED_CHECKED_BACKGROUND => new(180, 180, 180);

// NOTE: Images ---------------------------------------------------------------------------------------------------------------------------
    // Background -------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor BACKGROUND_LIGHT_COLOR => new(210, 210, 210);
    public override RGBAColor BACKGROUND_DARK_COLOR => new(180, 180, 180);

    // Image ------------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor IMAGE_LIGHT_COLOR => new(210, 210, 210);
    public override RGBAColor IMAGE_DARK_COLOR => new(180, 180, 180);
    public override RGBAColor IMAGE_ERROR_COLOR => new(180, 180, 180);
    public override RGBAColor IMAGE_ICON_COLOR => new(60, 60, 60);

// NOTE: Links ----------------------------------------------------------------------------------------------------------------------------
    // LogoLink ---------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor LOGO_LINK_COLOR => new(42, 33, 56);
    public override RGBAColor LOGO_LINK_BACKGROUND => new(0, 0, 0, 0.0f);
    public override string LOGO_LINK_BOX_SHADOW => "none";

    // LogoLink [focus]:
    public override RGBAColor LOGO_LINK_FOCUS_COLOR => new(42, 33, 56);
    public override RGBAColor LOGO_LINK_FOCUS_BACKGROUND => new(42, 33, 56, 0.08f);
    public override string LOGO_LINK_FOCUS_BOX_SHADOW => "0 0.05em 0.3em rgba(0, 0, 0, 0.4)";

// NOTE: Loaders --------------------------------------------------------------------------------------------------------------------------
    // Loader -----------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor LOADER_COLOR => new(42, 33, 56);
    public virtual RGBAColor LOADER_COLOR__SURFACE_SECONDARY => new(255, 215, 0);
    public override RGBAColor LOADER_BACKGROUND => new(42, 33, 56, 0.4f);
    public virtual RGBAColor LOADER_BACKGROUND__SURFACE_SECONDARY => new(0, 0, 0, 0.45f);

    // PageLoader -------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor PAGE_LOADER_COLOR => new(235, 195, 0);
    public override RGBAColor PAGE_LOADER_BACKGROUND => new(0, 0, 0, 0.45f);

    // ServerPageLoader -------------------------------------------------------------------------------------------------------------------
    public override RGBAColor SERVER_PAGE_LOADER_TEXT_COLOR => new(42, 33, 56, 0.9f);
    public override RGBAColor SERVER_PAGE_LOADER_COLOR => PAGE_LOADER_COLOR;
    public override RGBAColor SERVER_PAGE_LOADER_BACKGROUND => PAGE_LOADER_BACKGROUND;
    public override RGBAColor SERVER_PAGE_LOADER_BACKDROP => SURFACE_BACKGROUND__SURFACE_SECONDARY;

// NOTE: Modals ---------------------------------------------------------------------------------------------------------------------------
    // Modal ------------------------------------------------------------------------------------------------------------------------------
    public override string MODAL_DIALOG_BOX_SHADOW => "0 0 30em 0 rgba(0, 0, 0, 0.3)";
    public override string MODAL_ENDING_BOX_SHADOW => "0 0 10em rgba(0, 0, 0, 0.1)";

    // Modal control:
    public override RGBAColor MODAL_CONTROL_COLOR => new(255, 255, 255);
    public override RGBAColor MODAL_CONTROL_BACKGROUND => new(170, 170, 170);
    public override string MODAL_CONTROL_BOX_SHADOW => "none";

    // Modal control [highlight]:
    public override RGBAColor MODAL_CONTROL_HIGHLIGHT_COLOR => new(255, 255, 255);
    public override RGBAColor MODAL_CONTROL_HIGHLIGHT_BACKGROUND => new(190, 190, 190);
    public override string MODAL_CONTROL_HIGHLIGHT_BOX_SHADOW => "none";

    // CookieModal ------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor MODAL_COOKIE_BUTTON_COLOR => new(42, 33, 56);

    // CookieModal [highlight]:
    public override RGBAColor MODAL_COOKIE_BUTTON_HIGHLIGHT_COLOR => new(42, 33, 56);
    public override string MODAL_COOKIE_BUTTON_HIGHLIGHT_TEXT_SHADOW => "0 0 1em rgb(42, 33, 56)";

    // ProfileModal -----------------------------------------------------------------------------------------------------------------------
    public override RGBAColor MODAL_PROFILE_AVATAR_BACKGROUND => new(102, 102, 102);
    public override RGBAColor MODAL_PROFILE_AVATAR_BORDER_COLOR => new(255, 255, 255);
    public override string MODAL_PROFILE_AVATAR_BOX_SHADOW => "0 1em 8em 1em rgba(0, 0, 0, 0.7)";

// NOTE: Progress -------------------------------------------------------------------------------------------------------------------------
    // ProgressCircle ---------------------------------------------------------------------------------------------------------------------
    public override RGBAColor PROGRESS_CIRCLE_COLOR => new(42, 33, 56);
    public virtual RGBAColor PROGRESS_CIRCLE_COLOR__SURFACE_SECONDARY => new(255, 215, 0);
    public override RGBAColor PROGRESS_CIRCLE_BACKGROUND => new(42, 33, 56, 0.4f);
    public virtual RGBAColor PROGRESS_CIRCLE_BACKGROUND__SURFACE_SECONDARY => new(0, 0, 0, 0.45f);

// NOTE: Text -----------------------------------------------------------------------------------------------------------------------------
    // Text -------------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor TEXT_COLOR => new(42, 33, 56);

    // Text [highlight]:
    public override RGBAColor TEXT_HIGHLIGHT_COLOR => new(122, 113, 136);

    // Text Accent ------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor TEXT_ACCENT_COLOR => new(42, 33, 56);
    public virtual RGBAColor TEXT_ACCENT_COLOR__SURFACE_SECONDARY => new(235, 195, 0);

    // Text Accent [highlight]:
    public override RGBAColor TEXT_ACCENT_HIGHLIGHT_COLOR => new(122, 113, 136);
    public virtual RGBAColor TEXT_ACCENT_HIGHLIGHT_COLOR__SURFACE_SECONDARY => new(235, 219, 0);

    // Text [disabled] --------------------------------------------------------------------------------------------------------------------
    public override RGBAColor TEXT_DISABLED_COLOR => new(180, 180, 180);
}

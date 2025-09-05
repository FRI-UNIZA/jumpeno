namespace Jumpeno.Client.Constants;

#pragma warning disable CS8618
#pragma warning disable CA1822

public class BaseTheme {
// NOTE: Constants ------------------------------------------------------------------------------------------------------------------------
    // Fonts ------------------------------------------------------------------------------------------------------------------------------
    public string FONT_PRIMARY => "Montserrat, sans-serif";

    // Sizes (px) -------------------------------------------------------------------------------------------------------------------------
    public int SIZE_CONTAINER_MAX_WIDTH => 1340;
    public int SIZE_CONTAINER_PADDING_MOBILE => 16;
    public int SIZE_CONTAINER_PADDING_TABLET => 28;
    public int SIZE_CONTAINER_PADDING_DESKTOP => 40;
    public int SIZE_HEADER_HEIGHT => 75;
    public int SIZE_FOOTER_HEIGHT_MOBILE => 180;
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

    // Email ------------------------------------------------------------------------------------------------------------------------------
    public RGBAColor EMAIL_COLOR => new(42, 33, 56);
    public RGBAColor EMAIL_ACCENT_COLOR => new(255, 215, 0);
    
// NOTE: Surface --------------------------------------------------------------------------------------------------------------------------
    // Primary ----------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor SURFACE_BACKGROUND__SURFACE_PRIMARY { get; }
    public virtual RGBAColor SURFACE_BACKGROUND__SURFACE_PRIMARY_COLLAPSE { get; }
    public virtual RGBAColor SURFACE_BACKGROUND__SURFACE_PRIMARY_BOX { get; }
    public virtual RGBAColor SURFACE_BACKGROUND__SURFACE_PRIMARY_BOX_COLLAPSE { get; }
    public virtual RGBAColor SURFACE_BACKGROUND__SURFACE_PRIMARY_GLASS { get; }
    public virtual RGBAColor SURFACE_BACKGROUND__SURFACE_PRIMARY_GLASS_COLLAPSE { get; }

    // Secondary --------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor SURFACE_BACKGROUND__SURFACE_SECONDARY { get; }

    // Floating ---------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor SURFACE_BACKGROUND__SURFACE_FLOATING { get; }
    public virtual RGBAColor SURFACE_BACKGROUND__SURFACE_FLOATING_COLLAPSE { get; }
    public virtual RGBAColor SURFACE_BACKGROUND__SURFACE_FLOATING_ADDITIONAL { get; }
    public virtual RGBAColor SURFACE_BACKGROUND__SURFACE_FLOATING_ADDITIONAL_COLLAPSE { get; }

// NOTE: Status ---------------------------------------------------------------------------------------------------------------------------
    // Danger -----------------------------------------------------------------------------------------------------------------------------
    // Box:
    public virtual RGBAColor STATUS_DANGER_BOX_COLOR { get; }
    public virtual RGBAColor STATUS_DANGER_BOX_BACKGROUND { get; }
    public virtual RGBAColor STATUS_DANGER_BOX_OUTLINE_COLOR { get; }
    // Box [highlight]:
    public virtual RGBAColor STATUS_DANGER_BOX_HIGHLIGHT_COLOR { get; }
    public virtual RGBAColor STATUS_DANGER_BOX_HIGHLIGHT_BACKGROUND { get; }
    public virtual RGBAColor STATUS_DANGER_BOX_HIGHLIGHT_OUTLINE_COLOR { get; }
    
    // Accent:
    public virtual RGBAColor STATUS_DANGER_ACCENT_COLOR { get; }
    // Accent [highlight]:
    public virtual RGBAColor STATUS_DANGER_ACCENT_HIGHLIGHT_COLOR { get; }
    
    // Neon:
    public virtual RGBAColor STATUS_DANGER_NEON_COLOR { get; }

    // Success ----------------------------------------------------------------------------------------------------------------------------
    // Box:
    public virtual RGBAColor STATUS_SUCCESS_BOX_COLOR { get; }
    public virtual RGBAColor STATUS_SUCCESS_BOX_BACKGROUND { get; }
    public virtual RGBAColor STATUS_SUCCESS_BOX_OUTLINE_COLOR { get; }
    // Box [highlight]:
    public virtual RGBAColor STATUS_SUCCESS_BOX_HIGHLIGHT_COLOR { get; }
    public virtual RGBAColor STATUS_SUCCESS_BOX_HIGHLIGHT_BACKGROUND { get; }
    public virtual RGBAColor STATUS_SUCCESS_BOX_HIGHLIGHT_OUTLINE_COLOR { get; }

    // Accent:
    public virtual RGBAColor STATUS_SUCCESS_ACCENT_COLOR { get; }
    // Accent [highlight]:
    public virtual RGBAColor STATUS_SUCCESS_ACCENT_HIGHLIGHT_COLOR { get; }

    // Neon:
    public virtual RGBAColor STATUS_SUCCESS_NEON_COLOR { get; }

    // Warning ----------------------------------------------------------------------------------------------------------------------------
    // Box:
    public virtual RGBAColor STATUS_WARNING_BOX_COLOR { get; }
    public virtual RGBAColor STATUS_WARNING_BOX_BACKGROUND { get; }
    public virtual RGBAColor STATUS_WARNING_BOX_OUTLINE_COLOR { get; }
    // Box [highlight]:
    public virtual RGBAColor STATUS_WARNING_BOX_HIGHLIGHT_COLOR { get; }
    public virtual RGBAColor STATUS_WARNING_BOX_HIGHLIGHT_BACKGROUND { get; }
    public virtual RGBAColor STATUS_WARNING_BOX_HIGHLIGHT_OUTLINE_COLOR { get; }

    // Accent:
    public virtual RGBAColor STATUS_WARNING_ACCENT_COLOR { get; }
    // Accent [highlight]:
    public virtual RGBAColor STATUS_WARNING_ACCENT_HIGHLIGHT_COLOR { get; }

    // Neon:
    public virtual RGBAColor STATUS_WARNING_NEON_COLOR { get; }

    // Info -------------------------------------------------------------------------------------------------------------------------------
    // Box:
    public virtual RGBAColor STATUS_INFO_BOX_COLOR { get; }
    public virtual RGBAColor STATUS_INFO_BOX_BACKGROUND { get; }
    public virtual RGBAColor STATUS_INFO_BOX_OUTLINE_COLOR { get; }
    // Box [highlight]:
    public virtual RGBAColor STATUS_INFO_BOX_HIGHLIGHT_COLOR { get; }
    public virtual RGBAColor STATUS_INFO_BOX_HIGHLIGHT_BACKGROUND { get; }
    public virtual RGBAColor STATUS_INFO_BOX_HIGHLIGHT_OUTLINE_COLOR { get; }

    // Accent:
    public virtual RGBAColor STATUS_INFO_ACCENT_COLOR { get; }
    // Accent [highlight]:
    public virtual RGBAColor STATUS_INFO_ACCENT_HIGHLIGHT_COLOR { get; }

    // Neon:
    public virtual RGBAColor STATUS_INFO_NEON_COLOR { get; }

// NOTE: Layout ---------------------------------------------------------------------------------------------------------------------------
    // Body -------------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor BODY_BACKGROUND { get; }
    // Scrollbars:
    public virtual SCROLLAREA_THEME BODY_SCROLL_THEME { get; }
    // Selection:
    public virtual RGBAColor BODY_SELECTION_COLOR { get; }
    public virtual RGBAColor BODY_SELECTION_BACKGROUND { get; }
    // Backdrop:
    public virtual RGBAColor BODY_BACKDROP { get; }

    // NavMenu ----------------------------------------------------------------------------------------------------------------------------
    public virtual string NAV_MENU_BOX_SHADOW { get; }

    // NavMenu [mobile]:
    public virtual RGBAColor NAV_MENU_MOBILE_BUTTON_COLOR { get; }
    public virtual SCROLLAREA_THEME NAV_MENU_MOBILE_SCROLL_THEME { get; }

    // NavMenu [mobile][highlight]:
    public virtual RGBAColor NAV_MENU_MOBILE_BUTTON_HIGHLIGHT_COLOR { get; }

    // NavMenu [mobile][focus]:
    public virtual RGBAColor NAV_MENU_MOBILE_BUTTON_FOCUS_BACKGROUND { get; }
    public virtual string NAV_MENU_MOBILE_BUTTON_FOCUS_BOX_SHADOW { get; }
    
    // QR code ----------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor QR_CODE_BACKGROUND { get; }

// NOTE: Pages ----------------------------------------------------------------------------------------------------------------------------
    // Game -------------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor GAME_STATE_COLOR { get; }
    public virtual string GAME_STATE_TEXT_SHADOW { get; }

    // GameScreen:
    public virtual RGBAColor GAME_SCREEN_CANVAS_BACKGROUND { get; }
    public virtual string GAME_SCREEN_CANVAS_BOX_SHADOW { get; }

    // GameScreen > Control:
    public virtual RGBAColor GAME_SCREEN_CONTROL_COLOR { get; }
    public virtual RGBAColor GAME_SCREEN_CONTROL_BACKGROUND { get; }
    public virtual string GAME_SCREEN_CONTROL_BOX_SHADOW { get; }
    
    // GameScreen > Control [pressed]:
    public virtual RGBAColor GAME_SCREEN_CONTROL_PRESSED_COLOR { get; }
    public virtual RGBAColor GAME_SCREEN_CONTROL_PRESSED_BACKGROUND { get; }
    public virtual string GAME_SCREEN_CONTROL_PRESSED_BOX_SHADOW { get; }

    // Lobby:
    public virtual string LOBBY_BOX_SHADOW { get; }
    public virtual RGBAColor LOBBY_EMPTY_COLOR { get; }
    public virtual RGBAColor LOBBY_LINE_BACKGROUND { get; }
    public virtual string LOBBY_PRESENCE_BOX_SHADOW { get; }
    public virtual RGBAColor LOBBY_DASH_COLOR { get; }

    // Manual -----------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor MANUAL_COLOR { get; }
    public virtual string MANUAL_TEXT_SHADOW { get; }
    public virtual RGBAColor MANUAL_BACKGROUND { get; }
    public virtual RGBAColor MANUAL_BACKGROUND_TRANSITION { get; }
    public virtual string MANUAL_BOX_SHADOW { get; }

// NOTE: Box ------------------------------------------------------------------------------------------------------------------------------
    // Box [box] --------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor BOX_BACKGROUND__SURFACE_PRIMARY_BOX { get; }
    public virtual RGBAColor BOX_BACKGROUND__SURFACE_PRIMARY_GLASS { get; }
    public virtual string BOX_BOX_SHADOW__SURFACE_PRIMARY_BOX { get; }
    public virtual string BOX_BOX_SHADOW__SURFACE_PRIMARY_GLASS { get; }

// NOTE: Buttons --------------------------------------------------------------------------------------------------------------------------
    // Button [primary] -------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor BUTTON_PRIMARY_COLOR { get; }
    public virtual RGBAColor BUTTON_PRIMARY_BACKGROUND { get; }
    public virtual string BUTTON_PRIMARY_BOX_SHADOW { get; }

    // Button [primary][highlight]:
    public virtual RGBAColor BUTTON_PRIMARY_HIGHLIGHT_COLOR { get; }
    public virtual RGBAColor BUTTON_PRIMARY_HIGHLIGHT_BACKGROUND { get; }
    public virtual string BUTTON_PRIMARY_HIGHLIGHT_BOX_SHADOW { get; }

    // Button [secondary] -----------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor BUTTON_SECONDARY_COLOR { get; }
    public virtual RGBAColor BUTTON_SECONDARY_BACKGROUND { get; }
    public virtual string BUTTON_SECONDARY_BOX_SHADOW { get; }

    // Button [secondary][highlight]:
    public virtual RGBAColor BUTTON_SECONDARY_HIGHLIGHT_COLOR { get; }
    public virtual RGBAColor BUTTON_SECONDARY_HIGHLIGHT_BACKGROUND { get; }
    public virtual string BUTTON_SECONDARY_HIGHLIGHT_BOX_SHADOW { get; }

    // Button [tertiary] ------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor BUTTON_TERTIARY_COLOR { get; }
    public virtual RGBAColor BUTTON_TERTIARY_BACKGROUND { get; }
    public virtual string BUTTON_TERTIARY_BOX_SHADOW { get; }

    // Button [tertiary][highlight]:
    public virtual RGBAColor BUTTON_TERTIARY_HIGHLIGHT_COLOR { get; }
    public virtual RGBAColor BUTTON_TERTIARY_HIGHLIGHT_BACKGROUND { get; }
    public virtual string BUTTON_TERTIARY_HIGHLIGHT_BOX_SHADOW { get; }

    // Button [quaternary] ----------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor BUTTON_QUATERNARY_COLOR { get; }
    public virtual RGBAColor BUTTON_QUATERNARY_BACKGROUND { get; }
    public virtual string BUTTON_QUATERNARY_BOX_SHADOW { get; }

    // Button [quaternary][highlight]:
    public virtual RGBAColor BUTTON_QUATERNARY_HIGHLIGHT_COLOR { get; }
    public virtual RGBAColor BUTTON_QUATERNARY_HIGHLIGHT_BACKGROUND { get; }
    public virtual string BUTTON_QUATERNARY_HIGHLIGHT_BOX_SHADOW { get; }

    // Button [disabled] ------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor BUTTON_DISABLED_COLOR { get; }
    public virtual RGBAColor BUTTON_DISABLED_BACKGROUND { get; }
    public virtual string BUTTON_DISABLED_BOX_SHADOW { get; }

    // MenuButton -------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor MENU_BUTTON_COLOR { get; }
    public virtual RGBAColor MENU_BUTTON_BACKGROUND { get; }
    public virtual string MENU_BUTTON_BOX_SHADOW { get; }

    // MenuButton [highlight]:
    public virtual RGBAColor MENU_BUTTON_HIGHLIGHT_COLOR { get; }
    public virtual RGBAColor MENU_BUTTON_HIGHLIGHT_BACKGROUND { get; }
    public virtual string MENU_BUTTON_HIGHLIGHT_BOX_SHADOW { get; }

    // MenuButton [active]:
    public virtual RGBAColor MENU_BUTTON_ACTIVE_COLOR { get; }
    public virtual RGBAColor MENU_BUTTON_ACTIVE_BACKGROUND { get; }
    public virtual string MENU_BUTTON_ACTIVE_BOX_SHADOW { get; }

    // MenuButton [active][highlight]:
    public virtual RGBAColor MENU_BUTTON_ACTIVE_HIGHLIGHT_COLOR { get; }
    public virtual RGBAColor MENU_BUTTON_ACTIVE_HIGHLIGHT_BACKGROUND { get; }
    public virtual string MENU_BUTTON_ACTIVE_HIGHLIGHT_BOX_SHADOW { get; }

    // MenuButton [mobile] ----------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor MENU_BUTTON_MOBILE_COLOR { get; }
    public virtual RGBAColor MENU_BUTTON_MOBILE_BACKGROUND { get; }
    public virtual string MENU_BUTTON_MOBILE_BOX_SHADOW { get; }

    // MenuButton [mobile][hover]:
    public virtual RGBAColor MENU_BUTTON_MOBILE_HOVER_COLOR { get; }
    public virtual RGBAColor MENU_BUTTON_MOBILE_HOVER_BACKGROUND { get; }
    public virtual string MENU_BUTTON_MOBILE_HOVER_BOX_SHADOW { get; }

    // MenuButton [mobile][focus]:
    public virtual RGBAColor MENU_BUTTON_MOBILE_FOCUS_COLOR { get; }
    public virtual RGBAColor MENU_BUTTON_MOBILE_FOCUS_BACKGROUND { get; }
    public virtual string MENU_BUTTON_MOBILE_FOCUS_BOX_SHADOW { get; }

    // MenuButton [mobile][active]:
    public virtual RGBAColor MENU_BUTTON_MOBILE_ACTIVE_COLOR { get; }
    public virtual RGBAColor MENU_BUTTON_MOBILE_ACTIVE_BACKGROUND { get; }
    public virtual string MENU_BUTTON_MOBILE_ACTIVE_BOX_SHADOW { get; }

    // MenuButton [mobile][active][hover]:
    public virtual RGBAColor MENU_BUTTON_MOBILE_ACTIVE_HOVER_COLOR { get; }
    public virtual RGBAColor MENU_BUTTON_MOBILE_ACTIVE_HOVER_BACKGROUND { get; }
    public virtual string MENU_BUTTON_MOBILE_ACTIVE_HOVER_BOX_SHADOW { get; }

    // MenuButton [mobile][active][focus]:
    public virtual RGBAColor MENU_BUTTON_MOBILE_ACTIVE_FOCUS_COLOR { get; }
    public virtual RGBAColor MENU_BUTTON_MOBILE_ACTIVE_FOCUS_BACKGROUND { get; }
    public virtual string MENU_BUTTON_MOBILE_ACTIVE_FOCUS_BOX_SHADOW { get; }

// NOTE: Collapse -------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor COLLAPSE_TEXT_COLOR { get; }
    public virtual RGBAColor COLLAPSE_ICON_COLOR { get; }
    public virtual RGBAColor COLLAPSE_ICON_BACKGROUND { get; }
    public virtual RGBAColor COLLAPSE_BACKGROUND { get; }
    public virtual string COLLAPSE_FOCUS_BOX_SHADOW { get; }

// NOTE: DropDowns ------------------------------------------------------------------------------------------------------------------------
    // DropDown ---------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor DROPDOWN_OPTIONS_BACKGROUND { get; }
    public virtual RGBAColor DROPDOWN_OPTIONS_DIVIDER_COLOR { get; }
    public virtual string DROPDOWN_OPTIONS_BOX_SHADOW { get; }
    public virtual string DROPDOWN_MARK_BOX_SHADOW { get; }

    // DropDown [highlight]:
    public virtual RGBAColor DROPDOWN_OPTIONS_HIGHLIGHT_BACKGROUND { get; }

    // AdminDropDown ----------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor DROPDOWN_ADMIN_BACKGROUND { get; }
    public virtual RGBAColor DROPDOWN_ADMIN_BORDER_COLOR { get; }
    public virtual string DROPDOWN_ADMIN_BOX_SHADOW { get; }

    // AdminDropDown [highlight]:
    public virtual RGBAColor DROPDOWN_ADMIN_HIGHLIGHT_BORDER_COLOR { get; }
    public virtual string DROPDOWN_ADMIN_HIGHLIGHT_BOX_SHADOW { get; }

    // UserDropDown -----------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor DROPDOWN_USER_BACKGROUND { get; }
    public virtual RGBAColor DROPDOWN_USER_BORDER_COLOR { get; }
    public virtual string DROPDOWN_USER_BOX_SHADOW { get; }

    // AdminDropDown [highlight]:
    public virtual RGBAColor DROPDOWN_USER_HIGHLIGHT_BORDER_COLOR { get; }
    public virtual string DROPDOWN_USER_HIGHLIGHT_BOX_SHADOW { get; }

// NOTE: Forms ----------------------------------------------------------------------------------------------------------------------------
    // Form [primary] ---------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor FORM_PRIMARY_COLOR { get; }
    public virtual RGBAColor FORM_PRIMARY_PLACEHOLDER_COLOR { get; }
    public virtual RGBAColor FORM_PRIMARY_ICON_COLOR { get; }
    public virtual RGBAColor FORM_PRIMARY_DESCRIPTION_COLOR { get; }
    public virtual RGBAColor FORM_PRIMARY_BACKGROUND { get; }
    public virtual RGBAColor FORM_PRIMARY_BORDER_COLOR { get; }
    public virtual string FORM_PRIMARY_TEXT_SHADOW { get; }
    public virtual string FORM_PRIMARY_BOX_SHADOW { get; }

    // Form [primary][highlight]:
    public virtual RGBAColor FORM_PRIMARY_HIGHLIGHT_COLOR { get; }
    public virtual RGBAColor FORM_PRIMARY_HIGHLIGHT_PLACEHOLDER_COLOR { get; }
    public virtual RGBAColor FORM_PRIMARY_HIGHLIGHT_ICON_COLOR { get; }
    public virtual RGBAColor FORM_PRIMARY_HIGHLIGHT_DESCRIPTION_COLOR { get; }
    public virtual RGBAColor FORM_PRIMARY_HIGHLIGHT_BACKGROUND { get; }
    public virtual RGBAColor FORM_PRIMARY_HIGHLIGHT_BORDER_COLOR { get; }
    public virtual string FORM_PRIMARY_HIGHLIGHT_TEXT_SHADOW { get; }
    public virtual string FORM_PRIMARY_HIGHLIGHT_BOX_SHADOW { get; }

    // Form [primary][disabled]:
    public virtual RGBAColor FORM_PRIMARY_DISABLED_COLOR { get; }
    public virtual RGBAColor FORM_PRIMARY_DISABLED_PLACEHOLDER_COLOR { get; }
    public virtual RGBAColor FORM_PRIMARY_DISABLED_ICON_COLOR { get; }
    public virtual RGBAColor FORM_PRIMARY_DISABLED_DESCRIPTION_COLOR { get; }
    public virtual RGBAColor FORM_PRIMARY_DISABLED_BACKGROUND { get; }
    public virtual RGBAColor FORM_PRIMARY_DISABLED_BORDER_COLOR { get; }
    public virtual string FORM_PRIMARY_DISABLED_TEXT_SHADOW { get; }
    public virtual string FORM_PRIMARY_DISABLED_BOX_SHADOW { get; }

    // Form [primary] > Icon [highlight]:
    public virtual RGBAColor FORM_PRIMARY_ICON_HIGHLIGHT_COLOR { get; }

    // Form [primary] > Clear:
    public virtual RGBAColor FORM_PRIMARY_CLEAR_COLOR { get; }
    public virtual RGBAColor FORM_PRIMARY_CLEAR_BACKGROUND { get; }
    public virtual string FORM_PRIMARY_CLEAR_BOX_SHADOW { get; }

    // Form [primary] > Clear [highlight]:
    public virtual RGBAColor FORM_PRIMARY_CLEAR_HIGHLIGHT_COLOR { get; }
    public virtual RGBAColor FORM_PRIMARY_CLEAR_HIGHLIGHT_BACKGROUND { get; }
    public virtual string FORM_PRIMARY_CLEAR_HIGHLIGHT_BOX_SHADOW { get; }

// NOTE: Forms > CheckBoxes ---------------------------------------------------------------------------------------------------------------
    // CheckBox [primary] -----------------------------------------------------------------------------------------------------------------
    // CheckBox [primary][checked]:
    public virtual RGBAColor CHECKBOX_PRIMARY_CHECKED_MARK_COLOR { get; }
    public virtual RGBAColor CHECKBOX_PRIMARY_CHECKED_BACKGROUND { get; }

    // CheckBox [primary][disabled][checked]:
    public virtual RGBAColor CHECKBOX_PRIMARY_DISABLED_CHECKED_MARK_COLOR { get; }
    public virtual RGBAColor CHECKBOX_PRIMARY_DISABLED_CHECKED_BACKGROUND { get; }

// NOTE: Forms > Radios -------------------------------------------------------------------------------------------------------------------
    // Radio [primary] --------------------------------------------------------------------------------------------------------------------
    // Radio [primary][selected]:
    public virtual RGBAColor RADIO_PRIMARY_SELECTED_MARK_COLOR { get; }
    public virtual RGBAColor RADIO_PRIMARY_SELECTED_BACKGROUND { get; }

    // Radio [primary][disabled][selected]:
    public virtual RGBAColor RADIO_PRIMARY_DISABLED_SELECTED_MARK_COLOR { get; }
    public virtual RGBAColor RADIO_PRIMARY_DISABLED_SELECTED_BACKGROUND { get; }

    // RadioButton [primary] --------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor RADIO_BUTTON_PRIMARY_COLOR { get; }
    public virtual RGBAColor RADIO_BUTTON_PRIMARY_BACKGROUND { get; }
    public virtual string RADIO_BUTTON_PRIMARY_BOX_SHADOW { get; }

    // RadioButton [primary][selected]:
    public virtual RGBAColor RADIO_BUTTON_PRIMARY_SELECTED_COLOR { get; }
    public virtual RGBAColor RADIO_BUTTON_PRIMARY_SELECTED_BACKGROUND { get; }
    public virtual string RADIO_BUTTON_PRIMARY_SELECTED_BOX_SHADOW { get; }
    public virtual RGBAColor RADIO_BUTTON_PRIMARY_SELECTED_OUTLINE_COLOR { get; }

    // RadioButton [primary][highlight]:
    public virtual RGBAColor RADIO_BUTTON_PRIMARY_HIGHLIGHT_COLOR { get; }
    public virtual RGBAColor RADIO_BUTTON_PRIMARY_HIGHLIGHT_BACKGROUND { get; }
    public virtual string RADIO_BUTTON_PRIMARY_HIGHLIGHT_BOX_SHADOW { get; }

    // RadioButton [primary][highlight][selected]:
    public virtual RGBAColor RADIO_BUTTON_PRIMARY_HIGHLIGHT_SELECTED_COLOR { get; }
    public virtual RGBAColor RADIO_BUTTON_PRIMARY_HIGHLIGHT_SELECTED_BACKGROUND { get; }
    public virtual string RADIO_BUTTON_PRIMARY_HIGHLIGHT_SELECTED_BOX_SHADOW { get; }
    public virtual RGBAColor RADIO_BUTTON_PRIMARY_HIGHLIGHT_SELECTED_OUTLINE_COLOR { get; }
    
    // RadioButton [primary][disabled]:
    public virtual RGBAColor RADIO_BUTTON_PRIMARY_DISABLED_COLOR { get; }
    public virtual RGBAColor RADIO_BUTTON_PRIMARY_DISABLED_BACKGROUND { get; }
    public virtual string RADIO_BUTTON_PRIMARY_DISABLED_BOX_SHADOW { get; }

    // RadioButton [primary][disabled][selected]:
    public virtual RGBAColor RADIO_BUTTON_PRIMARY_DISABLED_SELECTED_COLOR { get; }
    public virtual RGBAColor RADIO_BUTTON_PRIMARY_DISABLED_SELECTED_BACKGROUND { get; }
    public virtual string RADIO_BUTTON_PRIMARY_DISABLED_SELECTED_BOX_SHADOW { get; }
    public virtual RGBAColor RADIO_BUTTON_PRIMARY_DISABLED_SELECTED_OUTLINE_COLOR { get; }

// NOTE: Forms > Selects ------------------------------------------------------------------------------------------------------------------
    // Select [primary] -------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor SELECT_OPTION_COLOR { get; }
    public virtual RGBAColor SELECT_OPTION_BACKGROUND { get; }
    public virtual RGBAColor SELECT_OPTION_DIVIDER_COLOR { get; }

    // Select [primary][highlight]:
    public virtual RGBAColor SELECT_OPTION_HIGHLIGHT_COLOR { get; }
    public virtual RGBAColor SELECT_OPTION_HIGHLIGHT_BACKGROUND { get; }

    // Select [primary][active]:
    public virtual RGBAColor SELECT_OPTION_ACTIVE_COLOR { get; }
    public virtual RGBAColor SELECT_OPTION_ACTIVE_BACKGROUND { get; }

    // Select [primary][active][highlight]:
    public virtual RGBAColor SELECT_OPTION_ACTIVE_HIGHLIGHT_COLOR { get; }
    public virtual RGBAColor SELECT_OPTION_ACTIVE_HIGHLIGHT_BACKGROUND { get; }

    // SelectCulture ----------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor SELECT_CULTURE_COLOR { get; }
    public virtual RGBAColor SELECT_CULTURE_BACKGROUND { get; }
    public virtual RGBAColor SELECT_CULTURE_BORDER_COLOR { get; }
    public virtual string SELECT_CULTURE_BOX_SHADOW { get; }

    // SelectCulture [highlight]:
    public virtual RGBAColor SELECT_CULTURE_HIGHLIGHT_COLOR { get; }
    public virtual RGBAColor SELECT_CULTURE_HIGHLIGHT_BACKGROUND { get; }
    public virtual RGBAColor SELECT_CULTURE_HIGHLIGHT_BORDER_COLOR { get; }
    public virtual string SELECT_CULTURE_HIGHLIGHT_BOX_SHADOW { get; }

// NOTE: Forms > SelectsMulti -------------------------------------------------------------------------------------------------------------
    // SelectMulti [primary] --------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor SELECT_MULTI_PRIMARY_COUNT_COLOR { get; }
    public virtual RGBAColor SELECT_MULTI_PRIMARY_COUNT_BACKGROUND { get; }
    public virtual string SELECT_MULTI_PRIMARY_COUNT_TEXT_SHADOW { get; }
    public virtual string SELECT_MULTI_PRIMARY_COUNT_BOX_SHADOW { get; }
    public virtual RGBAColor SELECT_MULTI_PRIMARY_COUNT_PLUS_COLOR { get; }
    public virtual string SELECT_MULTI_PRIMARY_COUNT_PLUS_TEXT_SHADOW { get; }

    // SelectMulti [primary][disabled]:
    public virtual RGBAColor SELECT_MULTI_PRIMARY_DISABLED_COUNT_COLOR { get; }
    public virtual RGBAColor SELECT_MULTI_PRIMARY_DISABLED_COUNT_BACKGROUND { get; }
    public virtual string SELECT_MULTI_PRIMARY_DISABLED_COUNT_TEXT_SHADOW { get; }
    public virtual string SELECT_MULTI_PRIMARY_DISABLED_COUNT_BOX_SHADOW { get; }
    public virtual RGBAColor SELECT_MULTI_PRIMARY_DISABLED_COUNT_PLUS_COLOR { get; }
    public virtual string SELECT_MULTI_PRIMARY_DISABLED_COUNT_PLUS_TEXT_SHADOW { get; }

// NOTE: Forms > Switches -----------------------------------------------------------------------------------------------------------------
    // Switch [primary] -------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor SWITCH_PRIMARY_BACKGROUND { get; }
    public virtual RGBAColor SWITCH_PRIMARY_BULLET_BACKGROUND { get; }

    // Switch [primary][checked]:
    public virtual RGBAColor SWITCH_PRIMARY_CHECKED_BACKGROUND { get; }

    // Switch [primary][focus]:
    public virtual RGBAColor SWITCH_PRIMARY_FOCUS_OUTLINE_COLOR { get; }
    public virtual string SWITCH_PRIMARY_FOCUS_OUTLINE_SHADOW { get; }

    // Switch [primary][disabled]:
    public virtual RGBAColor SWITCH_PRIMARY_DISABLED_BACKGROUND { get; }
    public virtual RGBAColor SWITCH_PRIMARY_DISABLED_BULLET_BACKGROUND { get; }

    // Switch [primary][disabled][checked]:
    public virtual RGBAColor SWITCH_PRIMARY_DISABLED_CHECKED_BACKGROUND { get; }

// NOTE: Images ---------------------------------------------------------------------------------------------------------------------------
    // Background -------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor BACKGROUND_LIGHT_COLOR { get; }
    public virtual RGBAColor BACKGROUND_DARK_COLOR { get; }

    // Image ------------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor IMAGE_LIGHT_COLOR { get; }
    public virtual RGBAColor IMAGE_DARK_COLOR { get; }
    public virtual RGBAColor IMAGE_ERROR_COLOR { get; }
    public virtual RGBAColor IMAGE_ICON_COLOR { get; }

// NOTE: Links ----------------------------------------------------------------------------------------------------------------------------
    // LogoLink ---------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor LOGO_LINK_COLOR { get; }
    public virtual RGBAColor LOGO_LINK_BACKGROUND { get; }
    public virtual string LOGO_LINK_BOX_SHADOW { get; }

    // LogoLink [focus]:
    public virtual RGBAColor LOGO_LINK_FOCUS_COLOR { get; }
    public virtual RGBAColor LOGO_LINK_FOCUS_BACKGROUND { get; }
    public virtual string LOGO_LINK_FOCUS_BOX_SHADOW { get; }

// NOTE: Loaders --------------------------------------------------------------------------------------------------------------------------
    // Loader -----------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor LOADER_COLOR { get; }
    public virtual RGBAColor LOADER_BACKGROUND { get; }

    // PageLoader -------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor PAGE_LOADER_COLOR { get; }
    public virtual RGBAColor PAGE_LOADER_BACKGROUND { get; }

    // ServerPageLoader -------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor SERVER_PAGE_LOADER_TEXT_COLOR { get; }
    public virtual RGBAColor SERVER_PAGE_LOADER_COLOR { get; }
    public virtual RGBAColor SERVER_PAGE_LOADER_BACKGROUND { get; }
    public virtual RGBAColor SERVER_PAGE_LOADER_BACKDROP { get; }

// NOTE: Modals ---------------------------------------------------------------------------------------------------------------------------
    // Modal ------------------------------------------------------------------------------------------------------------------------------
    public virtual string MODAL_DIALOG_BOX_SHADOW { get; }
    public virtual string MODAL_ENDING_BOX_SHADOW { get; }
    
    // Modal control:
    public virtual RGBAColor MODAL_CONTROL_COLOR { get; }
    public virtual RGBAColor MODAL_CONTROL_BACKGROUND { get; }
    public virtual string MODAL_CONTROL_BOX_SHADOW { get; }

    // Modal control [highlight]:
    public virtual RGBAColor MODAL_CONTROL_HIGHLIGHT_COLOR { get; }
    public virtual RGBAColor MODAL_CONTROL_HIGHLIGHT_BACKGROUND { get; }
    public virtual string MODAL_CONTROL_HIGHLIGHT_BOX_SHADOW { get; }
    
    // CookieModal ------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor MODAL_COOKIE_BUTTON_COLOR { get; }

    // CookieModal [highlight]:
    public virtual RGBAColor MODAL_COOKIE_BUTTON_HIGHLIGHT_COLOR { get; }
    public virtual string MODAL_COOKIE_BUTTON_HIGHLIGHT_TEXT_SHADOW { get; }
    
    // ProfileModal -----------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor MODAL_PROFILE_AVATAR_BACKGROUND { get; }
    public virtual RGBAColor MODAL_PROFILE_AVATAR_BORDER_COLOR { get; }
    public virtual string MODAL_PROFILE_AVATAR_BOX_SHADOW { get; }

// NOTE: Notification ---------------------------------------------------------------------------------------------------------------------
    public virtual string NOTIFICATION_BOX_SHADOW { get; }

    // Notification control:
    public virtual RGBAColor NOTIFICATION_CONTROL_COLOR { get; }
    
    // Notification control [highlight]:
    public virtual RGBAColor NOTIFICATION_CONTROL_HIGHLIGHT_COLOR { get; }

// NOTE: Progress -------------------------------------------------------------------------------------------------------------------------
    // ProgressCircle ---------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor PROGRESS_CIRCLE_COLOR { get; }
    public virtual RGBAColor PROGRESS_CIRCLE_BACKGROUND { get; }

// NOTE: Text -----------------------------------------------------------------------------------------------------------------------------
    // Text -------------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor TEXT_COLOR { get; }

    // Text [highlight]:
    public virtual RGBAColor TEXT_HIGHLIGHT_COLOR { get; }

    // Text Accent ------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor TEXT_ACCENT_COLOR { get; }

    // Text Accent [highlight]:
    public virtual RGBAColor TEXT_ACCENT_HIGHLIGHT_COLOR { get; }

    // Text [disabled] --------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor TEXT_DISABLED_COLOR { get; }
}

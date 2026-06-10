namespace Jumpeno.Client.Constants;

#pragma warning disable CS8618
#pragma warning disable CA1822

public class BaseTheme {
// NOTE: Constants ------------------------------------------------------------------------------------------------------------------------
    // Fonts ------------------------------------------------------------------------------------------------------------------------------
    public string FontPrimary => "Montserrat, sans-serif";

    // Sizes (px) -------------------------------------------------------------------------------------------------------------------------
    public int SizeContainerMaxWidth => 1340;
    public int SizeContainerPaddingMobile => 16;
    public int SizeContainerPaddingTablet => 28;
    public int SizeContainerPaddingDesktop => 40;
    public int SizeHeaderHeight => 75;
    public int SizeFooterHeightMobile => 180;
    public int SizeFooterHeightTablet => 190;
    public int SizeFooterHeightDesktop => 210;

    // Transitions (ms) -------------------------------------------------------------------------------------------------------------------
    public int TransitionBolt => 0;
    public int TransitionSemiBolt => 50;
    public int TransitionUltraFast => 100;
    public int TransitionSemiUltraFast => 150;
    public int TransitionFast => 200;
    public int TransitionSemiFast => 250;
    public int TransitionNormal => 300;
    public int TransitionSemiSlow => 350;
    public int TransitionSlow => 400;
    public int TransitionSemiExtraSlow => 450;
    public int TransitionExtraSlow => 500;

    // Z-index ----------------------------------------------------------------------------------------------------------------------------
    public int ZIndexFormError => 100;
    public int ZIndexDropdown => 1000;
    public int ZIndexMenu => 1001;
    public int ZIndexModal => 1002;
    public int ZIndexPageLoader => 1000000;
    public int ZIndexServerPageLoader => 1000001;
    public int ZIndexNotification => 1000002;
    public int ZIndexConsoleUi => 1000003;

    // Breakpoints (px) -------------------------------------------------------------------------------------------------------------------
    public double BreakpointMobileSm => 480;
    public double BreakpointMobile => 768;
    public double BreakpointTabletSm => 992;
    public double BreakpointTablet => 1200;

    // Email ------------------------------------------------------------------------------------------------------------------------------
    public string EmailTitleFont => "Arial";
    public string EmailTextFont => "Helvetica";
    public string EmailButtonFont => "Arial";
    // Email > Text:
    public RGBColor EmailBackground => new(255, 255, 255);
    public RGBColor EmailTextColor => new(0, 0, 0);
    // Email > Button:
    public RGBColor EmailButtonColor => new(0, 0, 0);
    public RGBColor EmailButtonBackground => new(255, 215, 0);
    public RGBColor EmailButtonBackgroundHighlight => new(255, 239, 0);
    
// NOTE: Surface --------------------------------------------------------------------------------------------------------------------------
    // Primary ----------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor SurfaceBackground_SurfacePrimary { get; }
    public virtual RGBAColor SurfaceBackground_SurfacePrimaryCollapse { get; }
    public virtual RGBAColor SurfaceBackground_SurfacePrimaryBox { get; }
    public virtual RGBAColor SurfaceBackground_SurfacePrimaryBoxCollapse { get; }
    public virtual RGBAColor SurfaceBackground_SurfacePrimaryTransparent { get; }
    public virtual RGBAColor SurfaceBackground_SurfacePrimaryTransparentCollapse { get; }
    public virtual RGBAColor SurfaceBackground_SurfacePrimaryGlass { get; }
    public virtual RGBAColor SurfaceBackground_SurfacePrimaryGlassCollapse { get; }

    // Secondary --------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor SurfaceBackground_SurfaceSecondary { get; }

    // Floating ---------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor SurfaceBackground_SurfaceFloating { get; }
    public virtual RGBAColor SurfaceBackground_SurfaceFloatingCollapse { get; }
    public virtual RGBAColor SurfaceBackground_SurfaceFloatingAdditional { get; }
    public virtual RGBAColor SurfaceBackground_SurfaceFloatingAdditionalCollapse { get; }

// NOTE: Status ---------------------------------------------------------------------------------------------------------------------------
    // Danger -----------------------------------------------------------------------------------------------------------------------------
    // Box:
    public virtual RGBAColor StatusDangerBoxColor { get; }
    public virtual RGBAColor StatusDangerBoxBackground { get; }
    public virtual RGBAColor StatusDangerBoxOutlineColor { get; }
    // Box [highlight]:
    public virtual RGBAColor StatusDangerBoxHighlightColor { get; }
    public virtual RGBAColor StatusDangerBoxHighlightBackground { get; }
    public virtual RGBAColor StatusDangerBoxHighlightOutlineColor { get; }
    
    // Accent:
    public virtual RGBAColor StatusDangerAccentColor { get; }
    // Accent [highlight]:
    public virtual RGBAColor StatusDangerAccentHighlightColor { get; }
    
    // Neon:
    public virtual RGBAColor StatusDangerNeonColor { get; }

    // Success ----------------------------------------------------------------------------------------------------------------------------
    // Box:
    public virtual RGBAColor StatusSuccessBoxColor { get; }
    public virtual RGBAColor StatusSuccessBoxBackground { get; }
    public virtual RGBAColor StatusSuccessBoxOutlineColor { get; }
    // Box [highlight]:
    public virtual RGBAColor StatusSuccessBoxHighlightColor { get; }
    public virtual RGBAColor StatusSuccessBoxHighlightBackground { get; }
    public virtual RGBAColor StatusSuccessBoxHighlightOutlineColor { get; }

    // Accent:
    public virtual RGBAColor StatusSuccessAccentColor { get; }
    // Accent [highlight]:
    public virtual RGBAColor StatusSuccessAccentHighlightColor { get; }

    // Neon:
    public virtual RGBAColor StatusSuccessNeonColor { get; }

    // Warning ----------------------------------------------------------------------------------------------------------------------------
    // Box:
    public virtual RGBAColor StatusWarningBoxColor { get; }
    public virtual RGBAColor StatusWarningBoxBackground { get; }
    public virtual RGBAColor StatusWarningBoxOutlineColor { get; }
    // Box [highlight]:
    public virtual RGBAColor StatusWarningBoxHighlightColor { get; }
    public virtual RGBAColor StatusWarningBoxHighlightBackground { get; }
    public virtual RGBAColor StatusWarningBoxHighlightOutlineColor { get; }

    // Accent:
    public virtual RGBAColor StatusWarningAccentColor { get; }
    // Accent [highlight]:
    public virtual RGBAColor StatusWarningAccentHighlightColor { get; }

    // Neon:
    public virtual RGBAColor StatusWarningNeonColor { get; }

    // Info -------------------------------------------------------------------------------------------------------------------------------
    // Box:
    public virtual RGBAColor StatusInfoBoxColor { get; }
    public virtual RGBAColor StatusInfoBoxBackground { get; }
    public virtual RGBAColor StatusInfoBoxOutlineColor { get; }
    // Box [highlight]:
    public virtual RGBAColor StatusInfoBoxHighlightColor { get; }
    public virtual RGBAColor StatusInfoBoxHighlightBackground { get; }
    public virtual RGBAColor StatusInfoBoxHighlightOutlineColor { get; }

    // Accent:
    public virtual RGBAColor StatusInfoAccentColor { get; }
    // Accent [highlight]:
    public virtual RGBAColor StatusInfoAccentHighlightColor { get; }

    // Neon:
    public virtual RGBAColor StatusInfoNeonColor { get; }

// NOTE: Layout ---------------------------------------------------------------------------------------------------------------------------
    // Body -------------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor BodyBackground { get; }
    // Scrollbars:
    public virtual ScrollAreaTheme BodyScrollTheme { get; }
    // Selection:
    public virtual RGBAColor BodySelectionColor { get; }
    public virtual RGBAColor BodySelectionBackground { get; }
    // Backdrop:
    public virtual RGBAColor BodyBackdrop { get; }

    // NavMenu ----------------------------------------------------------------------------------------------------------------------------
    public virtual string NavMenuBoxShadow { get; }

    // NavMenu [mobile]:
    public virtual RGBAColor NavMenuMobileButtonColor { get; }
    public virtual ScrollAreaTheme NavMenuMobileScrollTheme { get; }

    // NavMenu [mobile][highlight]:
    public virtual RGBAColor NavMenuMobileButtonHighlightColor { get; }

    // NavMenu [mobile][focus]:
    public virtual RGBAColor NavMenuMobileButtonFocusBackground { get; }
    public virtual string NavMenuMobileButtonFocusBoxShadow { get; }
    
    // QR code ----------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor QrCodeBackground { get; }

// NOTE: Pages ----------------------------------------------------------------------------------------------------------------------------
    // Game -------------------------------------------------------------------------------------------------------------------------------
    // Game > Atoms > GameState:
    public virtual RGBAColor GameStateColor => new(255, 255, 255);
    public virtual string GameStateTextShadow => "0.09em 0.045em 0.24em rgba(0, 0, 0, 0.8)";

    // Game > Atoms > GameWaiting:
    public virtual RGBColor GameWaitingBackground => new(61, 61, 61);
    public virtual string GameWaitingDropShadow => "0 0.043em 0.086em rgba(0, 0, 0, 0.8)";

    // Game > Components > CreateBox:
    public virtual RGBAColor CreateBoxSettingsIconColor => new(255, 255, 255);
    public virtual string CreateBoxSettingsIconDropShadow => "0 0 0.03em rgba(0, 0, 0, 0.6)";
    public virtual string CreateBoxLogoDropShadow => "0 0 2em rgba(0, 0, 0, 0.6)";
    public virtual RGBAColor CreateBoxMapIconColor => new(210, 210, 150);
    public virtual string CreateBoxCanvasBoxShadowSize => "0 0.009em 0.04em 0.014em";
    public virtual string CreateBoxCanvasBoxShadowOpacity { get; }

    // Game > Components > GameScreen:
    // Control:
    public virtual RGBAColor GameScreenControlColor { get; }
    public virtual RGBAColor GameScreenControlBackground { get; }
    public virtual string GameScreenControlBoxShadow { get; }
    // Control [pressed]:
    public virtual RGBAColor GameScreenControlPressedColor { get; }
    public virtual RGBAColor GameScreenControlPressedBackground { get; }
    public virtual string GameScreenControlPressedBoxShadow { get; }

    // Game > Components > Lobby:
    public virtual string LobbyBoxShadow { get; }
    public virtual RGBAColor LobbyEmptyColor { get; }
    // Players:
    public virtual RGBColor LobbyPlayerSettingsColor => new(65, 65, 65);
    public virtual RGBAColor LobbyLineBackground { get; }
    public virtual string LobbyPresenceBoxShadow { get; }
    public virtual RGBAColor LobbyDashColor { get; }

    // Manual -----------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor ManualColor { get; }
    public virtual string ManualTextShadow { get; }
    public virtual RGBAColor ManualBackground { get; }
    public virtual RGBAColor ManualBackgroundTransition { get; }
    public virtual string ManualBoxShadow { get; }

// NOTE: Box ------------------------------------------------------------------------------------------------------------------------------
    // Box [box] --------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor BoxBackground_SurfacePrimaryBox { get; }
    public virtual string BoxBoxShadow_SurfacePrimaryBox { get; }

    // Box [transparent] ------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor BoxBackground_SurfacePrimaryTransparent { get; }
    public virtual string BoxBoxShadow_SurfacePrimaryTransparent { get; }

    // Box [glass] ------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor BoxBackground_SurfacePrimaryGlass { get; }
    public virtual string BoxBoxShadow_SurfacePrimaryGlass { get; }

    // Box > BoxHeader --------------------------------------------------------------------------------------------------------------------
    public virtual string BoxHeaderBackground => "linear-gradient(45deg, rgba(255, 255, 255, 0.5), transparent)";
    public virtual string BoxHeaderBoxShadow => "0 2em 10em -2em rgba(0, 0, 0, 0.05)";

// NOTE: Buttons --------------------------------------------------------------------------------------------------------------------------
    // Button [primary] -------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor ButtonPrimaryColor { get; }
    public virtual RGBAColor ButtonPrimaryBackground { get; }
    public virtual string ButtonPrimaryBoxShadow { get; }

    // Button [primary][highlight]:
    public virtual RGBAColor ButtonPrimaryHighlightColor { get; }
    public virtual RGBAColor ButtonPrimaryHighlightBackground { get; }
    public virtual string ButtonPrimaryHighlightBoxShadow { get; }

    // Button [secondary] -----------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor ButtonSecondaryColor { get; }
    public virtual RGBAColor ButtonSecondaryBackground { get; }
    public virtual string ButtonSecondaryBoxShadow { get; }

    // Button [secondary][highlight]:
    public virtual RGBAColor ButtonSecondaryHighlightColor { get; }
    public virtual RGBAColor ButtonSecondaryHighlightBackground { get; }
    public virtual string ButtonSecondaryHighlightBoxShadow { get; }

    // Button [tertiary] ------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor ButtonTertiaryColor { get; }
    public virtual RGBAColor ButtonTertiaryBackground { get; }
    public virtual string ButtonTertiaryBoxShadow { get; }

    // Button [tertiary][highlight]:
    public virtual RGBAColor ButtonTertiaryHighlightColor { get; }
    public virtual RGBAColor ButtonTertiaryHighlightBackground { get; }
    public virtual string ButtonTertiaryHighlightBoxShadow { get; }

    // Button [quaternary] ----------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor ButtonQuaternaryColor { get; }
    public virtual RGBAColor ButtonQuaternaryBackground { get; }
    public virtual string ButtonQuaternaryBoxShadow { get; }

    // Button [quaternary][highlight]:
    public virtual RGBAColor ButtonQuaternaryHighlightColor { get; }
    public virtual RGBAColor ButtonQuaternaryHighlightBackground { get; }
    public virtual string ButtonQuaternaryHighlightBoxShadow { get; }

    // Button [success] -------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor ButtonSuccessColor => new(255, 255, 255);
    public virtual string ButtonSuccessBackground => "linear-gradient(rgb(159 221 1) 00%, #6b9700 90%)";
    public virtual RGBColor ButtonSuccessTintColor => new(89, 184, 0);
    public virtual string ButtonSuccessBoxShadow => "0.8em 3em 4em rgba(0, 0, 0, 0.55)";

    // Button [success][highlight]:
    public virtual RGBAColor ButtonSuccessHighlightColor => new(255, 255, 255);
    public virtual string ButtonSuccessHighlightBackground => "linear-gradient(rgb(166 231 0) 0%, #7caf00 90%)";
    public virtual RGBColor ButtonSuccessHighlightTintColor => new(89, 184, 0);
    public virtual string ButtonSuccessHighlightBoxShadow => "0.8em 3em 4em rgba(0, 0, 0, 0.55)";

    // Button [danger] --------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor ButtonDangerColor { get; }
    public virtual RGBAColor ButtonDangerBackground { get; }
    public virtual string ButtonDangerBoxShadow { get; }

    // Button [danger][highlight]:
    public virtual RGBAColor ButtonDangerHighlightColor { get; }
    public virtual RGBAColor ButtonDangerHighlightBackground { get; }
    public virtual string ButtonDangerHighlightBoxShadow { get; }

    // Button [disabled] ------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor ButtonDisabledColor { get; }
    public virtual RGBAColor ButtonDisabledBackground { get; }
    public virtual string ButtonDisabledBoxShadow { get; }

    // MenuButton -------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor MenuButtonColor { get; }
    public virtual RGBAColor MenuButtonBackground { get; }
    public virtual string MenuButtonBoxShadow { get; }

    // MenuButton [highlight]:
    public virtual RGBAColor MenuButtonHighlightColor { get; }
    public virtual RGBAColor MenuButtonHighlightBackground { get; }
    public virtual string MenuButtonHighlightBoxShadow { get; }

    // MenuButton [active]:
    public virtual RGBAColor MenuButtonActiveColor { get; }
    public virtual RGBAColor MenuButtonActiveBackground { get; }
    public virtual string MenuButtonActiveBoxShadow { get; }

    // MenuButton [active][highlight]:
    public virtual RGBAColor MenuButtonActiveHighlightColor { get; }
    public virtual RGBAColor MenuButtonActiveHighlightBackground { get; }
    public virtual string MenuButtonActiveHighlightBoxShadow { get; }

    // MenuButton [mobile] ----------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor MenuButtonMobileColor { get; }
    public virtual RGBAColor MenuButtonMobileBackground { get; }
    public virtual string MenuButtonMobileBoxShadow { get; }

    // MenuButton [mobile][hover]:
    public virtual RGBAColor MenuButtonMobileHoverColor { get; }
    public virtual RGBAColor MenuButtonMobileHoverBackground { get; }
    public virtual string MenuButtonMobileHoverBoxShadow { get; }

    // MenuButton [mobile][focus]:
    public virtual RGBAColor MenuButtonMobileFocusColor { get; }
    public virtual RGBAColor MenuButtonMobileFocusBackground { get; }
    public virtual string MenuButtonMobileFocusBoxShadow { get; }

    // MenuButton [mobile][active]:
    public virtual RGBAColor MenuButtonMobileActiveColor { get; }
    public virtual RGBAColor MenuButtonMobileActiveBackground { get; }
    public virtual string MenuButtonMobileActiveBoxShadow { get; }

    // MenuButton [mobile][active][hover]:
    public virtual RGBAColor MenuButtonMobileActiveHoverColor { get; }
    public virtual RGBAColor MenuButtonMobileActiveHoverBackground { get; }
    public virtual string MenuButtonMobileActiveHoverBoxShadow { get; }

    // MenuButton [mobile][active][focus]:
    public virtual RGBAColor MenuButtonMobileActiveFocusColor { get; }
    public virtual RGBAColor MenuButtonMobileActiveFocusBackground { get; }
    public virtual string MenuButtonMobileActiveFocusBoxShadow { get; }

// NOTE: Collapse -------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor CollapseTextColor { get; }
    public virtual RGBAColor CollapseIconColor { get; }
    public virtual RGBAColor CollapseIconBackground { get; }
    public virtual RGBAColor CollapseBackground { get; }
    public virtual string CollapseFocusBoxShadow { get; }

// NOTE: DropDowns ------------------------------------------------------------------------------------------------------------------------
    // DropDown ---------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor DropdownOptionsBackground { get; }
    public virtual RGBAColor DropdownOptionsDividerColor { get; }
    public virtual string DropdownOptionsBoxShadow { get; }
    public virtual string DropdownMarkBoxShadow { get; }

    // DropDown [highlight]:
    public virtual RGBAColor DropdownOptionsHighlightBackground { get; }

    // AdminDropDown ----------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor DropdownAdminBackground { get; }
    public virtual RGBAColor DropdownAdminBorderColor { get; }
    public virtual string DropdownAdminBoxShadow { get; }

    // AdminDropDown [highlight]:
    public virtual RGBAColor DropdownAdminHighlightBorderColor { get; }
    public virtual string DropdownAdminHighlightBoxShadow { get; }

    // UserDropDown -----------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor DropdownUserBackground { get; }
    public virtual RGBAColor DropdownUserBorderColor { get; }
    public virtual string DropdownUserBoxShadow { get; }

    // AdminDropDown [highlight]:
    public virtual RGBAColor DropdownUserHighlightBorderColor { get; }
    public virtual string DropdownUserHighlightBoxShadow { get; }

// NOTE: Forms ----------------------------------------------------------------------------------------------------------------------------
    // Form [primary] ---------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor FormPrimaryColor { get; }
    public virtual RGBAColor FormPrimaryPlaceholderColor { get; }
    public virtual RGBAColor FormPrimaryIconColor { get; }
    public virtual RGBAColor FormPrimaryDescriptionColor { get; }
    public virtual RGBAColor FormPrimaryBackground { get; }
    public virtual RGBAColor FormPrimaryBorderColor { get; }
    public virtual string FormPrimaryTextShadow { get; }
    public virtual string FormPrimaryBoxShadow { get; }

    // Form [primary][highlight]:
    public virtual RGBAColor FormPrimaryHighlightColor { get; }
    public virtual RGBAColor FormPrimaryHighlightPlaceholderColor { get; }
    public virtual RGBAColor FormPrimaryHighlightIconColor { get; }
    public virtual RGBAColor FormPrimaryHighlightDescriptionColor { get; }
    public virtual RGBAColor FormPrimaryHighlightBackground { get; }
    public virtual RGBAColor FormPrimaryHighlightBorderColor { get; }
    public virtual string FormPrimaryHighlightTextShadow { get; }
    public virtual string FormPrimaryHighlightBoxShadow { get; }

    // Form [primary][disabled]:
    public virtual RGBAColor FormPrimaryDisabledColor { get; }
    public virtual RGBAColor FormPrimaryDisabledPlaceholderColor { get; }
    public virtual RGBAColor FormPrimaryDisabledIconColor { get; }
    public virtual RGBAColor FormPrimaryDisabledDescriptionColor { get; }
    public virtual RGBAColor FormPrimaryDisabledBackground { get; }
    public virtual RGBAColor FormPrimaryDisabledBorderColor { get; }
    public virtual string FormPrimaryDisabledTextShadow { get; }
    public virtual string FormPrimaryDisabledBoxShadow { get; }

    // Form [primary] > Icon [highlight]:
    public virtual RGBAColor FormPrimaryIconHighlightColor { get; }

    // Form [primary] > Clear:
    public virtual RGBAColor FormPrimaryClearColor { get; }
    public virtual RGBAColor FormPrimaryClearBackground { get; }
    public virtual string FormPrimaryClearBoxShadow { get; }

    // Form [primary] > Clear [highlight]:
    public virtual RGBAColor FormPrimaryClearHighlightColor { get; }
    public virtual RGBAColor FormPrimaryClearHighlightBackground { get; }
    public virtual string FormPrimaryClearHighlightBoxShadow { get; }

// NOTE: Forms > CheckBoxes ---------------------------------------------------------------------------------------------------------------
    // CheckBox [primary] -----------------------------------------------------------------------------------------------------------------
    // CheckBox [primary][checked]:
    public virtual RGBAColor CheckboxPrimaryCheckedMarkColor { get; }
    public virtual RGBAColor CheckboxPrimaryCheckedBackground { get; }

    // CheckBox [primary][disabled][checked]:
    public virtual RGBAColor CheckboxPrimaryDisabledCheckedMarkColor { get; }
    public virtual RGBAColor CheckboxPrimaryDisabledCheckedBackground { get; }

// NOTE: Forms > Radios -------------------------------------------------------------------------------------------------------------------
    // Radio [primary] --------------------------------------------------------------------------------------------------------------------
    // Radio [primary][selected]:
    public virtual RGBAColor RadioPrimarySelectedMarkColor { get; }
    public virtual RGBAColor RadioPrimarySelectedBackground { get; }

    // Radio [primary][disabled][selected]:
    public virtual RGBAColor RadioPrimaryDisabledSelectedMarkColor { get; }
    public virtual RGBAColor RadioPrimaryDisabledSelectedBackground { get; }

    // RadioButton [primary] --------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor RadioButtonPrimaryColor { get; }
    public virtual RGBAColor RadioButtonPrimaryBackground { get; }
    public virtual string RadioButtonPrimaryBoxShadow { get; }

    // RadioButton [primary][selected]:
    public virtual RGBAColor RadioButtonPrimarySelectedColor { get; }
    public virtual RGBAColor RadioButtonPrimarySelectedBackground { get; }
    public virtual string RadioButtonPrimarySelectedBoxShadow { get; }
    public virtual RGBAColor RadioButtonPrimarySelectedOutlineColor { get; }

    // RadioButton [primary][highlight]:
    public virtual RGBAColor RadioButtonPrimaryHighlightColor { get; }
    public virtual RGBAColor RadioButtonPrimaryHighlightBackground { get; }
    public virtual string RadioButtonPrimaryHighlightBoxShadow { get; }

    // RadioButton [primary][highlight][selected]:
    public virtual RGBAColor RadioButtonPrimaryHighlightSelectedColor { get; }
    public virtual RGBAColor RadioButtonPrimaryHighlightSelectedBackground { get; }
    public virtual string RadioButtonPrimaryHighlightSelectedBoxShadow { get; }
    public virtual RGBAColor RadioButtonPrimaryHighlightSelectedOutlineColor { get; }
    
    // RadioButton [primary][disabled]:
    public virtual RGBAColor RadioButtonPrimaryDisabledColor { get; }
    public virtual RGBAColor RadioButtonPrimaryDisabledBackground { get; }
    public virtual string RadioButtonPrimaryDisabledBoxShadow { get; }

    // RadioButton [primary][disabled][selected]:
    public virtual RGBAColor RadioButtonPrimaryDisabledSelectedColor { get; }
    public virtual RGBAColor RadioButtonPrimaryDisabledSelectedBackground { get; }
    public virtual string RadioButtonPrimaryDisabledSelectedBoxShadow { get; }
    public virtual RGBAColor RadioButtonPrimaryDisabledSelectedOutlineColor { get; }

// NOTE: Forms > Selects ------------------------------------------------------------------------------------------------------------------
    // Select [primary] -------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor SelectOptionColor { get; }
    public virtual RGBAColor SelectOptionBackground { get; }
    public virtual RGBAColor SelectOptionDividerColor { get; }

    // Select [primary][highlight]:
    public virtual RGBAColor SelectOptionHighlightColor { get; }
    public virtual RGBAColor SelectOptionHighlightBackground { get; }

    // Select [primary][active]:
    public virtual RGBAColor SelectOptionActiveColor { get; }
    public virtual RGBAColor SelectOptionActiveBackground { get; }

    // Select [primary][active][highlight]:
    public virtual RGBAColor SelectOptionActiveHighlightColor { get; }
    public virtual RGBAColor SelectOptionActiveHighlightBackground { get; }

    // SelectCulture ----------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor SelectCultureColor { get; }
    public virtual RGBAColor SelectCultureBackground { get; }
    public virtual RGBAColor SelectCultureBorderColor { get; }
    public virtual string SelectCultureBoxShadow { get; }

    // SelectCulture [highlight]:
    public virtual RGBAColor SelectCultureHighlightColor { get; }
    public virtual RGBAColor SelectCultureHighlightBackground { get; }
    public virtual RGBAColor SelectCultureHighlightBorderColor { get; }
    public virtual string SelectCultureHighlightBoxShadow { get; }

// NOTE: Forms > SelectsMulti -------------------------------------------------------------------------------------------------------------
    // SelectMulti [primary] --------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor SelectMultiPrimaryCountColor { get; }
    public virtual RGBAColor SelectMultiPrimaryCountBackground { get; }
    public virtual string SelectMultiPrimaryCountTextShadow { get; }
    public virtual string SelectMultiPrimaryCountBoxShadow { get; }
    public virtual RGBAColor SelectMultiPrimaryCountPlusColor { get; }
    public virtual string SelectMultiPrimaryCountPlusTextShadow { get; }

    // SelectMulti [primary][disabled]:
    public virtual RGBAColor SelectMultiPrimaryDisabledCountColor { get; }
    public virtual RGBAColor SelectMultiPrimaryDisabledCountBackground { get; }
    public virtual string SelectMultiPrimaryDisabledCountTextShadow { get; }
    public virtual string SelectMultiPrimaryDisabledCountBoxShadow { get; }
    public virtual RGBAColor SelectMultiPrimaryDisabledCountPlusColor { get; }
    public virtual string SelectMultiPrimaryDisabledCountPlusTextShadow { get; }

// NOTE: Forms > Switches -----------------------------------------------------------------------------------------------------------------
    // Switch [primary] -------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor SwitchPrimaryBackground { get; }
    public virtual RGBAColor SwitchPrimaryBulletBackground { get; }

    // Switch [primary][checked]:
    public virtual RGBAColor SwitchPrimaryCheckedBackground { get; }

    // Switch [primary][focus]:
    public virtual RGBAColor SwitchPrimaryFocusOutlineColor { get; }
    public virtual string SwitchPrimaryFocusOutlineShadow { get; }

    // Switch [primary][disabled]:
    public virtual RGBAColor SwitchPrimaryDisabledBackground { get; }
    public virtual RGBAColor SwitchPrimaryDisabledBulletBackground { get; }

    // Switch [primary][disabled][checked]:
    public virtual RGBAColor SwitchPrimaryDisabledCheckedBackground { get; }

// NOTE: GameCanvas -----------------------------------------------------------------------------------------------------------------------
    public virtual RGBColor GameCanvasDefaultBackground { get; }
    public virtual RGBColor GameCanvasDefaultForeground { get; }
    public virtual RGBColor GameCanvasDefaultTint { get; }
    public virtual RGBColor GameCanvasDefaultBorder { get; }
    public virtual string GameCanvasBoxShadowSize => "0 0.0025em 0.025em 0.01em";
    public virtual string GameCanvasBoxShadowOpacity { get; }

// NOTE: Images ---------------------------------------------------------------------------------------------------------------------------
    // Background -------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor BackgroundLightColor { get; }
    public virtual RGBAColor BackgroundDarkColor { get; }

    // Image ------------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor ImageLightColor { get; }
    public virtual RGBAColor ImageDarkColor { get; }
    public virtual RGBAColor ImageErrorColor { get; }
    public virtual RGBAColor ImageIconColor { get; }

// NOTE: Links ----------------------------------------------------------------------------------------------------------------------------
    // LogoLink ---------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor LogoLinkColor { get; }
    public virtual RGBAColor LogoLinkBackground { get; }
    public virtual string LogoLinkBoxShadow { get; }

    // LogoLink [focus]:
    public virtual RGBAColor LogoLinkFocusColor { get; }
    public virtual RGBAColor LogoLinkFocusBackground { get; }
    public virtual string LogoLinkFocusBoxShadow { get; }

// NOTE: Loaders --------------------------------------------------------------------------------------------------------------------------
    // Loader -----------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor LoaderColor { get; }
    public virtual RGBAColor LoaderBackground { get; }

    // PageLoader -------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor PageLoaderColor { get; }
    public virtual RGBAColor PageLoaderBackground { get; }

    // ServerPageLoader -------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor ServerPageLoaderTextColor { get; }
    public virtual RGBAColor ServerPageLoaderColor { get; }
    public virtual RGBAColor ServerPageLoaderBackground { get; }
    public virtual RGBAColor ServerPageLoaderBackdrop { get; }

// NOTE: Modals ---------------------------------------------------------------------------------------------------------------------------
    // Modal ------------------------------------------------------------------------------------------------------------------------------
    public virtual string ModalDialogBoxShadow { get; }
    public virtual string ModalEndingBoxShadow { get; }
    
    // Modal control:
    public virtual RGBAColor ModalControlColor { get; }
    public virtual RGBAColor ModalControlBackground { get; }
    public virtual string ModalControlBoxShadow { get; }

    // Modal control [highlight]:
    public virtual RGBAColor ModalControlHighlightColor { get; }
    public virtual RGBAColor ModalControlHighlightBackground { get; }
    public virtual string ModalControlHighlightBoxShadow { get; }
    
    // CookieModal ------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor ModalCookieButtonColor { get; }

    // CookieModal [highlight]:
    public virtual RGBAColor ModalCookieButtonHighlightColor { get; }
    public virtual string ModalCookieButtonHighlightTextShadow { get; }
    
    // ProfileModal -----------------------------------------------------------------------------------------------------------------------
    public virtual RGBColor ModalProfileTabButtonColor => new(42, 33, 56);
    public virtual RGBColor ModalProfileTabButtonHighlightColor => new(245, 208, 0);
    public virtual RGBColor ModalProfileTabButtonActiveColor => new(245, 208, 0);
    public virtual string ModalProfileTabButtonActiveTextShadow => "-1px 1px 2px rgb(255, 231, 94)";
    public virtual RGBColor ModalProfileTabButtonDividerColor => new(236, 236, 236);
    public virtual RGBAColor ModalProfileAvatarBackground { get; }
    public virtual RGBAColor ModalProfileAvatarBorderColor { get; }
    public virtual string ModalProfileAvatarBoxShadow { get; }
    public virtual string ModalProfileAvatarSkinBoxShadow => "inset 0 0 0.5em rgba(0, 0, 0, 0.2)";
    public virtual string ModalProfileBorderColor => new RGBAColor(0, 0, 0, 0.2f);
    public virtual string ModalProfileAvatarEditBackground => new RGBAColor(0, 0, 0, 0.4f);
    public virtual string ModalProfileInputLabelColor => new RGBColor(153, 153, 153);
    public virtual string ModalProfileNavigationColor => new RGBColor(214, 218, 28);
    public virtual RGBAColor ModalProfileNavigationBackground => new(42, 33, 56);
    public virtual RGBAColor ModalProfileNavigationItemUnactive => new(255, 255, 255, 0.4f);
    public virtual RGBColor ModalProfileNavigationItemActive => new(255, 255, 255);

    // NOTE: Progress -------------------------------------------------------------------------------------------------------------------------
    // PasswordStrength -------------------------------------------------------------------------------------------------------------------
    public virtual RGBColor PasswordStrengthBarSegmentBackground => new(236, 240, 241);
    public virtual RGBColor PasswordStrengthRuleCompletedTextColor => new(46, 125, 50);
    public virtual string PasswordStrengthBarSegmentN1FilledBackground => "linear-gradient(90deg, rgb(229, 57, 53), rgb(255, 82, 82))";
    public virtual string PasswordStrengthBarSegmentN2FilledBackground => "linear-gradient(90deg, rgb(251, 140, 0), rgb(255, 167, 38))";
    public virtual string PasswordStrengthBarSegmentN3FilledBackground => "linear-gradient(90deg, rgb(255, 191, 0), rgb(255, 215, 0))";
    public virtual string PasswordStrengthBarSegmentN4FilledBackground => "linear-gradient(90deg, rgb(67, 160, 71), rgb(102, 187, 106))";

    // ProgressCircle ---------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor ProgressCircleColor { get; }
    public virtual RGBAColor ProgressCircleBackground { get; }

// NOTE: Text -----------------------------------------------------------------------------------------------------------------------------
    // Text -------------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor TextColor { get; }

    // Text [highlight]:
    public virtual RGBAColor TextHighlightColor { get; }

    // Text Accent ------------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor TextAccentColor { get; }

    // Text Accent [highlight]:
    public virtual RGBAColor TextAccentHighlightColor { get; }

    // Text [disabled] --------------------------------------------------------------------------------------------------------------------
    public virtual RGBAColor TextDisabledColor { get; }

// NOTE: Chat -----------------------------------------------------------------------------------------------------------------------------
    // Chat Message -----------------------------------------------------------------------------------------------------------------------
    public RGBAColor ChatMessageOwnText => new(42, 33, 56);
    public virtual RGBAColor ChatMessageOtherText { get; }
    public virtual RGBAColor ChatMessageOtherBackground { get; }
    public virtual RGBAColor ChatMessageOwnBackground { get; }
}

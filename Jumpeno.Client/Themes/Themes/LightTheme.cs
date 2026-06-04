namespace Jumpeno.Client.Constants;

public class LightTheme : BaseTheme {
// NOTE: Surface --------------------------------------------------------------------------------------------------------------------------
    // Primary ----------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor SurfaceBackground_SurfacePrimary => new(255, 255, 83);
    public override RGBAColor SurfaceBackground_SurfacePrimaryCollapse => new(255, 255, 255);
    public override RGBAColor SurfaceBackground_SurfacePrimaryBox => new(250, 250, 175);
    public override RGBAColor SurfaceBackground_SurfacePrimaryBoxCollapse => new(255, 255, 255);
    public override RGBAColor SurfaceBackground_SurfacePrimaryTransparent => new(SurfaceBackground_SurfacePrimaryBox, 0.6f);
    public override RGBAColor SurfaceBackground_SurfacePrimaryTransparentCollapse => new(255, 255, 255);
    public override RGBAColor SurfaceBackground_SurfacePrimaryGlass => new(0, 0, 0, 0.04f);
    public override RGBAColor SurfaceBackground_SurfacePrimaryGlassCollapse => new(255, 255, 255);

    // Secondary --------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor SurfaceBackground_SurfaceSecondary => new(255, 255, 255);

    // Floating ---------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor SurfaceBackground_SurfaceFloating => new(255, 255, 255);
    public override RGBAColor SurfaceBackground_SurfaceFloatingCollapse => new(241, 241, 241);
    public override RGBAColor SurfaceBackground_SurfaceFloatingAdditional => new(220, 220, 220);
    public override RGBAColor SurfaceBackground_SurfaceFloatingAdditionalCollapse => new(241, 241, 241);

// NOTE: Status ---------------------------------------------------------------------------------------------------------------------------
    // Danger -----------------------------------------------------------------------------------------------------------------------------
    // Box:
    public override RGBAColor StatusDangerBoxColor => new(83, 0, 0);
    public override RGBAColor StatusDangerBoxBackground => new(255, 186, 186);
    public override RGBAColor StatusDangerBoxOutlineColor => new(255, 0, 0);
    // Box [highlight]:
    public override RGBAColor StatusDangerBoxHighlightColor => new(143, 0, 0);
    public override RGBAColor StatusDangerBoxHighlightBackground => new(255, 219, 219);
    public override RGBAColor StatusDangerBoxHighlightOutlineColor => new(253, 118, 118);

    // Accent:
    public override RGBAColor StatusDangerAccentColor => new(255, 77, 79);
    // Accent [highlight]:
    public override RGBAColor StatusDangerAccentHighlightColor => new(255, 0, 4);

    // Neon:
    public override RGBAColor StatusDangerNeonColor => new(255, 0, 0);

    // Success ----------------------------------------------------------------------------------------------------------------------------
    // Box:
    public override RGBAColor StatusSuccessBoxColor => new(21, 87, 36);
    public override RGBAColor StatusSuccessBoxBackground => new(212, 237, 218);
    public override RGBAColor StatusSuccessBoxOutlineColor => new(82, 196, 26);
    // Box [highlight]:
    public override RGBAColor StatusSuccessBoxHighlightColor => new(41, 107, 56);
    public override RGBAColor StatusSuccessBoxHighlightBackground => new(233, 242, 235);
    public override RGBAColor StatusSuccessBoxHighlightOutlineColor => new(102, 216, 46);

    // Accent:
    public override RGBAColor StatusSuccessAccentColor => new(82, 196, 26);
    // Accent [highlight]:
    public override RGBAColor StatusSuccessAccentHighlightColor => new(72, 217, 0);

    // Neon:
    public override RGBAColor StatusSuccessNeonColor => new(0, 255, 0);

    // Warning ----------------------------------------------------------------------------------------------------------------------------
    // Box:
    public override RGBAColor StatusWarningBoxColor => new(135, 92, 5);
    public override RGBAColor StatusWarningBoxBackground => new(247, 225, 183);
    public override RGBAColor StatusWarningBoxOutlineColor => new(250, 173, 20);
    // Box [highlight]:
    public override RGBAColor StatusWarningBoxHighlightColor => new(145, 107, 31);
    public override RGBAColor StatusWarningBoxHighlightBackground => new(252, 239, 213);
    public override RGBAColor StatusWarningBoxHighlightOutlineColor => new(255, 191, 0);

    // Accent:
    public override RGBAColor StatusWarningAccentColor => new(250, 173, 20);
    // Accent [highlight]:
    public override RGBAColor StatusWarningAccentHighlightColor => new(255, 191, 0);

    // Neon:
    public override RGBAColor StatusWarningNeonColor => new(255, 255, 0);

    // Info -------------------------------------------------------------------------------------------------------------------------------
    // Box:
    public override RGBAColor StatusInfoBoxColor => new(12, 63, 110);
    public override RGBAColor StatusInfoBoxBackground => new(191, 224, 255);
    public override RGBAColor StatusInfoBoxOutlineColor => new(24, 144, 255);
    // Box [highlight]:
    public override RGBAColor StatusInfoBoxHighlightColor => new(15, 77, 135);
    public override RGBAColor StatusInfoBoxHighlightBackground => new(217, 234, 250);
    public override RGBAColor StatusInfoBoxHighlightOutlineColor => new(24, 170, 255);

    // Accent:
    public override RGBAColor StatusInfoAccentColor => new(24, 144, 255);
    // Accent [highlight]:
    public override RGBAColor StatusInfoAccentHighlightColor => new(24, 170, 255);

    // Neon:
    public override RGBAColor StatusInfoNeonColor => new(0, 251, 255);

// NOTE: Layout ---------------------------------------------------------------------------------------------------------------------------
    // Body -------------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor BodyBackground => SurfaceBackground_SurfaceSecondary;
    // Scrollbars:
    public override ScrollAreaTheme BodyScrollTheme => ScrollAreaTheme.OSThemeDark;
    // Selection:
    public override RGBAColor BodySelectionColor => new(42, 33, 56);
    public override RGBAColor BodySelectionBackground => new(255, 239, 0);
    // Backdrop:
    public override RGBAColor BodyBackdrop => new(0, 0, 0, 0.5f);

    // QR code ----------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor QrCodeBackground => new(255, 255, 255);

    // NavMenu ----------------------------------------------------------------------------------------------------------------------------
    public override string NavMenuBoxShadow => "0 0 20em rgba(0, 0, 0, 0.3)";

    // NavMenu [mobile]:
    public override RGBAColor NavMenuMobileButtonColor => new(82, 69, 103);
    public override ScrollAreaTheme NavMenuMobileScrollTheme => ScrollAreaTheme.OSThemeDark;

    // NavMenu [mobile][highlight]:
    public override RGBAColor NavMenuMobileButtonHighlightColor => new(132, 124, 145);

    // NavMenu [mobile][focus]:
    public override RGBAColor NavMenuMobileButtonFocusBackground => new(42, 33, 56, 0.08f);
    public override string NavMenuMobileButtonFocusBoxShadow => "0 1em 6em rgba(0, 0, 0, 0.4)";

// NOTE: Pages ----------------------------------------------------------------------------------------------------------------------------
    // Game -------------------------------------------------------------------------------------------------------------------------------    
    // Game > Components > CreateBox:
    public override string CreateBoxCanvasBoxShadowOpacity => "0.25";

    // Game > Components > GameScreen:
    // Control:
    public override RGBAColor GameScreenControlColor => new(105, 99, 115);
    public override RGBAColor GameScreenControlBackground => new(255, 255, 255);
    public override string GameScreenControlBoxShadow => "0 0.04em 0.16em 0.02em rgba(0, 0, 0, 0.2)";
    // Control [pressed]:
    public override RGBAColor GameScreenControlPressedColor => new(105, 99, 115);
    public override RGBAColor GameScreenControlPressedBackground => new(255, 255, 255, 0.7f);
    public override string GameScreenControlPressedBoxShadow => "0 0.04em 0.16em 0.02em rgba(0, 0, 0, 0.14)";

    // Game > Components > Lobby:
    public override string LobbyBoxShadow => "0 0.006em 0.018em 0 rgba(0, 0, 0, 0.2)";
    public override RGBAColor LobbyEmptyColor => new(0, 0, 0, 0.4f);
    // Players:
    public override RGBAColor LobbyLineBackground => new(255, 215, 0, 0.7f);
    public override string LobbyPresenceBoxShadow => "0.02em 0.02em 0.05em rgba(0, 0, 0, 0.7)";
    public override RGBAColor LobbyDashColor => new(0, 0, 0, 0.14f);

    // Manual -----------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor ManualColor => new(255, 255, 255);
    public override string ManualTextShadow => "0.5em 0.1em 0.53em rgba(0, 0, 0, 0.4)";
    public override RGBAColor ManualBackground => new(42, 144, 244);
    public override RGBAColor ManualBackgroundTransition => new(73, 175, 255);
    public override string ManualBoxShadow => "0 12em 20em 3em rgba(0, 0, 0, 0.25)";

// NOTE: Box ------------------------------------------------------------------------------------------------------------------------------
    // Box [box] --------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor BoxBackground_SurfacePrimaryBox => SurfaceBackground_SurfacePrimaryBox;
    public override string BoxBoxShadow_SurfacePrimaryBox => "0 6em 16em 0 rgba(0, 0, 0, 0.15)";

    // Box [transparent] ------------------------------------------------------------------------------------------------------------------
    public override RGBAColor BoxBackground_SurfacePrimaryTransparent => SurfaceBackground_SurfacePrimaryTransparent;
    public override string BoxBoxShadow_SurfacePrimaryTransparent => "0 6em 16em 0 rgba(0, 0, 0, 0.15)";

    // Box [glass] ------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor BoxBackground_SurfacePrimaryGlass => SurfaceBackground_SurfacePrimaryGlass;
    public override string BoxBoxShadow_SurfacePrimaryGlass => "0 3em 6em rgba(0, 0, 0, 0.14)";

// NOTE: Buttons --------------------------------------------------------------------------------------------------------------------------
    // Button [primary] -------------------------------------------------------------------------------------------------------------------
    public override RGBAColor ButtonPrimaryColor => new(42, 33, 56);
    public override RGBAColor ButtonPrimaryBackground => new(255, 255, 255);
    public override string ButtonPrimaryBoxShadow => "0 2em 4em rgba(0, 0, 0, 0.35)";

    // Button [primary][highlight]:
    public override RGBAColor ButtonPrimaryHighlightColor => new(42, 33, 56);
    public override RGBAColor ButtonPrimaryHighlightBackground => new(235, 235, 235);
    public override string ButtonPrimaryHighlightBoxShadow => "0 2em 4em rgba(0, 0, 0, 0.3)";

    // Button [secondary] -----------------------------------------------------------------------------------------------------------------
    public override RGBAColor ButtonSecondaryColor => new(42, 33, 56);
    public override RGBAColor ButtonSecondaryBackground => new(255, 215, 0);
    public override string ButtonSecondaryBoxShadow => "0 2em 4em rgba(0, 0, 0, 0.35)";

    // Button [secondary][highlight]:
    public override RGBAColor ButtonSecondaryHighlightColor => new(42, 33, 56);
    public override RGBAColor ButtonSecondaryHighlightBackground => new(255, 239, 0);
    public override string ButtonSecondaryHighlightBoxShadow => "0 2em 4em rgba(0, 0, 0, 0.3)";

    // Button [tertiary] ------------------------------------------------------------------------------------------------------------------
    public override RGBAColor ButtonTertiaryColor => new(42, 33, 56);
    public override RGBAColor ButtonTertiaryBackground => new(230, 230, 230);
    public override string ButtonTertiaryBoxShadow => ButtonSecondaryBoxShadow;

    // Button [tertiary][highlight]:
    public override RGBAColor ButtonTertiaryHighlightColor => new(42, 33, 56);
    public override RGBAColor ButtonTertiaryHighlightBackground => new(236, 236, 236);
    public override string ButtonTertiaryHighlightBoxShadow => ButtonSecondaryHighlightBoxShadow;

    // Button [quaternary] ----------------------------------------------------------------------------------------------------------------
    public override RGBAColor ButtonQuaternaryColor => new(236, 240, 241);
    public override RGBAColor ButtonQuaternaryBackground => new(42, 33, 56);
    public override string ButtonQuaternaryBoxShadow => ButtonPrimaryBoxShadow;

    // Button [quaternary][highlight]:
    public override RGBAColor ButtonQuaternaryHighlightColor => new(236, 240, 241);
    public override RGBAColor ButtonQuaternaryHighlightBackground => new(72, 63, 86);
    public override string ButtonQuaternaryHighlightBoxShadow => ButtonPrimaryHighlightBoxShadow;

    // Button [danger] --------------------------------------------------------------------------------------------------------------------
    public override RGBAColor ButtonDangerColor => new(255, 255, 255);
    public override RGBAColor ButtonDangerBackground => new(217, 26, 29);
    public override string ButtonDangerBoxShadow => ButtonPrimaryBoxShadow;

    // Button [danger][highlight]:
    public override RGBAColor ButtonDangerHighlightColor => new(255, 255, 255);
    public override RGBAColor ButtonDangerHighlightBackground => new(237, 43, 46);
    public override string ButtonDangerHighlightBoxShadow => ButtonPrimaryHighlightBoxShadow;

    // Button [disabled] ------------------------------------------------------------------------------------------------------------------
    public override RGBAColor ButtonDisabledColor => new(190, 190, 190);
    public override RGBAColor ButtonDisabledBackground => new(240, 240, 240);
    public override string ButtonDisabledBoxShadow => "0 2em 2em rgba(0, 0, 0, 0.2)";

    // MenuButton -------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor MenuButtonColor => new(42, 33, 56);
    public override RGBAColor MenuButtonBackground => new(0, 0, 0, 0.0f);
    public override string MenuButtonBoxShadow => "none";

    // MenuButton [highlight]:
    public override RGBAColor MenuButtonHighlightColor => new(42, 33, 56);
    public override RGBAColor MenuButtonHighlightBackground => new(42, 33, 56, 0.055f);
    public override string MenuButtonHighlightBoxShadow => "0em 2em 10em rgba(0, 0, 0, 0.02)";

    // MenuButton [active]:
    public override RGBAColor MenuButtonActiveColor => new(42, 33, 56);
    public override RGBAColor MenuButtonActiveBackground => new(0, 0, 0, 0.0f);
    public override string MenuButtonActiveBoxShadow => "0em 2em 7em rgba(0, 0, 0, 0.35)";

    // MenuButton [active][highlight]:
    public override RGBAColor MenuButtonActiveHighlightColor => new(42, 33, 56);
    public override RGBAColor MenuButtonActiveHighlightBackground => new(0, 0, 0, 0.0f);
    public override string MenuButtonActiveHighlightBoxShadow => "0em 2em 9em rgba(0, 0, 0, 0.5)";

    // MenuButton [mobile] ----------------------------------------------------------------------------------------------------------------
    public override RGBAColor MenuButtonMobileColor => new(42, 33, 56);
    public override RGBAColor MenuButtonMobileBackground => new(0, 0, 0, 0.0f);
    public override string MenuButtonMobileBoxShadow => "none";

    // MenuButton [mobile][hover]:
    public override RGBAColor MenuButtonMobileHoverColor => new(122, 113, 136);
    public override RGBAColor MenuButtonMobileHoverBackground => new(0, 0, 0, 0.0f);
    public override string MenuButtonMobileHoverBoxShadow => "none";

    // MenuButton [mobile][focus]:
    public override RGBAColor MenuButtonMobileFocusColor => new(42, 33, 56);
    public override RGBAColor MenuButtonMobileFocusBackground => new(42, 33, 56, 0.055f);
    public override string MenuButtonMobileFocusBoxShadow => "0em 2em 10em rgba(0, 0, 0, 0.02)";

    // MenuButton [mobile][active]:
    public override RGBAColor MenuButtonMobileActiveColor => new(42, 33, 56);
    public override RGBAColor MenuButtonMobileActiveBackground => new(0, 0, 0, 0.0f);
    public override string MenuButtonMobileActiveBoxShadow => "0em 2em 7em rgba(0, 0, 0, 0.35)";

    // MenuButton [mobile][active][hover]:
    public override RGBAColor MenuButtonMobileActiveHoverColor => new(42, 33, 56);
    public override RGBAColor MenuButtonMobileActiveHoverBackground => new(0, 0, 0, 0.0f);
    public override string MenuButtonMobileActiveHoverBoxShadow => "0em 2em 7em rgba(0, 0, 0, 0.35)";

    // MenuButton [mobile][active][focus]:
    public override RGBAColor MenuButtonMobileActiveFocusColor => new(42, 33, 56);
    public override RGBAColor MenuButtonMobileActiveFocusBackground => new(0, 0, 0, 0.0f);
    public override string MenuButtonMobileActiveFocusBoxShadow => "0em 2em 9em rgba(0, 0, 0, 0.5)";

// NOTE: Collapse -------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor CollapseTextColor => new(42, 33, 56);
    public override RGBAColor CollapseIconColor => new(42, 33, 56);
    public override RGBAColor CollapseIconBackground => new(235, 235, 235);
    public virtual RGBAColor CollapseIconBackground_SurfaceFloatingCollapse => new(220, 220, 220);
    public virtual RGBAColor CollapseIconBackground_SurfaceFloatingAdditionalCollapse => new(220, 220, 220);
    public override RGBAColor CollapseBackground => SurfaceBackground_SurfacePrimaryCollapse;
    public virtual RGBAColor CollapseBackground_SurfaceFloatingCollapse => SurfaceBackground_SurfaceFloatingCollapse;
    public virtual RGBAColor CollapseBackground_SurfaceFloatingAdditionalCollapse => SurfaceBackground_SurfaceFloatingAdditionalCollapse;
    public override string CollapseFocusBoxShadow => "0 0 10em rgb(204, 204, 204)";
    public virtual string CollapseFocusBoxShadow_SurfaceFloatingCollapse => "0 0 10em rgb(190, 190, 190)";
    public virtual string CollapseFocusBoxShadow_SurfaceFloatingAdditionalCollapse => "0 0 10em rgb(160, 160, 160)";

// NOTE: DropDowns ------------------------------------------------------------------------------------------------------------------------
    // DropDown ---------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor DropdownOptionsBackground => new(255, 255, 255);
    public override RGBAColor DropdownOptionsDividerColor => new(0, 0, 0, 0.1f);
    public override string DropdownOptionsBoxShadow => "0 0 12em rgba(0, 0, 0, 0.2)";
    public override string DropdownMarkBoxShadow => "0 0 8em 2em rgba(0, 0, 0, 0.2)";

    // DropDown [highlight]:
    public override RGBAColor DropdownOptionsHighlightBackground => new(241, 241, 241);

    // AdminDropDown ----------------------------------------------------------------------------------------------------------------------
    public override RGBAColor DropdownAdminBackground => new(102, 102, 102);
    public override RGBAColor DropdownAdminBorderColor => new(255, 255, 255, 0.9f);
    public override string DropdownAdminBoxShadow => "0 0 3em 1em rgba(0, 0, 0, 0.5)";

    // AdminDropDown [highlight]:
    public override RGBAColor DropdownAdminHighlightBorderColor => new(255, 255, 255);
    public override string DropdownAdminHighlightBoxShadow => "0 0 3em 2em rgba(0, 0, 0, 0.4)";

    // UserDropDown -----------------------------------------------------------------------------------------------------------------------
    public override RGBAColor DropdownUserBackground => new(102, 102, 102);
    public override RGBAColor DropdownUserBorderColor => new(255, 255, 255, 0.9f);
    public override string DropdownUserBoxShadow => "0 0 3em 2em rgba(42, 33, 56, 0.5)";

    // UserDropDown [highlight]:
    public override RGBAColor DropdownUserHighlightBorderColor => new(255, 255, 255);
    public override string DropdownUserHighlightBoxShadow => "0 0 3em 3em rgba(0, 0, 0, 0.5)";

// NOTE: Forms ----------------------------------------------------------------------------------------------------------------------------
    // Form [primary] ---------------------------------------------------------------------------------------------------------------------
    public override RGBAColor FormPrimaryColor => new(42, 33, 56);
    public override RGBAColor FormPrimaryPlaceholderColor => new(182, 178, 189);
    public override RGBAColor FormPrimaryIconColor => new(148, 148, 148);
    public override RGBAColor FormPrimaryDescriptionColor => new(42, 33, 56);
    public override RGBAColor FormPrimaryBackground => new(255, 255, 255);
    public override RGBAColor FormPrimaryBorderColor => new(204, 204, 204);
    public override string FormPrimaryTextShadow => "none";
    public override string FormPrimaryBoxShadow => "none";

    // Form [primary][highlight]:
    public override RGBAColor FormPrimaryHighlightColor => new(42, 33, 56);
    public override RGBAColor FormPrimaryHighlightPlaceholderColor => new(182, 178, 189);
    public override RGBAColor FormPrimaryHighlightIconColor => new(148, 148, 148);
    public override RGBAColor FormPrimaryHighlightDescriptionColor => new(42, 33, 56);
    public override RGBAColor FormPrimaryHighlightBackground => new(255, 255, 255);
    public override RGBAColor FormPrimaryHighlightBorderColor => new(204, 204, 204);
    public override string FormPrimaryHighlightTextShadow => "none";
    public override string FormPrimaryHighlightBoxShadow => "0 0 5em rgba(0, 0, 0, 0.4)";

    // Form [primary][disabled]:
    public override RGBAColor FormPrimaryDisabledColor => new(190, 190, 190);
    public override RGBAColor FormPrimaryDisabledPlaceholderColor => new(190, 190, 190);
    public override RGBAColor FormPrimaryDisabledIconColor => new(190, 190, 190);
    public override RGBAColor FormPrimaryDisabledDescriptionColor => new(180, 180, 180);
    public virtual RGBAColor FormPrimaryDisabledDescriptionColor_SurfaceFloatingCollapse => new(160, 160, 160);
    public virtual RGBAColor FormPrimaryDisabledDescriptionColor_SurfaceFloatingAdditional => new(140, 140, 140);
    public virtual RGBAColor FormPrimaryDisabledDescriptionColor_SurfaceFloatingAdditionalCollapse => new(160, 160, 160);
    public override RGBAColor FormPrimaryDisabledBackground => new(240, 240, 240);
    public override RGBAColor FormPrimaryDisabledBorderColor => new(214, 214, 214);
    public override string FormPrimaryDisabledTextShadow => "none";
    public override string FormPrimaryDisabledBoxShadow => "none";

    // Form [primary] > Icon [highlight]:
    public override RGBAColor FormPrimaryIconHighlightColor => new(70, 70, 70);
    
    // Form [primary] > Clear:
    public override RGBAColor FormPrimaryClearColor => new(42, 33, 56);
    public override RGBAColor FormPrimaryClearBackground => new(230, 230, 230);
    public override string FormPrimaryClearBoxShadow => "none";
    
    // Form [primary] > Clear [highlight]:
    public override RGBAColor FormPrimaryClearHighlightColor => new(0, 0, 0);
    public override RGBAColor FormPrimaryClearHighlightBackground => new(216, 216, 216);
    public override string FormPrimaryClearHighlightBoxShadow => "none";

// NOTE: Forms > CheckBoxes ---------------------------------------------------------------------------------------------------------------
    // CheckBox [primary] -----------------------------------------------------------------------------------------------------------------
    // CheckBox [primary][checked]:
    public override RGBAColor CheckboxPrimaryCheckedMarkColor => new(255, 255, 255);
    public override RGBAColor CheckboxPrimaryCheckedBackground => new(42, 33, 56);
    public virtual RGBAColor CheckboxPrimaryCheckedBackground_SurfaceSecondary => new(235, 195, 0);

    // CheckBox [primary][disabled][checked]:
    public override RGBAColor CheckboxPrimaryDisabledCheckedMarkColor => new(240, 240, 240);
    public override RGBAColor CheckboxPrimaryDisabledCheckedBackground => new(180, 180, 180);

// NOTE: Forms > Radios -------------------------------------------------------------------------------------------------------------------
    // Radio [primary] --------------------------------------------------------------------------------------------------------------------
    // Radio [primary][selected]:
    public override RGBAColor RadioPrimarySelectedMarkColor => new(42, 33, 56);
    public override RGBAColor RadioPrimarySelectedBackground => new(255, 255, 255);

    // Radio [primary][disabled][selected]:
    public override RGBAColor RadioPrimaryDisabledSelectedMarkColor => new(180, 180, 180);
    public override RGBAColor RadioPrimaryDisabledSelectedBackground => new(240, 240, 240);

    // RadioButton [primary] --------------------------------------------------------------------------------------------------------------
    public override RGBAColor RadioButtonPrimaryColor => new(42, 33, 56);
    public override RGBAColor RadioButtonPrimaryBackground => new(252, 252, 252);
    public override string RadioButtonPrimaryBoxShadow => "0 0 6em rgba(0, 0, 0, 0.2), 0 0 12em 4em rgba(0, 0, 0, 0.03) inset";

    // RadioButton [primary][selected]:
    public override RGBAColor RadioButtonPrimarySelectedColor => new(40, 40, 40);
    public override RGBAColor RadioButtonPrimarySelectedBackground => new(255, 215, 0);
    public override string RadioButtonPrimarySelectedBoxShadow => "0 0 6em 4em rgba(0, 0, 0, 0.3)";
    public override RGBAColor RadioButtonPrimarySelectedOutlineColor => new(255, 255, 255);

    // RadioButton [primary][highlight]:
    public override RGBAColor RadioButtonPrimaryHighlightColor => new(0, 0, 0);
    public override RGBAColor RadioButtonPrimaryHighlightBackground => new(255, 255, 255);
    public override string RadioButtonPrimaryHighlightBoxShadow => "0 0 6em rgba(0, 0, 0, 0.2)";

    // RadioButton [primary][highlight][selected]:
    public override RGBAColor RadioButtonPrimaryHighlightSelectedColor => new(20, 20, 20);
    public override RGBAColor RadioButtonPrimaryHighlightSelectedBackground => new(255, 239, 0);
    public override string RadioButtonPrimaryHighlightSelectedBoxShadow => "0 0 6em 4em rgba(0, 0, 0, 0.3)";
    public override RGBAColor RadioButtonPrimaryHighlightSelectedOutlineColor => new(255, 255, 255);
    
    // RadioButton [primary][disabled]:
    public override RGBAColor RadioButtonPrimaryDisabledColor => new(190, 190, 190);
    public override RGBAColor RadioButtonPrimaryDisabledBackground => new(240, 240, 240);
    public override string RadioButtonPrimaryDisabledBoxShadow => "0 0 6em rgba(0, 0, 0, 0.15)";

    // RadioButton [primary][disabled][selected]:
    public override RGBAColor RadioButtonPrimaryDisabledSelectedColor => new(130, 130, 130);
    public override RGBAColor RadioButtonPrimaryDisabledSelectedBackground => new(210, 210, 210);
    public override string RadioButtonPrimaryDisabledSelectedBoxShadow => "0 0 6em 4em rgba(0, 0, 0, 0.2)";
    public override RGBAColor RadioButtonPrimaryDisabledSelectedOutlineColor => new(255, 255, 255);

// NOTE: Forms > Selects ------------------------------------------------------------------------------------------------------------------
    // Select [primary] -------------------------------------------------------------------------------------------------------------------
    public override RGBAColor SelectOptionColor => new(42, 33, 56);
    public override RGBAColor SelectOptionBackground => new(255, 255, 255);
    public override RGBAColor SelectOptionDividerColor => new(233, 233, 233);

    // Select [primary][highlight]:
    public override RGBAColor SelectOptionHighlightColor => new(42, 33, 56);
    public override RGBAColor SelectOptionHighlightBackground => new(233, 233, 233);

    // Select [active]:
    public override RGBAColor SelectOptionActiveColor => new(255, 255, 255);
    public override RGBAColor SelectOptionActiveBackground => new(148, 148, 148);

    // Select [active][highlight]:
    public override RGBAColor SelectOptionActiveHighlightColor => new(255, 255, 255);
    public override RGBAColor SelectOptionActiveHighlightBackground => new(170, 170, 170);

    // SelectCulture ----------------------------------------------------------------------------------------------------------------------
    public override RGBAColor SelectCultureColor => new(42, 33, 56);
    public override RGBAColor SelectCultureBackground => new(0, 0, 0, 0.0f);
    public override RGBAColor SelectCultureBorderColor => new(0, 0, 0, 0.2f);
    public override string SelectCultureBoxShadow => "none";

    // SelectCulture [highlight]:
    public override RGBAColor SelectCultureHighlightColor => new(42, 33, 56);
    public override RGBAColor SelectCultureHighlightBackground => new(0, 0, 0, 0.0f);
    public override RGBAColor SelectCultureHighlightBorderColor => new(0, 0, 0, 0.2f);
    public override string SelectCultureHighlightBoxShadow => "0 1em 6em rgba(0, 0, 0, 0.3)";

// NOTE: Forms > SelectsMulti -------------------------------------------------------------------------------------------------------------
    // SelectMulti [primary] --------------------------------------------------------------------------------------------------------------
    public override RGBAColor SelectMultiPrimaryCountColor => new(255, 255, 255);
    public override RGBAColor SelectMultiPrimaryCountBackground => new(190, 190, 190);
    public override string SelectMultiPrimaryCountTextShadow => "1px 1px 2px rgba(0, 0, 0, 0.4)";
    public override string SelectMultiPrimaryCountBoxShadow => "1px 0 3px rgba(0, 0, 0, 0.5)";
    public override RGBAColor SelectMultiPrimaryCountPlusColor => new(255, 255, 255);
    public override string SelectMultiPrimaryCountPlusTextShadow => "1px 1px 2px rgba(0, 0, 0, 0.4)";

    // SelectMulti [primary][disabled]:
    public override RGBAColor SelectMultiPrimaryDisabledCountColor => new(240, 240, 240);
    public override RGBAColor SelectMultiPrimaryDisabledCountBackground => new(220, 220, 220);
    public override string SelectMultiPrimaryDisabledCountTextShadow => "1px 1px 2px rgba(0, 0, 0, 0.1)";
    public override string SelectMultiPrimaryDisabledCountBoxShadow => "1px 0 3px rgba(0, 0, 0, 0.2)";
    public override RGBAColor SelectMultiPrimaryDisabledCountPlusColor => new(240, 240, 240);
    public override string SelectMultiPrimaryDisabledCountPlusTextShadow => "1px 1px 2px rgba(0, 0, 0, 0.1)";

// NOTE: Forms > Switches -----------------------------------------------------------------------------------------------------------------
    // Switch [primary] -------------------------------------------------------------------------------------------------------------------
    public override RGBAColor SwitchPrimaryBackground => new(200, 200, 200);
    public virtual RGBAColor SwitchPrimaryBackground_SurfaceFloatingCollapse => new(180, 180, 180);
    public override RGBAColor SwitchPrimaryBulletBackground => new(255, 255, 255);

    // Switch [primary][checked]:
    public override RGBAColor SwitchPrimaryCheckedBackground => new(42, 33, 56);
    public virtual RGBAColor SwitchPrimaryCheckedBackground_SurfaceSecondary => new(235, 195, 0);

    // Switch [primary][focus]:
    public override RGBAColor SwitchPrimaryFocusOutlineColor => new(255, 255, 255);
    public override string SwitchPrimaryFocusOutlineShadow => "0 0 2em rgb(0, 0, 0)";

    // Switch [primary][disabled]:
    public override RGBAColor SwitchPrimaryDisabledBackground => new(220, 220, 220);
    public virtual RGBAColor SwitchPrimaryDisabledBackground_SurfaceFloatingCollapse => new(200, 200, 200);
    public virtual RGBAColor SwitchPrimaryDisabledBackground_SurfaceFloatingAdditional => new(180, 180, 180);
    public virtual RGBAColor SwitchPrimaryDisabledBackground_SurfaceFloatingAdditionalCollapse => new(200, 200, 200);
    public override RGBAColor SwitchPrimaryDisabledBulletBackground => new(240, 240, 240);

    // Switch [primary][disabled][checked]:
    public override RGBAColor SwitchPrimaryDisabledCheckedBackground => new(180, 180, 180);

// NOTE: GameCanvas -----------------------------------------------------------------------------------------------------------------------
    public override RGBAColor GameCanvasDefaultBackground => new(255, 255, 255);
    public override RGBAColor GameCanvasDefaultForeground => new(42, 33, 56);
    public override RGBColor GameCanvasDefaultTint => new(0, 0, 0);
    public override RGBAColor GameCanvasDefaultBorder => new(230, 230, 230);
    public override string GameCanvasBoxShadowOpacity => "0.4";

// NOTE: Images ---------------------------------------------------------------------------------------------------------------------------
    // Background -------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor BackgroundLightColor => new(210, 210, 210);
    public override RGBAColor BackgroundDarkColor => new(180, 180, 180);

    // Image ------------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor ImageLightColor => new(210, 210, 210);
    public override RGBAColor ImageDarkColor => new(180, 180, 180);
    public override RGBAColor ImageErrorColor => new(180, 180, 180);
    public override RGBAColor ImageIconColor => new(60, 60, 60);

// NOTE: Links ----------------------------------------------------------------------------------------------------------------------------
    // LogoLink ---------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor LogoLinkColor => new(42, 33, 56);
    public override RGBAColor LogoLinkBackground => new(0, 0, 0, 0.0f);
    public override string LogoLinkBoxShadow => "none";

    // LogoLink [focus]:
    public override RGBAColor LogoLinkFocusColor => new(42, 33, 56);
    public override RGBAColor LogoLinkFocusBackground => new(42, 33, 56, 0.08f);
    public override string LogoLinkFocusBoxShadow => "0 0.05em 0.3em rgba(0, 0, 0, 0.4)";

// NOTE: Loaders --------------------------------------------------------------------------------------------------------------------------
    // Loader -----------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor LoaderColor => new(42, 33, 56);
    public virtual RGBAColor LoaderColor_SurfaceSecondary => new(255, 215, 0);
    public override RGBAColor LoaderBackground => new(42, 33, 56, 0.4f);
    public virtual RGBAColor LoaderBackground_SurfaceSecondary => new(0, 0, 0, 0.45f);

    // PageLoader -------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor PageLoaderColor => new(235, 195, 0);
    public override RGBAColor PageLoaderBackground => new(0, 0, 0, 0.45f);

    // ServerPageLoader -------------------------------------------------------------------------------------------------------------------
    public override RGBAColor ServerPageLoaderTextColor => new(42, 33, 56, 0.9f);
    public override RGBAColor ServerPageLoaderColor => PageLoaderColor;
    public override RGBAColor ServerPageLoaderBackground => PageLoaderBackground;
    public override RGBAColor ServerPageLoaderBackdrop => SurfaceBackground_SurfaceSecondary;

// NOTE: Modals ---------------------------------------------------------------------------------------------------------------------------
    // Modal ------------------------------------------------------------------------------------------------------------------------------
    public override string ModalDialogBoxShadow => "0 0 30em 0 rgba(0, 0, 0, 0.3)";
    public override string ModalEndingBoxShadow => "0 0 10em rgba(0, 0, 0, 0.1)";

    // Modal control:
    public override RGBAColor ModalControlColor => new(255, 255, 255);
    public override RGBAColor ModalControlBackground => new(170, 170, 170);
    public override string ModalControlBoxShadow => "none";

    // Modal control [highlight]:
    public override RGBAColor ModalControlHighlightColor => new(255, 255, 255);
    public override RGBAColor ModalControlHighlightBackground => new(190, 190, 190);
    public override string ModalControlHighlightBoxShadow => "none";

    // CookieModal ------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor ModalCookieButtonColor => new(42, 33, 56);

    // CookieModal [highlight]:
    public override RGBAColor ModalCookieButtonHighlightColor => new(42, 33, 56);
    public override string ModalCookieButtonHighlightTextShadow => "0 0 1em rgb(42, 33, 56)";

    // ProfileModal -----------------------------------------------------------------------------------------------------------------------
    public override RGBAColor ModalProfileAvatarBackground => new(102, 102, 102);
    public override RGBAColor ModalProfileAvatarBorderColor => new(255, 255, 255);
    public override string ModalProfileAvatarBoxShadow => "0 1em 8em 1em rgba(0, 0, 0, 0.7)";

// NOTE: Progress -------------------------------------------------------------------------------------------------------------------------
    // ProgressCircle ---------------------------------------------------------------------------------------------------------------------
    public override RGBAColor ProgressCircleColor => new(42, 33, 56);
    public virtual RGBAColor ProgressCircleColor_SurfaceSecondary => new(255, 215, 0);
    public override RGBAColor ProgressCircleBackground => new(42, 33, 56, 0.4f);
    public virtual RGBAColor ProgressCircleBackground_SurfaceSecondary => new(0, 0, 0, 0.45f);

// NOTE: Text -----------------------------------------------------------------------------------------------------------------------------
    // Text -------------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor TextColor => new(42, 33, 56);

    // Text [highlight]:
    public override RGBAColor TextHighlightColor => new(122, 113, 136);

    // Text Accent ------------------------------------------------------------------------------------------------------------------------
    public override RGBAColor TextAccentColor => new(42, 33, 56);
    public virtual RGBAColor TextAccentColor_SurfaceSecondary => new(235, 195, 0);

    // Text Accent [highlight]:
    public override RGBAColor TextAccentHighlightColor => new(122, 113, 136);
    public virtual RGBAColor TextAccentHighlightColor_SurfaceSecondary => new(235, 219, 0);

    // Text [disabled] --------------------------------------------------------------------------------------------------------------------
    public override RGBAColor TextDisabledColor => new(180, 180, 180);

// NOTE: Chat -----------------------------------------------------------------------------------------------------------------------------
    // Chat Message -----------------------------------------------------------------------------------------------------------------------
    public override RGBAColor ChatMessageOtherBackground => new(240, 240, 240);
    public override RGBAColor ChatMessageOwnBackground => new(250, 250, 175);
    public override RGBAColor ChatMessageOtherText => new(42, 33, 56);
}

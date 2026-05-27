namespace Jumpeno.Client.Enums;

public enum Surface {
    // Primary:
    [CSSClass("surface-primary")] Priamary,
    [CSSClass("surface-primary-collapse")] PrimaryCollapse,
    [CSSClass("surface-primary-box")] PrimaryBox,
    [CSSClass("surface-primary-box-collapse")] PrimaryBoxCollapse,
    [CSSClass("surface-primary-transparent")] PrimaryTransparent,
    [CSSClass("surface-primary-transparent-collapse")] PrimaryTransparentCollapse,
    [CSSClass("surface-primary-glass")] PrimaryGlass,
    [CSSClass("surface-primary-glass-collapse")] PrimaryGlassCollapse,
    // Secondary:
    [CSSClass("surface-secondary")] Secondary,
    // Floating:
    [CSSClass("surface-floating")] Floating,
    [CSSClass("surface-floating-collapse")] FloatingCollapse,
    [CSSClass("surface-floating-additional")] FloatingAditional,
    [CSSClass("surface-floating-additional-collapse")] FloatingAddtitionalCollapse
}

namespace Jumpeno.Client.Constants;

public enum Surface {
    // Primary:
    [CSSClass("surface-primary")] PRIMARY,
    [CSSClass("surface-primary-collapse")] PRIMARY_COLLAPSE,
    [CSSClass("surface-primary-box")] PRIMARY_BOX,
    [CSSClass("surface-primary-box-collapse")] PRIMARY_BOX_COLLAPSE,
    [CSSClass("surface-primary-transparent")] PRIMARY_TRANSPARENT,
    [CSSClass("surface-primary-transparent-collapse")] PRIMARY_TRANSPARENT_COLLAPSE,
    [CSSClass("surface-primary-glass")] PRIMARY_GLASS,
    [CSSClass("surface-primary-glass-collapse")] PRIMARY_GLASS_COLLAPSE,
    // Secondary:
    [CSSClass("surface-secondary")] SECONDARY,
    // Floating:
    [CSSClass("surface-floating")] FLOATING,
    [CSSClass("surface-floating-collapse")] FLOATING_COLLAPSE,
    [CSSClass("surface-floating-additional")] FLOATING_ADDITIONAL,
    [CSSClass("surface-floating-additional-collapse")] FLOATING_ADDITIONAL_COLLAPSE
}

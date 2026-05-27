namespace Jumpeno.Client.Enums;

public enum TokenType {
    [StringValue("AccessToken")] Access,
    [StringValue("RefreshToken")] Refresh,
    [StringValue("ActivationToken")] Activation,
    [StringValue("PasswordResetToken")] PasswordReset,
    [StringValue("EmailChangeToken")] EmailChange
}

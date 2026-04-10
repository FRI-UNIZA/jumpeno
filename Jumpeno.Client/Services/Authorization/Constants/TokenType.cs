namespace Jumpeno.Client.Constants;

public enum TokenType {
    [StringValue("AccessToken")] ACCESS,
    [StringValue("RefreshToken")] REFRESH,
    [StringValue("ActivationToken")] ACTIVATION,
    [StringValue("PasswordResetToken")] PASSWORD_RESET,
    [StringValue("EmailChangeToken")] EMAIL_CHANGE
}

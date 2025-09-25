namespace Jumpeno.Client.Constants;

public enum TOKEN_TYPE {
    [StringValue("AccessToken")] ACCESS,
    [StringValue("RefreshToken")] REFRESH,
    [StringValue("ActivationToken")] ACTIVATION,
    [StringValue("PasswordResetToken")] PASSWORD_RESET
}

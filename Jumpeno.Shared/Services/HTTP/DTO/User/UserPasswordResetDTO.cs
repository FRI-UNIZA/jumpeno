namespace Jumpeno.Shared.Models;

public record UserPasswordResetDTO(
    string ResetToken
) : IValidable<UserPasswordResetDTO> {
    public List<Error> Validate() => TokenValidator.ValidateToken(ResetToken, nameof(ResetToken));
    public UserPasswordResetDTO Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? EXCEPTION.VALUES);
}

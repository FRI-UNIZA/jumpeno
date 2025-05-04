namespace Jumpeno.Shared.Models;

public record AuthInvalidateDTO(
    string RefreshToken
) : IValidable<AuthInvalidateDTO> {
    public List<Error> Validate() => TokenValidator.ValidateToken(RefreshToken, nameof(RefreshToken));
    public AuthInvalidateDTO Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? EXCEPTION.VALUES);
}

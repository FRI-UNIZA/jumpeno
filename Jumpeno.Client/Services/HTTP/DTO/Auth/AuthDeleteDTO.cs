namespace Jumpeno.Client.Models;

public record AuthDeleteDTO(
    string RefreshToken
) : IValidable<AuthDeleteDTO> {
    public List<Error> Validate() => TokenValidator.ValidateToken(RefreshToken, nameof(RefreshToken));
    public AuthDeleteDTO Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? EXCEPTION.VALUES);
}

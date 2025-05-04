namespace Jumpeno.Shared.Models;

public record AuthRefreshDTO(
    string RefreshToken
) : IValidable<AuthRefreshDTO> {
    public List<Error> Validate() => TokenValidator.ValidateToken(RefreshToken, nameof(RefreshToken));
    public AuthRefreshDTO Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? EXCEPTION.VALUES);
}

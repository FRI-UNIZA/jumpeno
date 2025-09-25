namespace Jumpeno.Client.Models;

public record UserActivateDTO(
    string ActivationToken
) : IValidable<UserActivateDTO> {
    public List<Error> Validate() => TokenValidator.ValidateToken(ActivationToken, nameof(ActivationToken));
    public UserActivateDTO Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? EXCEPTION.VALUES);
}

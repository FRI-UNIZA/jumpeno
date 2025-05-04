namespace Jumpeno.Shared.Models;

public record CookieSetDTO(
    List<string> AcceptedNames
) : IValidable<CookieSetDTO> {
    public List<Error> Validate() => Checker.ValidateUndefined(AcceptedNames, nameof(AcceptedNames));
    public CookieSetDTO Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? EXCEPTION.VALUES);
}

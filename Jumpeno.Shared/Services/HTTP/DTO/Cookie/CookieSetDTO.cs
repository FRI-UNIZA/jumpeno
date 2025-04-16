namespace Jumpeno.Shared.Models;

public record CookieSetDTO(
    List<string> AcceptedNames
) {
    public List<Error> Validate() => Checker.ValidateUndefined(nameof(AcceptedNames), AcceptedNames);
    public void Check(string? message = null) => Checker.CheckValues(Validate(), message);
}

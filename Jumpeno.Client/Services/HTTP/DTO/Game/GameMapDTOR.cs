namespace Jumpeno.Client.Models;

public record GameMapDTOR(
    Map Map
) : IValidable<GameMapDTOR> {
    public List<Error> Validate() => Checker.ValidateUndefined(Map, nameof(Map));
    public GameMapDTOR Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? Exceptions.Values);
}

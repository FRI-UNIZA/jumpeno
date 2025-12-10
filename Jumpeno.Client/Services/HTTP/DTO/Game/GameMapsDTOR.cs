namespace Jumpeno.Client.Models;

public record GameMapsDTORMap(
    int ID,
    string Name
);

public record GameMapsDTOR(
    List<GameMapsDTORMap> Maps
) : IValidable<GameMapsDTOR> {
    public List<Error> Validate() {
        var errors = Checker.ValidateUndefined(Maps, nameof(Maps));
        foreach (var map in Maps) {
            errors.AddRange(Checker.ValidateUndefined(map, nameof(GameMapsDTORMap)));
        }
        return errors;
    }
    public GameMapsDTOR Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? EXCEPTION.VALUES);
}

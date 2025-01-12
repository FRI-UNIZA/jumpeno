namespace Jumpeno.Shared.Utils;

public static class GameValidator {
    // Game code --------------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateCode(string code) {
        var errors = Checker.Validate(code.Length != Game.CODE_LENGTH,
            new Error(Game.CODE_ID, "Length must be equal to I18N{length}", new() {{"length", $"{Game.CODE_LENGTH}"}})
        );
        Checker.Validate(errors, !Checker.IsAlphaNum(code), new Error(Game.CODE_ID, "Code must be alphanumeric"));
        Checker.Validate(errors, code.ToUpper() != code, new Error(Game.CODE_ID, "Code must be uppercase"));
        return errors;
    }
    public static void CheckCode(string code) => Checker.Check(ValidateCode(code));

    // Game name --------------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateName(string name) {
        name = name.Trim();
        var errors = Checker.Validate(
            name.Length < Game.NAME_MIN_LENGTH || Game.NAME_MAX_LENGTH < name.Length,
            new Error(
                Game.NAME_ID,
                "Length is not between I18N{min} and I18N{max}",
                new() {{"min", $"{Game.NAME_MIN_LENGTH}"}, {"max", $"{Game.NAME_MAX_LENGTH}"}}
            )
        );
        Checker.Validate(errors, !Checker.IsAlphaNum(name, ['.', ' ']), new Error(Game.NAME_ID, "Value contains not allowed character"));
        Checker.Validate(errors, name.Length > 0 && name[0] == '.', new Error(Game.NAME_ID, "Value must not start with a dot"));
        return errors;
    }
    public static void CheckName(string name) => Checker.Check(ValidateName(name));

    // Game capacity ----------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateCapacity(byte capacity) => Checker.Validate(
        capacity < Game.MIN_CAPACITY || Game.MAX_CAPACITY < capacity,
        new Error(
            Game.CAPACITY_ID, "Capacity not between I18N{min} and I18N{max}",
            new() {{"min", $"{Game.MIN_CAPACITY}"}, {"max", $"{Game.MAX_CAPACITY}"}}
        )
    );
    public static void CheckCapacity(byte capacity) => Checker.Check(ValidateCapacity(capacity));

    // Connection type --------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateConnectionType(Connection connection) => Checker.Validate(
        connection.GetType() != typeof(Connection),
        new Error(Connection.CONNECTION_ID, "Connection type invalid!")
    );
    public static void CheckConnectionType(Connection connection) => Checker.Check(ValidateConnectionType(connection));

    // Player count -----------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidatePlayerCount(Game game) => Checker.Validate(
        game.Capacity <= game.ActivePlayersCount,
        new Error(Game.CAPACITY_ID, "The game is currently full!")
    );
    public static void CheckPlayerCount(Game game) => Checker.Check(ValidatePlayerCount(game));

    // Spectator count --------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateSpectatorCount(Game game) => Checker.Validate(
        Game.MAX_SPECTATORS <= game.SpectatorCount,
        new Error(Game.SPECTATOR_ID, "Game can not have more spectators!")
    );
    public static void CheckSpectatorCount(Game game) => Checker.Check(ValidateSpectatorCount(game));
}

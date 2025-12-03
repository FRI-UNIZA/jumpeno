namespace Jumpeno.Client.Utils;

public static class GameValidator {
    // Code -------------------------------------------------------------------------------------------------------------------------------
    public const byte CODE_LENGTH = 4;

    public static bool IsCode(string value) => Checker.IsAlphaNum(value);
    public static bool IsCodeCase(string value) => IsCode(value) && value.ToUpper() == value;
    public static List<Error> ValidateCode(string? value, string id = "") {
        var errors = Checker.ValidateUndefined(value, id); value = $"{value}";
        Checker.Validate(errors, value.Length != CODE_LENGTH,
            ERROR.DEFAULT.SetID(id)
            .SetInfo("Length must be I18N{length}", new() {{ "length", CODE_LENGTH }})
        );
        Checker.Validate(errors, !Checker.IsAlphaNum(value), ERROR.DEFAULT.SetID(id).SetInfo("Code must be alphanumeric"));
        Checker.Validate(errors, value.ToUpper() != value, ERROR.DEFAULT.SetID(id).SetInfo("Code must be uppercase"));
        return errors;
    }
    public static string AssertCode(string? value, string id = "", AppException? exception = null) {
        return Checker.Assert(value, ValidateCode(value, id), exception ?? EXCEPTION.VALUES)!;
    }

    // Name -------------------------------------------------------------------------------------------------------------------------------
    public const byte NAME_MIN_LENGTH = 3;
    public const byte NAME_MAX_LENGTH = 20;

    public static bool IsName(string value) => Checker.IsAlphaNum(value, ['.', ' ']);
    public static List<Error> ValidateName(string? value, string id = "") {
        var errors = Checker.ValidateUndefined(value, id);
        value = $"{value?.Trim()}";
        Checker.Validate(
            errors,
            value.Length < NAME_MIN_LENGTH || NAME_MAX_LENGTH < value.Length,
            ERROR.DEFAULT.SetID(id)
            .SetInfo("Length is not between I18N{min} and I18N{max}", new() {{ "min", NAME_MIN_LENGTH }, { "max", NAME_MAX_LENGTH }})
        );
        Checker.Validate(errors, !IsName(value), ERROR.DEFAULT.SetID(id).SetInfo("Value contains not allowed character"));
        Checker.Validate(errors, value.Length > 0 && value[0] == '.', ERROR.DEFAULT.SetID(id).SetInfo("Value must not start with a dot"));
        return errors;
    }
    public static string AssertName(string? value, string id = "", AppException? exception = null) {
        return Checker.Assert(value, ValidateName(value, id), exception ?? EXCEPTION.VALUES)!;
    }

    // Anonyms ----------------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateAnonyms(bool? value, string id = "") => Checker.ValidateUndefined(value, id);
    public static bool AssertAnonyms(bool? value, string id = "", AppException? exception = null) {
        return (bool)Checker.Assert(value, ValidateAnonyms(value, id), exception ?? EXCEPTION.VALUES)!;
    }

    public static List<Error> ValidateAllowedAnonyms(
        // Parameters:
        Game? game, User? user,
        // Exceptions:
        string gameID = "",
        string userID = "", string user_IDID = ""
    ) {
        List<Error> errors = [];
        errors.AddRange(Checker.ValidateUndefined(game, gameID));
        errors.AddRange(Checker.ValidateUndefined(user, userID));
        if (game == null || user == null) return errors;
        errors.AddRange(
            Checker.Validate(
                !game.Anonyms && user.ID == null,
                ERROR.DEFAULT.SetID(user_IDID).SetInfo("Anonymous players not allowed!")
            )
        );
        return errors;
    }
    public static User AssertAllowedAnonyms(
        // Parameters:
        Game? game, User? user,
        // Exceptions:
        string gameID = "",
        string userID = "", string user_IDID = "",
        AppException? exception = null
    )
    => Checker.Assert(
        user,
        ValidateAllowedAnonyms(game, user, gameID, userID, user_IDID),
        exception ?? EXCEPTION.CLIENT
    )!;

    // Rounds -----------------------------------------------------------------------------------------------------------------------------
    public const byte MIN_ROUNDS = 1;
    public const byte MAX_ROUNDS = 12;
    public static List<Error> ValidateRounds(byte? value, string id = "") {
        var errors = Checker.ValidateUndefined(value, id); value ??= 0;
        Checker.Validate(
            errors,
            value < MIN_ROUNDS || MAX_ROUNDS < value,
            ERROR.DEFAULT.SetID(id)
            .SetInfo("Number of rounds not between I18N{min} and I18N{max}", new() {{ "min", MIN_ROUNDS }, { "max", MAX_ROUNDS }})
        );
        return errors;
    }
    public static byte AssertRounds(byte? value, string id = "", AppException? exception = null) {
        return (byte)Checker.Assert(value, ValidateRounds(value, id), exception ?? EXCEPTION.VALUES)!;
    }

    // Capacity ---------------------------------------------------------------------------------------------------------------------------
    public const byte MIN_CAPACITY = 2;
    public const byte MAX_CAPACITY = 10;

    public static List<Error> ValidateCapacity(byte? value, string id = "") {
        var errors = Checker.ValidateUndefined(value, id); value ??= 0;
        Checker.Validate(
            errors,
            value < MIN_CAPACITY || MAX_CAPACITY < value,
            ERROR.DEFAULT.SetID(id)
            .SetInfo("Capacity not between I18N{min} and I18N{max}", new() {{ "min", MIN_CAPACITY }, { "max", MAX_CAPACITY }})
        );
        return errors;
    }
    public static byte AssertCapacity(byte? value, string id = "", AppException? exception = null) {
        return (byte)Checker.Assert(value, ValidateCapacity(value, id), exception ?? EXCEPTION.VALUES)!;
    }

    public static List<Error> ValidatePlayerCount(Game value, string id = "") => Checker.Validate(
        value.Capacity <= value.ActivePlayersCount,
        ERROR.DEFAULT.SetID(id).SetInfo("The game is currently full!")
    );
    public static Game AssertPlayerCount(Game value, string id = "", AppException? exception = null) {
        return Checker.Assert(value, ValidatePlayerCount(value, id), exception ?? EXCEPTION.VALUES);
    }

    // Display mode -----------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateDisplayMode(DISPLAY_MODE? value, string id = "") => Checker.ValidateUndefined(value, id);
    public static DISPLAY_MODE AssertDisplayMode(DISPLAY_MODE? value, string id = "", AppException? exception = null) {
        return (DISPLAY_MODE)Checker.Assert(value, ValidateDisplayMode(value, id), exception ?? EXCEPTION.VALUES)!;
    }

    // Game mode --------------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateGameMode(GAME_MODE? value, string id = "") => Checker.ValidateUndefined(value, id);
    public static GAME_MODE AssertGameMode(GAME_MODE? value, string id = "", AppException? exception = null) {
        return (GAME_MODE)Checker.Assert(value, ValidateGameMode(value, id), exception ?? EXCEPTION.VALUES)!;
    }

    // Host -------------------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateHostAlreadyConnected(
        // Parameters:
        Game? game, User? user,
        // Exceptions:
        string gameID = "",
        string userID = "", string user_IDID = ""
    ) {
        List<Error> errors = [];
        errors.AddRange(Checker.ValidateUndefined(game, gameID));
        errors.AddRange(Checker.ValidateUndefined(user, userID));
        if (game == null || user == null) return errors;
        errors.AddRange(
            Checker.Validate(
                game.HostConnected && user.ID == game.Host.ID,
                ERROR.DEFAULT.SetID(user_IDID).SetInfo("Host already connected!")
            )
        );
        return errors;
    }
    public static User AssertHostAlreadyConnected(
        // Parameters:
        Game? game, User? user,
        // Exceptions:
        string gameID = "",
        string userID = "", string user_IDID = "",
        AppException? exception = null
    )
    => Checker.Assert(
        user,
        ValidateHostAlreadyConnected(game, user, gameID, userID, user_IDID),
        exception ?? EXCEPTION.CLIENT
    )!;

    // Players ----------------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidatePlayerHostPresentation(
        // Parameters:
        Game? game, User? user,
        // Exceptions:
        string gameID = "",
        string userID = "", string user_IDID = ""
    ) {
        List<Error> errors = [];
        errors.AddRange(Checker.ValidateUndefined(game, gameID));
        errors.AddRange(Checker.ValidateUndefined(user, userID));
        if (game == null || user == null) return errors;
        errors.AddRange(
            Checker.Validate(
                game.DisplayMode == DISPLAY_MODE.PRESENTATION && user.ID == game.Host.ID,
                ERROR.DEFAULT.SetID(user_IDID).SetInfo("Host can not participate as a player!")
            )
        );
        return errors;
    }
    public static User AssertPlayerHostPresentation(
        // Parameters:
        Game? game, User? user,
        // Exceptions:
        string gameID = "",
        string userID = "", string user_IDID = "",
        AppException? exception = null
    )
    => Checker.Assert(
        user,
        ValidatePlayerHostPresentation(game, user, gameID, userID, user_IDID),
        exception ?? EXCEPTION.CLIENT
    )!;
    
    public static List<Error> ValidateReservedPlayerHostSpace(
        // Parameters:
        Game? game, User? user,
        // Exceptions:
        string gameID = "",
        string userID = "", string user_IDID = ""
    ) {
        List<Error> errors = [];
        errors.AddRange(Checker.ValidateUndefined(game, gameID));
        errors.AddRange(Checker.ValidateUndefined(user, userID));
        if (game == null || user == null) return errors;
        errors.AddRange(
            Checker.Validate(
                game.DisplayMode != DISPLAY_MODE.PRESENTATION &&
                game.State == GAME_STATE.LOBBY &&
                game.Capacity - 1 <= game.ActivePlayersCount &&
                !game.HostConnected &&
                user.ID != game.Host.ID,
                ERROR.DEFAULT.SetID(user_IDID).SetInfo("Space reserved for the host!")
            )
        );
        return errors;
    }
    public static User AssertReservedPlayerHostSpace(
        // Parameters:
        Game? game, User? user,
        // Exceptions:
        string gameID = "",
        string userID = "", string user_IDID = "",
        AppException? exception = null
    )
    => Checker.Assert(
        user,
        ValidateReservedPlayerHostSpace(game, user, gameID, userID, user_IDID),
        exception ?? EXCEPTION.DEFAULT
    )!;
    
    public static List<Error> ValidateReservedPlayerHostName(
        // Parameters:
        Game? game, User? user,
        // Exceptions:
        string gameID = "",
        string userID = "", string userNameID = ""
    ) {
        List<Error> errors = [];
        errors.AddRange(Checker.ValidateUndefined(game, gameID));
        errors.AddRange(Checker.ValidateUndefined(user, userID));
        if (game == null || user == null) return errors;
        errors.AddRange(
            Checker.Validate(
                game.DisplayMode != DISPLAY_MODE.PRESENTATION && user.ID != game.Host.ID && user.Name == game.Host.Name,
                ERROR.DEFAULT.SetID(userNameID).SetInfo("Name is reserved!")
            )
        );
        return errors;
    }
    public static string AssertReservedPlayerHostName(
        // Parameters:
        Game? game, User? user,
        // Exceptions:
        string gameID = "",
        string userID = "", string userNameID = "",
        AppException? exception = null
    )
    => Checker.Assert(
        user?.Name,
        ValidateReservedPlayerHostName(game, user, gameID, userID, userNameID),
        exception ?? EXCEPTION.VALUES
    )!;

    // Spectators -------------------------------------------------------------------------------------------------------------------------
    public static int MAX_SPECTATORS => AppSettings.Game.MaxSpectators;

    public static List<Error> ValidateSpectatorCount(Game value, string id = "") => Checker.Validate(
        MAX_SPECTATORS <= value.SpectatorCount,
        ERROR.DEFAULT.SetID(id).SetInfo("Game can not have more spectators!")
    );
    public static Game AssertSpectatorCount(Game value, string id = "", AppException? exception = null) {
        return Checker.Assert(value, ValidateSpectatorCount(value, id), exception ?? EXCEPTION.VALUES);
    }

    public static List<Error> ValidateSpectatorHostNonPresentation(
        // Parameters:
        Game? game, User? user,
        // Exceptions:
        string gameID = "",
        string userID = "", string user_IDID = ""
    ) {
        List<Error> errors = [];
        errors.AddRange(Checker.ValidateUndefined(game, gameID));
        errors.AddRange(Checker.ValidateUndefined(user, userID));
        if (game == null || user == null) return errors;
        errors.AddRange(
            Checker.Validate(
                game.DisplayMode != DISPLAY_MODE.PRESENTATION && user.ID == game.Host.ID,
                ERROR.DEFAULT.SetID(user_IDID).SetInfo("You must be connected as a player!")
            )
        );
        return errors;
    }
    public static User AssertSpectatorHostNonPresentation(
        // Parameters:
        Game? game, User? user,
        // Exceptions:
        string gameID = "",
        string userID = "", string user_IDID = "",
        AppException? exception = null
    )
    => Checker.Assert(
        user,
        ValidateSpectatorHostNonPresentation(game, user, gameID, userID, user_IDID),
        exception ?? EXCEPTION.CLIENT
    )!;

    public static List<Error> ValidateReservedSpectatorHostSpace(
        // Parameters:
        Game? game, User? user,
        // Exceptions:
        string gameID = "",
        string userID = "", string user_IDID = ""
    ) {
        List<Error> errors = [];
        errors.AddRange(Checker.ValidateUndefined(game, gameID));
        errors.AddRange(Checker.ValidateUndefined(user, userID));
        if (game == null || user == null) return errors;
        errors.AddRange(
            Checker.Validate(
                game.DisplayMode == DISPLAY_MODE.PRESENTATION &&
                MAX_SPECTATORS - 1 <= game.SpectatorCount &&
                !game.HostConnected &&
                user.ID != game.Host.ID,
                ERROR.DEFAULT.SetID(user_IDID).SetInfo("Space reserved for the host!")
            )
        );
        return errors;
    }
    public static User AssertReservedSpectatorHostSpace(
        // Parameters:
        Game? game, User? user,
        // Exceptions:
        string gameID = "",
        string userID = "", string user_IDID = "",
        AppException? exception = null
    )
    => Checker.Assert(
        user,
        ValidateReservedSpectatorHostSpace(game, user, gameID, userID, user_IDID),
        exception ?? EXCEPTION.DEFAULT
    )!;

    // Instances --------------------------------------------------------------------------------------------------------------------------
    public static int MAX_INSTANCES => AppSettings.Game.MaxInstances;

    public static List<Error> ValidateMaxInstances(int? value, string id = "") {
        var errors = Checker.ValidateUndefined(value, id);
        Checker.Validate(
            errors,
            MAX_INSTANCES <= value,
            ERROR.DEFAULT.SetID(id).SetInfo("Maximum active games limit exceeded!")
        );
        return errors;
    }
    public static int AssertMaxInstances(int? value, string id = "", AppException? exception = null) {
        return (int)Checker.Assert(value, ValidateMaxInstances(value, id), exception ?? EXCEPTION.VALUES)!;
    }
}

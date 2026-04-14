namespace Jumpeno.Client.Models;

public partial class Game {
    // Settings > Host --------------------------------------------------------------------------------------------------------------------
    private List<Error> ValidateHostConnectedOnce(
        // Parameters:
        User? user,
        // Exceptions:
        string userID = "", string user_IDID = ""
    ) {
        List<Error> errors = [];
        errors.AddRange(Checker.ValidateUndefined(user, userID));
        if (user == null) return errors;
        errors.AddRange(
            Checker.Validate(
                HostConnected && user.ID == Host.ID,
                Errors.DEFAULT.SetID(user_IDID).SetInfo("Host already connected!")
            )
        );
        return errors;
    }
    private User AssertHostConnectedOnce(
        // Parameters:
        User? user,
        // Exceptions:
        string userID = "", string user_IDID = "",
        AppException? exception = null
    )
    => Checker.Assert(
        user,
        ValidateHostConnectedOnce(user, userID, user_IDID),
        exception ?? Exceptions.CLIENT
    )!;
    
    // Settings > Anonyms -----------------------------------------------------------------------------------------------------------------
    private void AssertAllowedAnonymousUser(
        // Parameters:
        User? user,
        // Exceptions:
        string userID = "", string user_IDID = "",
        AppException? exception = null
    ) {
        Checker.Assert(Checker.ValidateUndefined(user, userID));
        if (user == null) return;
        Checker.Assert(
            Checker.Validate(
                !Anonyms && user.ID == null,
                Errors.DEFAULT.SetID(user_IDID).SetInfo("Anonymous players not allowed!")
            ),
            exception ?? Exceptions.CLIENT
        );
    }

    // Settings > Capacity ----------------------------------------------------------------------------------------------------------------
    protected void AssertCapacity(AppException? exception = null) {
        if (Capacity <= ActivePlayersCount) throw (exception ?? Exceptions.VALUES).SetInfo("The game is currently full!");
    }

    // Settings > Players -----------------------------------------------------------------------------------------------------------------
    private List<Error> ValidatePlayerHostPresentation(
        // Parameters:
        User? user,
        // Exceptions:
        string userID = "", string user_IDID = ""
    ) {
        List<Error> errors = [];
        errors.AddRange(Checker.ValidateUndefined(user, userID));
        if (user == null) return errors;
        errors.AddRange(
            Checker.Validate(
                DisplayMode == DisplayMode.Presentation && user.ID == Host.ID,
                Errors.DEFAULT.SetID(user_IDID).SetInfo("Host can not participate as a player!")
            )
        );
        return errors;
    }
    private User AssertPlayerHostPresentation(
        // Parameters:
        User? user,
        // Exceptions:
        string userID = "", string user_IDID = "",
        AppException? exception = null
    )
    => Checker.Assert(
        user,
        ValidatePlayerHostPresentation(user, userID, user_IDID),
        exception ?? Exceptions.CLIENT
    )!;
    
    private List<Error> ValidateReservedPlayerHostSpace(
        // Parameters:
        User? user,
        // Exceptions:
        string userID = "", string user_IDID = ""
    ) {
        List<Error> errors = [];
        errors.AddRange(Checker.ValidateUndefined(user, userID));
        if (user == null) return errors;
        errors.AddRange(
            Checker.Validate(
                DisplayMode != DisplayMode.Presentation &&
                State == GameStates.Lobby &&
                Capacity - 1 <= ActivePlayersCount &&
                !HostConnected &&
                user.ID != Host.ID,
                Errors.DEFAULT.SetID(user_IDID).SetInfo("Space reserved for the host!")
            )
        );
        return errors;
    }
    private User AssertReservedPlayerHostSpace(
        // Parameters:
        User? user,
        // Exceptions:
        string userID = "", string user_IDID = "",
        AppException? exception = null
    )
    => Checker.Assert(
        user,
        ValidateReservedPlayerHostSpace(user, userID, user_IDID),
        exception ?? Exceptions.DEFAULT
    )!;
    
    private List<Error> ValidateReservedPlayerHostName(
        // Parameters:
        User? user,
        // Exceptions:
        string userID = "", string userNameID = ""
    ) {
        List<Error> errors = [];
        errors.AddRange(Checker.ValidateUndefined(user, userID));
        if (user == null) return errors;
        errors.AddRange(
            Checker.Validate(
                DisplayMode != DisplayMode.Presentation && user.ID != Host.ID && user.Name == Host.Name,
                Errors.DEFAULT.SetID(userNameID).SetInfo("Name is reserved!")
            )
        );
        return errors;
    }
    private string AssertReservedPlayerHostName(
        // Parameters:
        User? user,
        // Exceptions:
        string userID = "", string userNameID = "",
        AppException? exception = null
    )
    => Checker.Assert(
        user?.Name,
        ValidateReservedPlayerHostName(user, userID, userNameID),
        exception ?? Exceptions.VALUES
    )!;

    // Settings > Spectators --------------------------------------------------------------------------------------------------------------
    private void AssertSpectatorCount(AppException? exception = null) {
        if (GameValidator.MAX_SPECTATORS <= SpectatorCount) throw (exception ?? Exceptions.VALUES)
        .SetInfo("Game can not have more spectators!");
    }

    private List<Error> ValidateSpectatorHostNonPresentation(
        // Parameters:
        User? user,
        // Exceptions:
        string userID = "", string user_IDID = ""
    ) {
        List<Error> errors = [];
        errors.AddRange(Checker.ValidateUndefined(user, userID));
        if (user == null) return errors;
        errors.AddRange(
            Checker.Validate(
                DisplayMode != DisplayMode.Presentation && user.ID == Host.ID,
                Errors.DEFAULT.SetID(user_IDID).SetInfo("You must be connected as a player!")
            )
        );
        return errors;
    }
    private User AssertSpectatorHostNonPresentation(
        // Parameters:
        User? user,
        // Exceptions:
        string userID = "", string user_IDID = "",
        AppException? exception = null
    )
    => Checker.Assert(
        user,
        ValidateSpectatorHostNonPresentation(user, userID, user_IDID),
        exception ?? Exceptions.CLIENT
    )!;

    private List<Error> ValidateReservedSpectatorHostSpace(
        // Parameters:
        User? user,
        // Exceptions:
        string userID = "", string user_IDID = ""
    ) {
        List<Error> errors = [];
        errors.AddRange(Checker.ValidateUndefined(user, userID));
        if (user == null) return errors;
        errors.AddRange(
            Checker.Validate(
                DisplayMode == DisplayMode.Presentation &&
                GameValidator.MAX_SPECTATORS - 1 <= SpectatorCount &&
                !HostConnected &&
                user.ID != Host.ID,
                Errors.DEFAULT.SetID(user_IDID).SetInfo("Space reserved for the host!")
            )
        );
        return errors;
    }
    private User AssertReservedSpectatorHostSpace(
        // Parameters:
        User? user,
        // Exceptions:
        string userID = "", string user_IDID = "",
        AppException? exception = null
    )
    => Checker.Assert(
        user,
        ValidateReservedSpectatorHostSpace(user, userID, user_IDID),
        exception ?? Exceptions.DEFAULT
    )!;

    // Player > Getters > Found player ----------------------------------------------------------------------------------------------------
    private static List<Error> ValidateFoundPlayer(
        // Parameters:
        Player? player,
        // Exceptions:
        string playerID = ""
    )
    => Checker.Validate(player == null, Errors.INVALID.SetID(playerID).SetInfo("Not a player of this game!"));
    private static Player AssertFoundPlayer(
        // Parameters:
        Player? player,
        // Exceptions:
        string playerID = "",
        AppException? exception = null
    )
    => Checker.Assert(player, ValidateFoundPlayer(player, playerID), exception ?? Exceptions.VALUES)!;

    // Player > Getters > Host ------------------------------------------------------------------------------------------------------------
    public Player AssertHostPlayer(string hostID = "") => AssertFoundPlayer(GetHostPlayer(), hostID);

    // Player > Getters > All players -----------------------------------------------------------------------------------------------------
    public Player AssertPlayer(
        // Parameters:
        byte? id,
        // Exceptions:
        string idID = ""
    )
    => AssertFoundPlayer(GetPlayer(id), idID);

    public Player AssertPlayerByConnectionID(
        // Parameters:
        string? connectionID,
        // Exceptions:
        string connectionIDID = ""
    )
    => AssertFoundPlayer(GetPlayerByConnectionID(connectionID), connectionIDID);

    public Player AssertPlayerByName(
        // Parameters:
        string? name,
        // Exceptions:
        string nameID = ""
    )
    => AssertFoundPlayer(GetPlayerByName(name), nameID);

    // Player > Getters > All players -----------------------------------------------------------------------------------------------------
    public Player AssertValidPlayer(
        // Parameters:
        byte? id,
        // Exceptions:
        string idID = ""
    )
    => AssertFoundPlayer(GetValidPlayer(id), idID);

    
    public Player AssertValidPlayerByConnectionID(
        // Parameters:
        string? connectionID,
        // Exceptions:
        string connectionIDID = ""
    )
    => AssertFoundPlayer(GetValidPlayerByConnectionID(connectionID), connectionIDID);

    public Player AssertValidPlayerByName(
        // Parameters:
        string? name,
        // Exceptions:
        string nameID = ""
    )
    => AssertFoundPlayer(GetValidPlayerByName(name), nameID);

    // Player > Getters > Active players --------------------------------------------------------------------------------------------------
    public Player AssertActivePlayer(
        // Parameters:
        byte? id,
        // Exceptions:
        string idID = ""
    )
    => AssertFoundPlayer(GetActivePlayer(id), idID);
    
    public Player AssertActivePlayerByConnectionID(
        // Parameters:
        string? connectionID,
        // Exceptions:
        string connectionIDID = ""
    )
    => AssertFoundPlayer(GetActivePlayerByConnectionID(connectionID), connectionIDID);

    public Player AssertActivePlayerByName(
        // Parameters:
        string? name,
        // Exceptions:
        string nameID = ""
    )
    => AssertFoundPlayer(GetActivePlayerByName(name), nameID);
}

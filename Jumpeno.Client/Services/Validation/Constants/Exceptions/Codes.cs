namespace Jumpeno.Client.Constants;

public static class Codes {
    // Success ----------------------------------------------------------------------------------------------------------------------------
    public const int Success = 200;
    public const int NoContent = 204;

    // Errors -----------------------------------------------------------------------------------------------------------------------------
    public const int Default = Server;
    public const int Client = 400;
    public const int Values = 400;
    public const int BadRequest = 400;
    public const int CaptchaMissing = Values;
    public const int CaptchaInvalid = Values;
    public const int CaptchaError = BadRequest;
    public const int NotAuthenticated = 401;
    public const int NotAuthorized = 403;
    public const int NotFound = 404;
    public const int InvalidToken = 406;
    public const int Server = 500;
    public const int Disconnect = 503;

    // Internal ---------------------------------------------------------------------------------------------------------------------------
    public const int RequestCancelled = 600;
    public const int RequestFailed = 601;
    public const int ParsingError = 602;
}

namespace Jumpeno.Client.Constants;

public static class Codes {
    // Success ----------------------------------------------------------------------------------------------------------------------------
    public const int SUCCESS = 200;
    public const int NO_CONTENT = 204;

    // Errors -----------------------------------------------------------------------------------------------------------------------------
    public const int DEFAULT = SERVER;
    public const int CLIENT = 400;
    public const int VALUES = 400;
    public const int BAD_REQUEST = 400;
    public const int CAPTCHA_MISSING = VALUES;
    public const int CAPTCHA_INVALID = VALUES;
    public const int CAPTCHA_ERROR = BAD_REQUEST;
    public const int NOT_AUTHENTICATED = 401;
    public const int NOT_AUTHORIZED = 403;
    public const int NOT_FOUND = 404;
    public const int INVALID_TOKEN = 406;
    public const int SERVER = 500;
    public const int DISCONNECT = 503;

    // Internal ---------------------------------------------------------------------------------------------------------------------------
    public const int REQUEST_CANCELLED = 600;
    public const int REQUEST_FAILED = 601;
    public const int PARSING_ERROR = 602;
}

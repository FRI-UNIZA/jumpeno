namespace Jumpeno.Client.Constants;

public static class Messages {
    public static TInfo Default => Server;
    public static TInfo Client => new("Something went wrong.");
    public static TInfo Values => new("Incorrect field values.");
    public static TInfo BadRequest => new("Bad request.");
    public static TInfo CaptchaMissing => new("ReCAPTCHA is missing.");
    public static TInfo CaptchaInvalid => new("Invalid reCAPTCHA token.");
    public static TInfo CaptchaError => new("Problem with reCAPTCHA.");
    public static TInfo NotAuthenticated => new("Not authenticated.");
    public static TInfo NotAuthorized => new("Not authorized.");
    public static TInfo NotFound => new("Not found.");
    public static TInfo InvalidToken => new("Invalid token.");
    public static TInfo Server => new("Something went wrong.");
    public static TInfo Disconnect => new("You have been disconnected from the server.");
    public static TInfo RequestCancelled => new("Request cancelled.");
    public static TInfo RequestFailed => new("Request failed.");
    public static TInfo ParsingError => new("Parsing error.");
}

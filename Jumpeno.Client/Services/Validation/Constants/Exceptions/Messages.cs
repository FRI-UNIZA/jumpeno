namespace Jumpeno.Client.Constants;

public static class Messages {
    public static TInfo DEFAULT => SERVER;
    public static TInfo CLIENT => new("Something went wrong.");
    public static TInfo VALUES => new("Incorrect field values.");
    public static TInfo BAD_REQUEST => new("Bad request.");
    public static TInfo CAPTCHA_MISSING => new("ReCAPTCHA is missing.");
    public static TInfo CAPTCHA_INVALID => new("Invalid reCAPTCHA token.");
    public static TInfo CAPTCHA_ERROR => new("Problem with reCAPTCHA.");
    public static TInfo NOT_AUTHENTICATED => new("Not authenticated.");
    public static TInfo NOT_AUTHORIZED => new("Not authorized.");
    public static TInfo NOT_FOUND => new("Not found.");
    public static TInfo INVALID_TOKEN => new("Invalid token.");
    public static TInfo SERVER => new("Something went wrong.");
    public static TInfo DISCONNECT => new("You have been disconnected from the server.");
    public static TInfo REQUEST_CANCELLED => new("Request cancelled.");
    public static TInfo REQUEST_FAILED => new("Request failed.");
    public static TInfo PARSING_ERROR => new("Parsing error.");
}

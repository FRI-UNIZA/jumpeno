namespace Jumpeno.Client.Constants;

public static class Exceptions {
    public static AppException Default => Server;
    public static AppException Client => new AppException().SetCode(Codes.Client).SetInfo(Messages.Client);
    public static AppException Values => new AppException().SetCode(Codes.Values).SetInfo(Messages.Values);
    public static AppException BadRequest => new AppException().SetCode(Codes.BadRequest).SetInfo(Messages.BadRequest);
    public static AppException CaptchaMissing => new AppException().SetCode(Codes.CaptchaMissing).SetInfo(Messages.CaptchaMissing);
    public static AppException CaptchaInvalid => new AppException().SetCode(Codes.CaptchaInvalid).SetInfo(Messages.CaptchaInvalid);
    public static AppException CaptchaError => new AppException().SetCode(Codes.CaptchaError).SetInfo(Messages.CaptchaError);
    public static AppException NotAuthenticated => new AppException().SetCode(Codes.NotAuthenticated).SetInfo(Messages.NotAuthenticated);
    public static AppException NotAuthorized => new AppException().SetCode(Codes.NotAuthorized).SetInfo(Messages.NotAuthorized);
    public static AppException NotFound => new AppException().SetCode(Codes.NotFound).SetInfo(Messages.NotFound);
    public static AppException InvalidToken => new AppException().SetCode(Codes.InvalidToken).SetInfo(Messages.InvalidToken);
    public static AppException Server => new AppException().SetCode(Codes.Server).SetInfo(Messages.Server);
    public static AppException Disconnect => new AppException().SetCode(Codes.Disconnect).SetInfo(Messages.Disconnect);
    public static AppException RequestCancelled => new AppException().SetCode(Codes.RequestCancelled).SetInfo(Messages.RequestCancelled);
    public static AppException RequestFailed => new AppException().SetCode(Codes.RequestFailed).SetInfo(Messages.RequestFailed);
    public static AppException ParsingError => new AppException().SetCode(Codes.ParsingError).SetInfo(Messages.ParsingError);
}

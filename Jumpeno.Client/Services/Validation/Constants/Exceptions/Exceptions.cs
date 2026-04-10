namespace Jumpeno.Client.Constants;

public static class Exceptions {
    public static AppException DEFAULT => SERVER;
    public static AppException CLIENT => new AppException().SetCode(Codes.CLIENT).SetInfo(Messages.CLIENT);
    public static AppException VALUES => new AppException().SetCode(Codes.VALUES).SetInfo(Messages.VALUES);
    public static AppException BAD_REQUEST => new AppException().SetCode(Codes.BAD_REQUEST).SetInfo(Messages.BAD_REQUEST);
    public static AppException CAPTCHA_MISSING => new AppException().SetCode(Codes.CAPTCHA_MISSING).SetInfo(Messages.CAPTCHA_MISSING);
    public static AppException CAPTCHA_INVALID => new AppException().SetCode(Codes.CAPTCHA_INVALID).SetInfo(Messages.CAPTCHA_INVALID);
    public static AppException CAPTCHA_ERROR => new AppException().SetCode(Codes.CAPTCHA_ERROR).SetInfo(Messages.CAPTCHA_ERROR);
    public static AppException NOT_AUTHENTICATED => new AppException().SetCode(Codes.NOT_AUTHENTICATED).SetInfo(Messages.NOT_AUTHENTICATED);
    public static AppException NOT_AUTHORIZED => new AppException().SetCode(Codes.NOT_AUTHORIZED).SetInfo(Messages.NOT_AUTHORIZED);
    public static AppException NOT_FOUND => new AppException().SetCode(Codes.NOT_FOUND).SetInfo(Messages.NOT_FOUND);
    public static AppException INVALID_TOKEN => new AppException().SetCode(Codes.INVALID_TOKEN).SetInfo(Messages.INVALID_TOKEN);
    public static AppException SERVER => new AppException().SetCode(Codes.SERVER).SetInfo(Messages.SERVER);
    public static AppException DISCONNECT => new AppException().SetCode(Codes.DISCONNECT).SetInfo(Messages.DISCONNECT);
    public static AppException REQUEST_CANCELLED => new AppException().SetCode(Codes.REQUEST_CANCELLED).SetInfo(Messages.REQUEST_CANCELLED);
    public static AppException REQUEST_FAILED => new AppException().SetCode(Codes.REQUEST_FAILED).SetInfo(Messages.REQUEST_FAILED);
    public static AppException PARSING_ERROR => new AppException().SetCode(Codes.PARSING_ERROR).SetInfo(Messages.PARSING_ERROR);
}

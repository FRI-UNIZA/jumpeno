namespace Jumpeno.Shared.Constants;

public static class EXCEPTION {
    public static AppException DEFAULT => SERVER;
    public static AppException CLIENT => new AppException().SetCode(CODE.CLIENT).SetInfo(MESSAGE.CLIENT);
    public static AppException VALUES => new AppException().SetCode(CODE.VALUES).SetInfo(MESSAGE.VALUES);
    public static AppException BAD_REQUEST => new AppException().SetCode(CODE.BAD_REQUEST).SetInfo(MESSAGE.BAD_REQUEST);
    public static AppException NOT_AUTHENTICATED => new AppException().SetCode(CODE.NOT_AUTHENTICATED).SetInfo(MESSAGE.NOT_AUTHENTICATED);
    public static AppException NOT_AUTHORIZED => new AppException().SetCode(CODE.NOT_AUTHORIZED).SetInfo(MESSAGE.NOT_AUTHORIZED);
    public static AppException NOT_FOUND => new AppException().SetCode(CODE.NOT_FOUND).SetInfo(MESSAGE.NOT_FOUND);
    public static AppException INVALID_TOKEN => new AppException().SetCode(CODE.INVALID_TOKEN).SetInfo(MESSAGE.INVALID_TOKEN);
    public static AppException SERVER => new AppException().SetCode(CODE.SERVER).SetInfo(MESSAGE.SERVER);
    public static AppException DISCONNECT => new AppException().SetCode(CODE.DISCONNECT).SetInfo(MESSAGE.DISCONNECT);
    public static AppException REQUEST_CANCELLED => new AppException().SetCode(CODE.REQUEST_CANCELLED).SetInfo(MESSAGE.REQUEST_CANCELLED);
    public static AppException REQUEST_FAILED => new AppException().SetCode(CODE.REQUEST_FAILED).SetInfo(MESSAGE.REQUEST_FAILED);
    public static AppException PARSING_ERROR => new AppException().SetCode(CODE.PARSING_ERROR).SetInfo(MESSAGE.PARSING_ERROR);
}

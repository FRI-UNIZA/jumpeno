namespace Jumpeno.Shared.Models;

/// <summary>Lightweight AppException used in websocket.</summary>
public class AppExceptionDTO {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public int Code { get; private set; }
    public TInfo Info { get; private set; }
    public List<Error> Errors { get; private set; }
    public IDictionary Data { get; private set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor][Newtonsoft.Json.JsonConstructor]
    protected AppExceptionDTO(
        int code,
        TInfo info, List<Error> errors,
        IDictionary data
    ) {
        Code = code;
        Info = new(info);
        Errors = errors;
        Data = data;
    }
    public AppExceptionDTO(AppException exception) : this(
        exception.Code,
        new(exception.Info), exception.Errors,
        exception.Data
    ) {}

    // Exception --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public AppException Exception => new AppException()
        .SetCode(Code)
        .SetInfo(new(Info)).SetErrors(Errors)
        .SetData(Data);
}

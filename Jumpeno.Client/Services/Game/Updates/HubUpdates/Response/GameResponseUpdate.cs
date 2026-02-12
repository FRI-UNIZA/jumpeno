namespace Jumpeno.Client.Models;

public abstract class GameResponseUpdate : GameHubUpdate
{
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public AppExceptionDTO? Exception { get; private set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    public GameResponseUpdate(AppExceptionDTO? exception = null) => Exception = exception;
    public GameResponseUpdate(Exception? exception) : this(DTO(exception)) {}
    
    // Utils ------------------------------------------------------------------------------------------------------------------------------
    protected static AppExceptionDTO? DTO(Exception? exception)
    => exception is null ? null : (exception is AppException e ? e : EXCEPTION.DEFAULT).DTO;

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}

namespace Jumpeno.Shared.Models;

public record MessageDTO(
    string Message
) : IValidable<MessageDTO> {
    public List<Error> Validate() => Checker.ValidateEmpty(Message, nameof(Message));
    public MessageDTO Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? EXCEPTION.VALUES);
}

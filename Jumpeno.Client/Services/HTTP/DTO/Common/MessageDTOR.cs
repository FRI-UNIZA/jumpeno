namespace Jumpeno.Client.Models;

public record MessageDTOR(
    string Message
) : IValidable<MessageDTOR> {
    public List<Error> Validate() => Checker.ValidateEmpty(Message, nameof(Message));
    public MessageDTOR Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? EXCEPTION.SERVER);   
}

namespace Jumpeno.Shared.Models;

public record MessageDTOR(
    string Message
) {
    public List<Error> Validate() => Checker.ValidateEmpty(nameof(Message), Message);
    public void Check(string? message = null) => Checker.CheckValues(Validate(), message);   
}

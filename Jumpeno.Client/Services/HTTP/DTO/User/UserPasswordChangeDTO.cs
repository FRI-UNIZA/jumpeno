namespace Jumpeno.Client.Models;

public record UserPasswordChangeDTO(string NewPassword) : IValidable<UserPasswordChangeDTO>
{
    public List<Error> Validate() => UserValidator.ValidatePassword(NewPassword, nameof(NewPassword));
    public UserPasswordChangeDTO Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? Exceptions.VALUES);
}

namespace Jumpeno.Client.Models;

public record UserLoginDTO(
    string Email,
    string Password
) : IValidable<UserLoginDTO> {
    public List<Error> Validate() {
        var errors = UserValidator.ValidateEmail(Email, nameof(Email));
        errors.AddRange(UserValidator.ValidatePassword(Password, nameof(Password)));
        return errors;
    }
    public UserLoginDTO Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? EXCEPTION.VALUES);
}

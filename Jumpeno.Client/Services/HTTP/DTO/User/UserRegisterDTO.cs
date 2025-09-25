namespace Jumpeno.Client.Models;

public record UserRegisterDTO(
    string Email,
    string Name,
    string Password
) : IValidable<UserRegisterDTO> {
    public List<Error> Validate() {
        var errors = UserValidator.ValidateEmail(Email, nameof(Email));
        errors.AddRange(UserValidator.ValidateName(Name, true, nameof(Name)));
        errors.AddRange(UserValidator.ValidatePassword(Password, nameof(Password)));
        return errors;
    }
    public UserRegisterDTO Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? EXCEPTION.VALUES);
}

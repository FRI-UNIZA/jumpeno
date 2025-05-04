namespace Jumpeno.Shared.Models;

public record UserPasswordResetRequestDTO(
    string Email
) : IValidable<UserPasswordResetRequestDTO> {
    public List<Error> Validate() => UserValidator.ValidateEmail(Email, nameof(Email));
    public UserPasswordResetRequestDTO Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? EXCEPTION.VALUES);
}

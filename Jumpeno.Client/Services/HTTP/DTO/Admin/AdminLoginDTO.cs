namespace Jumpeno.Client.Models;

public record AdminLoginDTO(
    string Email
) : IValidable<AdminLoginDTO> {
    public List<Error> Validate() => AdminValidator.ValidateEmail(Email, nameof(Email));
    public AdminLoginDTO Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? EXCEPTION.VALUES);
}

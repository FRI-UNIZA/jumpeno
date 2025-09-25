namespace Jumpeno.Client.Models;

public record UserProfileDTOR(
    User Profile
) : IValidable<UserProfileDTOR> {
    public List<Error> Validate() => UserValidator.Validate(Profile, nameof(Profile));
    public UserProfileDTOR Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? EXCEPTION.SERVER);
}

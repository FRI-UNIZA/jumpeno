namespace Jumpeno.Client.Models;

public record UserUpdateDTO(string? NewName = null, Skin? NewSkin = null, string? NewEmail = null) : IValidable<UserUpdateDTO>
{
    public List<Error> Validate()
    {
        List<Error> errors = [];
        if (NewSkin is not null) errors.AddRange(UserValidator.ValidateSkin(NewSkin, nameof(NewSkin)));
        if (NewEmail is not null) errors.AddRange(UserValidator.ValidateEmail(NewEmail, nameof(NewEmail)));
        if (NewName is not null) errors.AddRange(UserValidator.ValidateName(NewName, true, nameof(NewName)));
        return errors;
    }

    public UserUpdateDTO Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? Exceptions.Values);
}

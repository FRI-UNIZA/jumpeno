
namespace Jumpeno.Client.Models;

public record GoogleReCAPTCHA_DTOR(bool Success, float Score) : IValidable<GoogleReCAPTCHA_DTOR>
{
    public List<Error> Validate() 
    {
        var errors = new List<Error>();
        if (Score < 0f || Score > 1f) errors.Add(Errors.INVALID.SetID(nameof(Score)));
        return errors;
    }
    public GoogleReCAPTCHA_DTOR Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? Exceptions.SERVER);
}

namespace Jumpeno.Server.Services;

public class CaptchaValidatorService(AttemptService attemptService)
{
    // Actions [validation] ---------------------------------------------------------------------------------------------------------------
    public async Task<bool> ValidateAsync(string? token)
    {
        if (!AppSettings.ReCAPTCHA.On) return true;
        if (string.IsNullOrWhiteSpace(token)) return false;
        
        QueryParams q = new();
        q.Set("secret", ServerSettings.ReCAPTCHA.Secret);
        q.Set("response", token);
        var response = await HTTP.Post<GoogleReCAPTCHA_DTOR>(API.GOOGLE.RECAPTCHA.SITE_VERIFY, query: q);
        response.Body.Assert();

        return response.Code == CODE.SUCCESS && response.Body.Success;
    }

    /// <summary>Validates the provided captcha token.</summary>
    /// <param name="token">Captcha token to validate.</param>
    /// <param name="captchaID">ID of the captcha component for error tracking.</param>
    /// <exception cref="EXCEPTION.CAPTCHA_MISSING">Throws if the captcha is invalid.</exception>
    public async Task AssertAsync(
        // Parameters:
        string? token,
        // Exceptions:
        string captchaID = ""
    ) {
        if (!AppSettings.ReCAPTCHA.On) return;
        if (string.IsNullOrEmpty(token)) throw EXCEPTION.CAPTCHA_MISSING.Add(ERROR.EMPTY.SetID(captchaID));
        if (!await ValidateAsync(token)) throw EXCEPTION.CAPTCHA_INVALID.Add(ERROR.INVALID.SetID(captchaID));
    }

    // Actions [email] --------------------------------------------------------------------------------------------------------------------
    public async Task AssertTokenForEmail(
        // Parameters:
        string email, string? token,
        // Exceptions:
        string captchaID = ""
    ) {
        if (!AppSettings.ReCAPTCHA.On) return;
        if (!attemptService.IncrementAndCheckIfEmailBlocked(email)) return;        
        await AssertAsync(token, captchaID);
    }

    // Actions [IP] -----------------------------------------------------------------------------------------------------------------------
    public async Task AssertTokenForIP(
        // Parameters:
        ATTEMPTS_CATEGORY category, string? token,
        // Exceptions:
        string captchaID = ""
    ) {
        if (!AppSettings.ReCAPTCHA.On) return;
        if (!attemptService.IncrementAndCheckIfIPBlocked(category)) return;        
        await AssertAsync(token, captchaID);
    }

    // Actions [email][IP] ----------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Important because token can be checked only once, so both user and IP attempts need to be counted before checking the token.
    /// </summary>
    /// <param name="token">CAPTCHA token to validate.</param>
    /// <param name="email">Email to check user attempts for (optional).</param> 
    /// <param name="category">Category to check IP attempts for (optional).</param>
    /// <param name="captchaID">ID for error tracking (optional).</param>
    /// <returns>Task to await</returns>
    public async Task AssertTokenForEmailAndIP(
        // Parameters:
        string? token, string? email = null, ATTEMPTS_CATEGORY? category = null,
        // Exceptions:
        string captchaID = ""
    ) {
        if (!AppSettings.ReCAPTCHA.On) return;
        if (
            (email is null || !attemptService.IncrementAndCheckIfEmailBlocked(email)) &
            (category is null || !attemptService.IncrementAndCheckIfIPBlocked((ATTEMPTS_CATEGORY)category))
        ) return;
        await AssertAsync(token, captchaID);
    }
}

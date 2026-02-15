namespace Jumpeno.Client.Components;

public partial class ReCAPTCHA
{
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS_NAME = "recaptcha";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter] public required string Form { get; set; }
    [Parameter] public required string ID { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly string CAPTCHA_ID = IDGenerator.Generate(nameof(ReCAPTCHA));
    public bool Showing { get; private set; } = false;

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private ReCAPTCHAViewModel ViewModel { get; set; } = null!;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS_NAME, Base);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentInitialized() => ViewModel = new ReCAPTCHAViewModel(Form, ID, x => Show());

    protected override async Task OnComponentAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        try
        {
            await JS.EvalVoidAsync($$"""
                grecaptcha.render('{{CAPTCHA_ID}}', {
                    sitekey : '{{AppSettings.ReCAPTCHA.SiteKey}}',
                    theme : 'light'
                });
            """);
        }
        catch (Exception) { return; }
    }

    protected override void OnComponentDispose() => ViewModel.Error.Detach();

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    /// <summary>Retrieves token and resets ReCaptcha.</summary>
    /// <returns>Returns ReCaptcha token or empty string if ReCaptcha is not shown.</returns>
    /// <exception cref="EXCEPTION.DEFAULT">Can throw if grecaptcha is not loaded properly.</exception>
    public async Task<string> GetToken() 
    {
        if (!AppSettings.ReCAPTCHA.On) return string.Empty;
        try
        {
            if (!Showing) return string.Empty;

            var captchaToken = await JS.InvokeAsync<string>("grecaptcha.getResponse");
            await JS.InvokeVoidAsync("grecaptcha.reset"); // Reset captcha after each request bcs tokens live only for 1 check
            if (string.IsNullOrWhiteSpace(captchaToken)) throw EXCEPTION.CAPTCHA_MISSING;
            return captchaToken;
        }
        catch (AppException) { throw; }
        catch (Exception) { throw EXCEPTION.CAPTCHA_ERROR; }
    }

    public void Show()
    {
        Showing = true;
        StateHasChanged();
    }

    public void Hide()
    {
        Showing = false;
        StateHasChanged();
    }
}

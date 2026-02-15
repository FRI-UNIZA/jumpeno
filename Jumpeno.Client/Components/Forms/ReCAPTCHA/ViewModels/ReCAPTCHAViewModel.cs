namespace Jumpeno.Client.ViewModels;

public class ReCAPTCHAViewModel(
    string? form = null,
    string? id = null,
    Action<string>? onError = null
)
: FormViewModel(form, id, onError)
{}

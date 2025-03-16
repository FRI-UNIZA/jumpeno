namespace Jumpeno.Client.Components;

public partial class RegisterForm {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required LoginPageViewModel VM { get; set; }

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly InputViewModel<string> VMEmail;
    private readonly InputViewModel<string> VMPlayerName;
    private readonly InputViewModel<string> VMPassword;
    private readonly InputViewModel<string> VMConfirmPassword;
    
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public RegisterForm() {
        VMEmail = new(new InputViewModelTextParams(
            ID: "Email",
            TextMode: INPUT_TEXT_MODE.NORMAL,
            Trim: true,
            TextCheck: Checker.IsEmail,
            MaxLength: 100,
            Name: "Email",
            Label: @I18N.T("Email"),
            Placeholder: @I18N.T("Email"),
            DefaultValue: ""
        ));
        VMPlayerName = new(new InputViewModelTextParams(
            ID: "Player name",
            TextMode: INPUT_TEXT_MODE.NORMAL,
            Trim: true,
            TextCheck: Checker.IsAlphaNum,
            MaxLength: 100,
            Name: "Player name",
            Label: @I18N.T("Player name"),
            Placeholder: @I18N.T("Player name"),
            DefaultValue: ""
        ));
        VMPassword = new(new InputViewModelTextParams(
            ID: "Password",
            TextMode: INPUT_TEXT_MODE.NORMAL,
            Trim: true,
            TextCheck: Checker.IsPassword,
            MaxLength: 100,
            Name: "Password",
            Label: @I18N.T("Password"),
            Placeholder: @I18N.T("Password"),
            DefaultValue: "",
            Secret: true
        ));
        VMConfirmPassword = new(new InputViewModelTextParams(
            ID: "Confirm password",
            TextMode: INPUT_TEXT_MODE.NORMAL,
            Trim: true,
            TextCheck: Checker.IsPassword,
            MaxLength: 100,
            Name: "Confirm password",
            Label: @I18N.T("Confirm password"),
            Placeholder: @I18N.T("Confirm password"),
            DefaultValue: "",
            Secret: true
        ));
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private void Register() => Notification.Error(I18N.T("This functionality is not implemented yet."));
}

namespace Jumpeno.Client.Components;

public partial class UserLoginForm {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required LoginPageViewModel VM { get; set; }

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly InputViewModel<string> VMEmail;
    private readonly InputViewModel<string> VMPassword;
    
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public UserLoginForm() {
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
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private void Login() { Notification.Error(I18N.T("This functionality is not implemented yet.")); throw new InvalidDataException("asd");  }
}

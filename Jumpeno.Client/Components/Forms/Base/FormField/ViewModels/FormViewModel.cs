namespace Jumpeno.Client.Models;

public class FormViewModel {
    // Identifiers ------------------------------------------------------------------------------------------------------------------------
    public readonly string Form;
    public readonly string ID;
    public readonly string FormID;

    // Notification -----------------------------------------------------------------------------------------------------------------------
    public Action Notify { get; private set; } // Notify after state change
    public Action React { get; private set; } // Custom react action (can be used for anything)

    // Error ------------------------------------------------------------------------------------------------------------------------------
    public FormErrorViewModel Error { get; private set; }
    public Action<string>? ErrorDelegate { get; protected set; } // NOTE: Called instead of showing error if set
    public Action<string>? OnError { get; private set; }

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static void SetNotify(FormViewModel viewModel, Action notify) => viewModel.Notify = notify;
    public static void SetReact(FormViewModel viewModel, Action react) => viewModel.React = react;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public FormViewModel(string? form = null, string? id = null, Action<string>? onError = null) {
        Form = form ?? IDGenerator.Generate(nameof(Form).ToLower());
        ID = id ?? IDGenerator.Generate(nameof(FormID));
        FormID = FormManager.CreateFormID(Form, ID);
        Notify = () => {};
        React = () => {};
        Error = new FormErrorViewModel(this);
        ErrorDelegate = null;
        OnError = onError;
    }
}

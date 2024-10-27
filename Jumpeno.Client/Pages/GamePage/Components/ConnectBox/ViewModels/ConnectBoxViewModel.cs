namespace Jumpeno.Client.ViewModels;

public class ConnectBoxViewModel(ConnectBoxProps props) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string KEY_PREFIX = "connect-box";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public Func<string> Key { get; private set; } = () => $"{KEY_PREFIX}-{props.Key()}";
    public Func<string> DefaultCode { get; private set; } = props.DefaultCode ?? (() => "");
    public EventDelegate<ConnectData> OnPlay { get; private set; } = props.OnPlay ?? EventDelegate<ConnectData>.EMPTY;
    public EventDelegate<ConnectData> OnWatch { get; private set; } = props.OnWatch ?? EventDelegate<ConnectData>.EMPTY;
}

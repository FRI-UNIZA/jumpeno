namespace Jumpeno.Client.ViewModels;

public class LobbyViewModel(LobbyViewModelProps props) {
    public EmptyDelegate OnRender { get; private set; } = props.OnRender ?? EmptyDelegate.EMPTY;
}

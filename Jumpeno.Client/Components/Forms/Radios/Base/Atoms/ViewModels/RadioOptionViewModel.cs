namespace Jumpeno.Client.ViewModels;

public class RadioOptionViewModel<T>(RadioOptionViewModelParams<T> p) : FormViewModel(onError: p.OnError) {
    public T Value { get; private set; } = p.Value;
    public string Label { get; private set; } = p.Label;
}

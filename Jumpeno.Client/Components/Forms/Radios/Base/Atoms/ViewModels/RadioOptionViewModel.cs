namespace Jumpeno.Client.ViewModels;

public class RadioOptionViewModel<T>(RadioOptionViewModelParams<T> p) : FormViewModel(onError: p.OnError) {
    // Values -----------------------------------------------------------------------------------------------------------------------------
    public int Key { get { return DTO.Key; } set { DTO.Key = value; } }
    public T Value { get { return DTO.Value; } private set { DTO.Value = value; } }
    public string Label { get { return DTO.Label; } private set { DTO.Label = value; } }

    // Data Transfer Object ---------------------------------------------------------------------------------------------------------------
    public RadioOptionDTO<T> DTO { get; private set; } = new(p.Key, p.Value, p.Label);
}

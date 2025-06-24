namespace Jumpeno.Client.ViewModels;

public class SelectViewModel : FormViewModel {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static readonly SelectOption EMPTY_OPTION = new(null, I18N.T("Empty"));

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public List<SelectOption> Options { get; private set; }
    public SelectOption DefaultValue { get; private set; }
    public bool Empty { get; set; }
    // Selected:
    public SelectOption Value { get; private set; }
    public void SetValue(SelectOption option) { if (option != Value) { Value = option; Error.Clear(); } }
    // Callbacks:
    public EventDelegate<SelectEvent> OnSelect { get; set; }
    public EventDelegate<SelectEvent> OnCloseSelected { get; set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public SelectViewModel(SelectViewModelParams p) : base(p.Form, p.ID) {
        if (p.Options == null || p.Options.Count < 1) throw new InvalidDataException("Empty select options!");
        Options = p.Options;
        DefaultValue = p.DefaultValue ?? EMPTY_OPTION;
        Empty = p.Empty;
        Value = DefaultValue;
        OnSelect = p.OnSelect ?? EventDelegate<SelectEvent>.EMPTY;
        OnCloseSelected = p.OnCloseSelected ?? EventDelegate<SelectEvent>.EMPTY;
    }
}

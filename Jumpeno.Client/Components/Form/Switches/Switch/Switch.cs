namespace Jumpeno.Client.Components;

public partial class Switch {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASSNAME = "switch-component";
    public const string CLASSNAME_ELEMENT = "switch-element";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required string Label { get; set; }
    [Parameter]
    public SWITCH_SIZE Size { get; set; } = SWITCH_SIZE.LARGE;
    [Parameter]
    public bool DefaultValue { get; set; } = false;
    public bool Value { get; private set; } = false;
    [Parameter]
    public bool StopPropagation { get; set; } = false;
    [Parameter]
    public bool Disabled { get; set; } = false;
    [Parameter]
    public EventCallback<bool> OnChange { get; set; } = EventCallback<bool>.Empty;
    
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public readonly string ID;
    public readonly string ID_ELEMENT;
    protected CSSClass ComputeClass() {
        var c = ComputeClass(CLASSNAME);
        if (Disabled) c.Set("disabled");
        return c;
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public Switch() {
        ID = IDGenerator.Generate(CLASSNAME);
        ID_ELEMENT = $"{ID}-{CLASSNAME_ELEMENT}";
    }

    protected override void OnComponentParametersSet(bool firstTime) => Value = DefaultValue;
    protected override void OnComponentAfterRender(bool firstRender) {
        if (AppEnvironment.IsServer || !firstRender) return;
        JS.InvokeVoid(JSSwitch.InitInstance, ID, CLASSNAME_ELEMENT, Label);
    }

    // Events -----------------------------------------------------------------------------------------------------------------------------
    private async Task OnChangeEvent(bool value) {
        if (Disabled) return;
        await PageLoader.Show(PAGE_LOADER_TASK.SWITCH, true);
        await OnChange.InvokeAsync(value);
        await Task.Delay(AppTheme.TRANSITION_FAST);
        await PageLoader.Hide(PAGE_LOADER_TASK.SWITCH);
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public void Click() {
        ActionHandler.Click($"#{ID} > button");
    }
}

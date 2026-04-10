namespace Jumpeno.Client.Components;

public partial class ModalElement {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required Modal Modal { get; set; }
    [Parameter]
    public required bool Inert { get; set; }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() {
        var c = base.ComputeClass()
        .Set(Modal.CLASS, Base)
        .SetSurface(Modal.Surface);
        switch (Modal.State) {
            case ModalStateType.PRE_OPEN:
                c.Set("pre-open");
            break;
            case ModalStateType.LOADING:
                c.Set("loading");
            break;
            case ModalStateType.CLOSING:
                c.Set("closing");
            break;
            case ModalStateType.CLOSING_LOADING:
                c.Set("closing"); c.Set("closing-loading");
            break;
        }
        c.Set(Modal.CLASS_NO_HEADER, Modal.NoHeader);
        c.Set(Modal.CLASS_NO_FOOTER, Modal.NoFooter);
        c.Set(Modal.CLASS_UNCLOSABLE, Modal.Unclosable);
        c.Set(Modal.ComputeClass());
        return c;
    }

    public CSSStyle ComputeStyle() => new(Modal.Style);

    public CSSClass ComputeDialogClass() => new(Modal.CLASS_DIALOG);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    private ModalStateType LastState = ModalStateType.CLOSED;
    protected override async Task OnComponentAfterRenderAsync(bool firstRender) {
        if (LastState != ModalStateType.OPEN && Modal.State == ModalStateType.OPEN) {
            ModalProvider.NotifyOpen();
        }
        LastState = Modal.State;
        if (AppEnvironment.IsServer || !firstRender) return;
        await ModalProvider.AddElement(this);
        JS.InvokeVoid(JSModal.Activate, Modal.ID);
    }

    protected override void OnComponentDispose() => ModalProvider.NotifyDispose(Modal);

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public void StartClosing() {
        Type type = typeof(Modal);
        PropertyInfo? prop = type.GetProperty(nameof(Modal.State));
        if (prop is null) return;
        MethodInfo? setMethod = prop.GetSetMethod(nonPublic: true);
        if (setMethod is null) return;
        var loading = Modal.State == ModalStateType.LOADING;
        setMethod.Invoke(Modal, loading ? [ModalStateType.CLOSING_LOADING] : [ModalStateType.CLOSING]);
        JS.InvokeVoid(JSModal.Deactivate, Modal.ID, loading);
        StateHasChanged();
    }
}

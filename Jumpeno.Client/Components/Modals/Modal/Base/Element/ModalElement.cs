namespace Jumpeno.Client.Components;

using System.Reflection;

public partial class ModalElement {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS_UNCLOSABLE = "unclosable";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required Modal Modal { get; set; }
    [Parameter]
    public required bool Inert { get; set; }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() {
        var c = base.ComputeClass()
        .Set(Modal.CLASS_MODAL, Base)
        .SetSurface(Modal.Surface);
        switch (Modal.State) {
            case MODAL_STATE.PRE_OPEN:
                c.Set("pre-open");
            break;
            case MODAL_STATE.LOADING:
                c.Set("loading");
            break;
            case MODAL_STATE.CLOSING:
                c.Set("closing");
            break;
            case MODAL_STATE.CLOSING_LOADING:
                c.Set("closing"); c.Set("closing-loading");
            break;
        }
        c.Set(CLASS_UNCLOSABLE, Modal.Unclosable);
        c.Set(Modal.Class);
        return c;
    }

    public CSSStyle ComputeStyle() => new(Modal.Style);

    public CSSClass ComputeDialogClass() => new(Modal.CLASS_DIALOG);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    private MODAL_STATE LastState = MODAL_STATE.CLOSED;
    protected override async Task OnComponentAfterRenderAsync(bool firstRender) {
        if (LastState != MODAL_STATE.OPEN && Modal.State == MODAL_STATE.OPEN) {
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
        var loading = Modal.State == MODAL_STATE.LOADING;
        setMethod.Invoke(Modal, loading ? [MODAL_STATE.CLOSING_LOADING] : [MODAL_STATE.CLOSING]);
        JS.InvokeVoid(JSModal.Deactivate, Modal.ID, loading);
        StateHasChanged();
    }
}

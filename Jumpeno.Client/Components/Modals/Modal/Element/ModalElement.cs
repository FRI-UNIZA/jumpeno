namespace Jumpeno.Client.Components;

using System.Reflection;

public partial class ModalElement : IDisposable {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [CascadingParameter]
    public required BaseTheme Theme { get; set; }
    [Parameter]
    public required Modal Modal { get; set; }
    [Parameter]
    public required bool Inert { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public CSSClass ComputeClass() {
        var c = new CSSClass(Modal.CLASS_MODAL);
        switch (Modal.State) {
            case MODAL_STATE.LOADING:
                c.Set("loading");
            break;
            case MODAL_STATE.PRE_OPEN:
                c.Set("pre-open");
            break;
            case MODAL_STATE.CLOSING:
                c.Set("closing");
            break;
        }
        if (Modal.Unclosable) c.Set("unclosable");
        c.Set(Modal.Class);
        return c;
    }

    public CSSStyle ComputeStyle() {
        return new CSSStyle(Modal.Style);
    }

    public CSSClass ComputeDialogClass() {
        var c = new CSSClass(Modal.CLASS_DIALOG);
        if (Modal.MinWidth is not null) c.Set("min-width");
        if (Modal.MinHeight is not null) c.Set("min-height");
        return c;
    }

    public CSSStyle ComputeDialogStyle() {
        var s = new CSSStyle();
        if (Modal.MinWidth is not null) s.Set("--modal-min-width", Modal.MinWidth);
        s.Set("--modal-max-width", Modal.MaxWidth!);
        if (Modal.MinHeight is not null) s.Set("--modal-min-height", Modal.MinHeight);
        s.Set("--modal-max-height", Modal.MaxHeight!);
        return s;
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    private MODAL_STATE LastState = MODAL_STATE.CLOSED;
    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (LastState != MODAL_STATE.OPEN && Modal.State == MODAL_STATE.OPEN) {
            ModalProvider.NotifyOpen();
        }
        LastState = Modal.State;
        if (AppEnvironment.IsServer || !firstRender) return;
        await ModalProvider.AddElement(this);
        JS.InvokeVoid(JSModal.Activate, Modal.ID);
    }

    public void Dispose() {
        ModalProvider.NotifyDispose(Modal);
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public void TriggerStateChange() {
        StateHasChanged();
    }

    public void StartClosing() {
        Type type = typeof(Modal);
        PropertyInfo? prop = type.GetProperty(nameof(Modal.State));
        if (prop is null) return;
        MethodInfo? setMethod = prop.GetSetMethod(nonPublic: true);
        if (setMethod is null) return;
        setMethod.Invoke(Modal, [MODAL_STATE.CLOSING]);
        JS.InvokeVoid(JSModal.Deactivate, Modal.ID);
        StateHasChanged();
    }
}

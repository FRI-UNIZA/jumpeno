namespace Jumpeno.Client.Components;

public partial class ModalElement {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required Modal Modal { get; set; }
    [Parameter]
    public required bool Inert { get; set; }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() {
        var c = base.ComputeClass()
        .Set(Modal.ClassName, Base)
        .SetSurface(Modal.Surface);
        switch (Modal.State) {
            case ModalStateType.PreOpen:
                c.Set("pre-open");
            break;
            case ModalStateType.Loading:
                c.Set("loading");
            break;
            case ModalStateType.Closing:
                c.Set("closing");
            break;
            case ModalStateType.ClosingLoading:
                c.Set("closing"); c.Set("closing-loading");
            break;
        }
        c.Set(Modal.ClassNoHeader, Modal.NoHeader);
        c.Set(Modal.ClassNoFooter, Modal.NoFooter);
        c.Set(Modal.ClassUnclosable, Modal.Unclosable);
        c.Set(Modal.ComputeClass());
        return c;
    }

    public CSSStyle ComputeStyle() => new(Modal.Style);

    public CssClass ComputeDialogClass() => new(Modal.ClassDialog);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    private ModalStateType LastState = ModalStateType.Closed;
    protected override async Task OnComponentAfterRenderAsync(bool firstRender) {
        if (LastState != ModalStateType.Open && Modal.State == ModalStateType.Open) {
            ModalProvider.NotifyOpen();
        }
        LastState = Modal.State;
        if (AppEnvironment.IsServer || !firstRender) return;
        await ModalProvider.AddElement(this);
        JS.InvokeVoid(JSModal.Activate, Modal.Id);
    }

    protected override void OnComponentDispose() => ModalProvider.NotifyDispose(Modal);

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public void StartClosing() {
        Type type = typeof(Modal);
        PropertyInfo? prop = type.GetProperty(nameof(Modal.State));
        if (prop is null) return;
        MethodInfo? setMethod = prop.GetSetMethod(nonPublic: true);
        if (setMethod is null) return;
        var loading = Modal.State == ModalStateType.Loading;
        setMethod.Invoke(Modal, loading ? [ModalStateType.ClosingLoading] : [ModalStateType.Closing]);
        JS.InvokeVoid(JSModal.Deactivate, Modal.Id, loading);
        StateHasChanged();
    }
}

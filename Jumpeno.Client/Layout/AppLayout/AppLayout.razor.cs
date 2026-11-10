namespace Jumpeno.Client.Layouts;

public partial class AppLayout {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    private const string ClassName = "main-layout";
    private const string ClassNoNavigation = "no-navigation";
    private const string InertSelector = $"#{WebDocument.ID}";
    // Cascade:
    public const string CascadeAppLayout = $"{nameof(AppLayout)}.{nameof(CascadeAppLayout)}";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private NavMenu NavMenuRef = null!;
    private NavMenuMobile NavMenuMobileRef = null!;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() {
        return base.ComputeClass()
        .Set(ClassName, Base)
        .Set(ClassNoNavigation, !_layoutVm.NavigationDisplayed);
    }

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly AppLayoutVM _layoutVm = new();
    
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override async Task OnComponentInitializedAsync() => await Auth.Register(this);
    protected override bool ShouldComponentRender() => Auth.NotFreezed(this);
    protected override async ValueTask OnComponentDisposeAsync() => await Auth.Register(this);

    // Notification -----------------------------------------------------------------------------------------------------------------------
    private bool Key { get; set; } = false;
    private bool PageKey { get; set; } = false;
    private void NotifyState() { Layout.Current.Notify(); StateHasChanged(); }
    private void NotifyPage() { PageKey = !PageKey; NotifyState(); }
    private void NotifyAll() { Key = !Key; NotifyState(); }
    public static void Notify(NotifyType notify) {
        switch (notify) {
            case NotifyType.State: Instance().NotifyState(); break;
            case NotifyType.Page: Instance().NotifyPage(); break;
            case NotifyType.All: Instance().NotifyAll(); break;
        }
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private static void OnMobileMenuOpen() => ActionHandler.SetInert(InertSelector);
    private static void OnMobileMenuClose() => ActionHandler.RemoveInert(InertSelector);
}

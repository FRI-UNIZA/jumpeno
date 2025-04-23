namespace Jumpeno.Client.Pages;

public partial class ManualPage {
    public const string ROUTE_EN = "/en/manual";
    public const string ROUTE_SK = "/sk/navod";

    protected override void OnPageInitialized() => Console.WriteLine("ManualPage:OnPageInitialized");
    protected override async Task OnPageInitializedAsync() => Console.WriteLine("ManualPage:OnPageInitializedAsync");
    protected override void OnPageParametersSet(bool firstTime) => Console.WriteLine("ManualPage:OnPageParametersSet");
    protected override async Task OnPageParametersSetAsync(bool firstTime) => Console.WriteLine("ManualPage:OnPageParametersSetAsync");
    protected override void OnPageAfterRender(bool firstTime) => Console.WriteLine("ManualPage:OnPageAfterRender");
    protected override async Task OnPageAfterRenderAsync(bool firstTime) => Console.WriteLine("ManualPage:OnPageAfterRenderAsync");
    protected override void OnPageDispose() => Console.WriteLine("ManualPage:OnPageDispose");
    protected override async ValueTask OnPageDisposeAsync() => Console.WriteLine("ManualPage:OnPageDisposeAsync");
}

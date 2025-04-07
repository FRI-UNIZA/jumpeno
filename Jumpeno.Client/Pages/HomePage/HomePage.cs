
namespace Jumpeno.Client.Pages;

public partial class HomePage {
    public const string ROUTE_EN = "/en";
    public const string ROUTE_SK = "/sk";

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override async Task OnPageInitializedAsync() => await Auth.Activate();
}

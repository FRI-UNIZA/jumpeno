namespace Jumpeno.Client.Pages;

public partial class AdminPage {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ROUTE_EN = "/en/admin";
    public const string ROUTE_SK = "/sk/admin";

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private static async Task Start() {
        try {
            await PageLoader.Show();
            await HTTP.Patch(API.BASE.GAME_START, body: Game.MOCK_CODE);
        } catch {
        } finally {
            await PageLoader.Hide();
        }
    }

    private static async Task Reset() {
        try {
            await PageLoader.Show();
            await HTTP.Patch(API.BASE.GAME_RESET, body: Game.MOCK_CODE);
        } catch {
        } finally {
            await PageLoader.Hide();
        }
    }
}

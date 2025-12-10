namespace Jumpeno.Client.Components;

public partial class GameChat {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required GameViewModel VM { get; set; }

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private Modal ModalRef = null!;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task Open() => await ModalRef.Open();
    public async Task Close() => await ModalRef.Close();
}

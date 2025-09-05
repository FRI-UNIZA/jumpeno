namespace Jumpeno.Client.Models;

public class ModalState(Modal[] serverModals) {
    public Modal[] ServerModals { get; private set; } = serverModals;
}

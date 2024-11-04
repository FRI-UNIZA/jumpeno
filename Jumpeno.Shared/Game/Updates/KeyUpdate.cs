namespace Jumpeno.Shared.Models;

public class KeyUpdate(ulong id, byte playerID, GAME_CONTROLS key, bool pressed): GameUpdate(id) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public override string HUB_ACTION => GAME_HUB.KEY_UPDATE;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public byte PlayerID { get; private set; } = playerID;
    public GAME_CONTROLS Key { get; private set; } = key;
    public bool Pressed { get; private set; } = pressed;
}

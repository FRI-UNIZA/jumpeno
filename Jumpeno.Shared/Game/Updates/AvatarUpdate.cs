namespace Jumpeno.Shared.Models;

public class AvatarUpdate(byte playerID, Avatar avatar) {
    public byte PlayerID { get; private set; } = playerID;
    public Avatar Avatar { get; private set; } = avatar;
}

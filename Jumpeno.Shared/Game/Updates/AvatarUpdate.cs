namespace Jumpeno.Shared.Models;

public class AvatarUpdate(string connectionID, Avatar avatar) {
    public string ConnectionID { get; private set; } = connectionID;
    public Avatar Avatar { get; private set; } = avatar;
}

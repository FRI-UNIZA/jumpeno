namespace Jumpeno.Shared.Models;

public static class Exceptions {
    public static readonly CoreException BadRequest = new CoreException().SetCode(400).SetMessage("Bad request.");
    public static readonly CoreException NotAuthenticated = new CoreException().SetCode(401).SetMessage("Not authenticated.");
    public static readonly CoreException NotAuthorized = new CoreException().SetCode(403).SetMessage("Not authorized.");
}

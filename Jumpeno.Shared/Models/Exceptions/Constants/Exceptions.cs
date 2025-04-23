namespace Jumpeno.Shared.Constants;

public static class Exceptions {
    public static readonly CoreException BadRequest = new CoreException().SetCode(400).SetMessage("Bad request.");
    public static readonly CoreException NotAuthenticated = new CoreException().SetCode(401).SetMessage("Not authenticated.");
    public static readonly CoreException NotAuthorized = new CoreException().SetCode(403).SetMessage("Not authorized.");
    public static readonly CoreException NotFound = new CoreException().SetCode(404).SetMessage("Not found.");
    public static readonly CoreException InvalidToken = new CoreException().SetCode(406).SetMessage("Invalid token.");
}

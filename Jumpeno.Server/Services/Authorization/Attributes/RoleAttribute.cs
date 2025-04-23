namespace Jumpeno.Server.Utils;

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class RoleAttribute(params ROLE[] allowed) : Attribute {
    public ROLE[] Allowed { get; } = allowed;
}

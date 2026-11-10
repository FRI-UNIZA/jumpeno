namespace Jumpeno.Server.Utils;

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class RoleAttribute(params Role[] allowed) : Attribute {
    public Role[] Allowed { get; } = allowed;
}

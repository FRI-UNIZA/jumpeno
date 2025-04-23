namespace Jumpeno.Server.Utils;

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class RoleFilter : IOperationFilter {
    public void Apply(OpenApiOperation operation, OperationFilterContext context) {
        var roleAttribute = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<RoleAttribute>()
            .FirstOrDefault();

        if (roleAttribute != null) {
            var roles = roleAttribute.Allowed.Select(r => r.ToString()).ToList();
            operation.Security = [
                new() {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = AUTH.BEARER
                            }
                        },
                        roles
                    }
                }
            ];
            operation.Description ??= "";
            operation.Description += $"\n🔒 **Roles:** `{string.Join("` | `", roles)}`";
        }
    }
}

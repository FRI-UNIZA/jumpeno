namespace Jumpeno.Server.Utils;

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class ContentTypeFilter : IOperationFilter {
    public void Apply(OpenApiOperation operation, OperationFilterContext context) {
        var consumes = context.MethodInfo.GetCustomAttributes(typeof(ConsumesAttribute), false).Cast<ConsumesAttribute>().FirstOrDefault();
        var produces = context.MethodInfo.GetCustomAttributes(typeof(ProducesAttribute), false).Cast<ProducesAttribute>().FirstOrDefault();

        if (consumes == null && produces == null) {
            if (operation.RequestBody != null) {
                var schemas = operation.RequestBody.Content.ToDictionary(c => c.Key, c => c.Value.Schema);
                operation.RequestBody.Content.Clear();
                operation.RequestBody.Content.Add(CONTENT_TYPE.JSON, new OpenApiMediaType {
                    Schema = schemas.FirstOrDefault().Value
                });
            }

            foreach (var response in operation.Responses) {
                var schemas = response.Value.Content.ToDictionary(c => c.Key, c => c.Value.Schema);
                response.Value.Content.Clear();
                response.Value.Content.Add(CONTENT_TYPE.JSON, new OpenApiMediaType {
                    Schema = schemas.FirstOrDefault().Value
                });
            }
        }
    }
}

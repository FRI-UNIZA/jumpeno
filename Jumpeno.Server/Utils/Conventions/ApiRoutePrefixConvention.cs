namespace Jumpeno.Server.Utils;

public class ApiRoutePrefixConvention(string prefix) : IApplicationModelConvention {
    private readonly AttributeRouteModel Prefix = new(new RouteAttribute(prefix));

    public void Apply(ApplicationModel application) {
        foreach (var controller in application.Controllers) {
            foreach (var selector in controller.Selectors) {
                if (selector.AttributeRouteModel != null) {
                    // NOTE: Combine existing route models with the global prefix
                    selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
                        Prefix,
                        selector.AttributeRouteModel
                    );
                } else {
                    selector.AttributeRouteModel = Prefix;
                }
            }
        }
    }
}

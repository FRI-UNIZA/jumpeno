namespace Jumpeno.Server.Conventions;

public class ApiRoutePrefixConvention : IApplicationModelConvention {
    private readonly AttributeRouteModel _centralPrefix;

    public ApiRoutePrefixConvention(string prefix) {
        _centralPrefix = new AttributeRouteModel(new RouteAttribute(prefix));
    }

    public void Apply(ApplicationModel application) {
        foreach (var controller in application.Controllers) {
            foreach (var selector in controller.Selectors) {
                if (selector.AttributeRouteModel != null) {
                    // Combine existing route models with the global prefix
                    selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
                        _centralPrefix,
                        selector.AttributeRouteModel);
                } else {
                    selector.AttributeRouteModel = _centralPrefix;
                }
            }
        }
    }
}

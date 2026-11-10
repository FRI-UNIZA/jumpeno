namespace Jumpeno.Client.Pages;

public partial class DesignerPage {
    public const string RouteEN = "/en/designer";
    public const string RouteSK = "/sk/designer";
    
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassPage = "designer-page";
    public const string ClassHeadingMain = "designer-heading-main";
    public const string ClassHeading = "designer-heading";
    public const string ClassSurface = "designer-surface";
    public const string ClassContainer = "designer-container";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly List<DesignerSurface> Surfaces; 

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    private static CssClass GetClassSurface(Surface surface) => new CssClass(ClassSurface).Set(surface);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public DesignerPage() {
        // 1) Initialization:
        Dictionary<string, DesignerSurface> index = [];
        Surfaces = [];
        // 2) Store surfaces:
        foreach (Surface surface in Enum.GetValues(typeof(Surface))) {
            // 2.1) Create structure:
            var name = $"{surface}";
            var ds = new DesignerSurface(surface);
            // 2.2) Index structure:
            index[name] = ds;
            // 2.3) Get parent name:
            var parents = name.Split('_');
            parents = [.. parents.SkipLast(1)];
            var parentName = "";
            foreach (var parent in parents) {
                if (parentName != "") parentName += "_";
                parentName += $"{parent}";
            }
            // 2.4) Store structure:
            if (parentName == "") Surfaces.Add(ds);
            else index[parentName].Add(ds);
        }
    }
}

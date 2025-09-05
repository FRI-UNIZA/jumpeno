namespace Jumpeno.Client.Pages;

public partial class DesignerPage {
    public const string ROUTE_EN = "/en/designer";
    public const string ROUTE_SK = "/sk/designer";
    
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS_PAGE = "designer-page";
    public const string CLASS_HEADING_MAIN = "designer-heading-main";
    public const string CLASS_HEADING = "designer-heading";
    public const string CLASS_SURFACE = "designer-surface";
    public const string CLASS_CONTAINER = "designer-container";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly List<DesignerSurface> Surfaces; 

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    private static CSSClass ClassSurface(SURFACE surface) => new CSSClass(CLASS_SURFACE).Set(surface);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public DesignerPage() {
        // 1) Initialization:
        Dictionary<string, DesignerSurface> index = [];
        Surfaces = [];
        // 2) Store surfaces:
        foreach (SURFACE surface in Enum.GetValues(typeof(SURFACE))) {
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

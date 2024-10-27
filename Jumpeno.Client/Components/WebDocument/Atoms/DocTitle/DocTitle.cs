namespace Jumpeno.Client.Atoms;

using System.Reflection;

public partial class DocTitle {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required string Value { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private string Title { get; set; } = "";

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnParametersSet() {
        if (Title == Value) return;
        
        Type myClassType = typeof(WebDocument);
        MethodInfo? methodInfo = myClassType.GetMethod("SetTitle", BindingFlags.NonPublic | BindingFlags.Static);
        if (methodInfo == null) return;

        Title = Value;
        methodInfo.Invoke(null, [Title]);
    }
}

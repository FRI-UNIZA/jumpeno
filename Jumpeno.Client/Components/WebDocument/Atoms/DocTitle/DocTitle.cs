namespace Jumpeno.Client.Components;

using System.Reflection;

public partial class DocTitle {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required string Value { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private string Title { get; set; } = "";

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentParametersSet(bool firstTime) {
        if (Title == Value) return;
        
        Type myClassType = typeof(WebDocument);
        MethodInfo? methodInfo = myClassType.GetMethod("SetTitle", BindingFlags.NonPublic | BindingFlags.Static);
        if (methodInfo == null) return;

        Title = Value;
        methodInfo.Invoke(null, [Title]);
    }
}

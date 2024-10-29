namespace Jumpeno.Shared.Models;

[method: JsonConstructor]
public class Error(string id, string message) {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public string ID { get; set; } = id;
    public string Message { get; set; } = message;

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public Error(string message): this("", message) {}
}

namespace Jumpeno.Client.Models;

public record SelectMultiCancelEvent (
    Dictionary<string, SelectOption> Cancelled,
    Dictionary<string, SelectOption> Value
) {}

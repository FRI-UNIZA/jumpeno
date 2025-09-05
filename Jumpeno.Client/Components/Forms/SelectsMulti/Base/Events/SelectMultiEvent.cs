namespace Jumpeno.Client.Models;

public record SelectMultiEvent (
    Dictionary<string, SelectOption> Before,
    Dictionary<string, SelectOption> After
) {}

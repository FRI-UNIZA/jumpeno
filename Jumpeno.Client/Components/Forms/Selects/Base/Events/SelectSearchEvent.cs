namespace Jumpeno.Client.Models;

public record SelectSearchEvent (
    string Search,
    SelectOption Option
) {}

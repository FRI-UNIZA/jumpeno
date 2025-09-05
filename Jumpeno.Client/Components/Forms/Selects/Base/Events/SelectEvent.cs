namespace Jumpeno.Client.Models;

public record SelectEvent (
    SelectOption Before,
    SelectOption After
) {}

namespace Jumpeno.Client.Models;

public record SelectMultiOptionEvent<T>(
    SelectOption<T> Option
);

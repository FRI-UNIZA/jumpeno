namespace Jumpeno.Client.Models;

public record NavigationEvent(
    string BeforeURL,
    string AfterURL
);

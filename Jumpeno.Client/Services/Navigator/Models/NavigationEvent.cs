namespace Jumpeno.Client.Models;

public record NavigationEvent(
    bool Program,
    bool IsPopState,
    string BeforeURL,
    string AfterURL
);

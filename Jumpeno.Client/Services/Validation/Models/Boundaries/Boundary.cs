namespace Jumpeno.Client.Models;

public record Boundary<T>(
    T Value,
    bool Exclusive
);

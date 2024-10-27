namespace Jumpeno.Shared.Models;

public record Boundary<T>(
    T Value,
    bool Exclusive
);

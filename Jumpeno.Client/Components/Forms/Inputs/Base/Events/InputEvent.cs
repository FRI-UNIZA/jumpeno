namespace Jumpeno.Client.Models;

public record InputEvent<T> (
    string TextBefore, string TextAfter,
    T Before, T After
) {}

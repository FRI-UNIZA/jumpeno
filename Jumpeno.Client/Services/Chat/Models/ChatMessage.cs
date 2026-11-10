namespace Jumpeno.Client.Models;

public record ChatMessage(
    string SenderName,
    string Text,
    DateTime SentAt
);

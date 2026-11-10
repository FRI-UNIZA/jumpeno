namespace Jumpeno.Client.Models;

public record ChatMessageReceiveUpdate(
    Guid ID,
    string SenderName,
    string Text,
    DateTime SentAt
);

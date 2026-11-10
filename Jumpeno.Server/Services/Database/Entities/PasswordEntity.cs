namespace Jumpeno.Server.Models;

public class PasswordEntity {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const int HashSize = 32; // Bytes
    public const int SaltSize = 16; // Bytes

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public const string IndexId = "PRIMARY";
    [Key]
    [ForeignKey(nameof(User))]
    [Column(TypeName = "VARCHAR(255)")]
    public required string ID { get; set; }

    [Column(TypeName = "BINARY(32)")]
    public required byte[] Hash { get; set; }

    [Column(TypeName = "BINARY(16)")]
    public required byte[] Salt { get; set; }

    public required DateTime ModifiedAt { get; set; }

    // Relations --------------------------------------------------------------------------------------------------------------------------
    public required UserEntity User { get; set; }
}

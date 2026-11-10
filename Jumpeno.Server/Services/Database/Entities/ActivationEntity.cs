namespace Jumpeno.Server.Models;

public class ActivationEntity {

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public const string IndexId = "PRIMARY";
    [Key]
    [ForeignKey(nameof(User))]
    [Column(TypeName = "VARCHAR(255)")]
    public required string ID { get; set; }

    // Relations --------------------------------------------------------------------------------------------------------------------------
    public required UserEntity User { get; set; }
}

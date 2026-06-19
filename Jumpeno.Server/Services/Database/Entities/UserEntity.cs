namespace Jumpeno.Server.Models;

[Index(nameof(Email), IsUnique = true, Name = IndexEmail)]
[Index(nameof(Name), IsUnique = true, Name = IndexName)]
[Index(nameof(CreatedAt), Name = IndexCreatedAt)]
public class UserEntity {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public const string IndexId = "PRIMARY";
    [Key]
    [Column(TypeName = "VARCHAR(255)")]
    public required string Id { get; set; }

    public const string IndexEmail = "IX_User_Email";
    [Column(TypeName = "VARCHAR(255)")]
    public required string Email { get; set; }

    public const string IndexName = "IX_User_Name";
    [Column(TypeName = "VARCHAR(13)")]
    public required string Name { get; set; }

    [Column(TypeName = "INT(1)")]
    public required int Skin { get; set; }

    public required DateTime ModifiedAt { get; set; }

    public const string IndexCreatedAt = "IX_User_CreatedAt";
    public required DateTime CreatedAt { get; set; }

    // Relations --------------------------------------------------------------------------------------------------------------------------
    public ICollection<RefreshEntity> Refresh { get; set; } = [];
    public ActivationEntity? Activation { get; set; }
    public PasswordEntity? Password { get; set; }

    
}

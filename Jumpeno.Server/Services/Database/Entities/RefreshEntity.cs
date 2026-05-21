namespace Jumpeno.Server.Models;

[Index(nameof(Origin), Name = IndexOrigin)]
[Index(nameof(Expires), Name = IndexExpires)]
public class RefreshEntity {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public const string IndexToken = "PRIMARY";
    [Key]
    [Column(TypeName = "VARCHAR(512)")]
    public required string Token { get; set; }

    public const string IndexId = "IX_Refresh_ID";
    [ForeignKey(nameof(User))]
    [Column(TypeName = "VARCHAR(255)")]
    public string? Id { get; set; }

    public const string IndexOrigin = "IX_Refresh_Origin";
    [Column(TypeName = "VARCHAR(512)")]
    public string? Origin { get; set; }
    
    public const string IndexExpires = "IX_Refresh_Expires";
    public required DateTime Expires { get; set; }

    // Relations --------------------------------------------------------------------------------------------------------------------------
    public UserEntity? User { get; set; }
}

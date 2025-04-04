namespace Jumpeno.Server.Models;

[Index(nameof(Email), IsUnique = true)]
[Index(nameof(Name), IsUnique = true)]
[Index(nameof(CreatedAt))]
public class EntityUser {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    [Key]
    [Column(TypeName = "VARCHAR(255)")]
    public required string ID { get; set; }

    [Column(TypeName = "VARCHAR(254)")]
    public required string Email { get; set; }

    [Column(TypeName = "VARCHAR(30)")]
    public required string Name { get; set; }

    public required int Skin { get; set; }

    [Column(TypeName = "VARCHAR(100)")]
    public required string Password { get; set; }

    public required DateTime ModifiedAt { get; set; }

    public required DateTime CreatedAt { get; set; }

    // DML --------------------------------------------------------------------------------------------------------------------------------
    public static async Task Add(UserRegisterDTO data) {
        var db = await DB.Context();
        var time = DateTime.UtcNow;
        db.User.Add(new EntityUser() {
            ID = Guid.NewGuid().ToString(),
            Email = data.Email,
            Name = data.Name,
            Skin = (int) SKIN.MAGE_MAGIC,
            Password = data.Password,
            ModifiedAt = time,
            CreatedAt = time
        });
    }
}

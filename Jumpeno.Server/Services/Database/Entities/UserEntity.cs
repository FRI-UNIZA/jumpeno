namespace Jumpeno.Server.Models;

[Index(nameof(Email), IsUnique = true)]
[Index(nameof(Name), IsUnique = true)]
[Index(nameof(CreatedAt))]
public class UserEntity {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public const string INDEX_ID = "PRIMARY";
    [Key]
    [Column(TypeName = "VARCHAR(255)")]
    public required string ID { get; set; }

    public const string INDEX_EMAIL = "IX_User_Email";
    [Column(TypeName = "VARCHAR(255)")]
    public required string Email { get; set; }

    public const string INDEX_NAME = "IX_User_Name";
    [Column(TypeName = "VARCHAR(13)")]
    public required string Name { get; set; }

    [Column(TypeName = "INT(1)")]
    public required int Skin { get; set; }

    public required DateTime ModifiedAt { get; set; }

    public const string INDEX_CREATED_AT = "IX_User_CreatedAt";
    public required DateTime CreatedAt { get; set; }

    public ICollection<RefreshEntity> Refresh { get; set; } = [];
    public ActivationEntity? Activation { get; set; }
    public PasswordEntity? Password { get; set; }

    // Create -----------------------------------------------------------------------------------------------------------------------------
    public static async Task<UserEntity> Create(string email, string name) {
        // 1) Validation:
        var errors = UserValidator.ValidateEmail(email);
        errors.AddRange(UserValidator.ValidateName(name));
        Checker.CheckValues(errors);
        // 2) Create record:
        var at = DateTime.UtcNow;
        var record = new UserEntity() {
            ID = Guid.NewGuid().ToString(),
            Email = email,
            Name = name,
            Skin = (int) User.GenerateSkin(),
            ModifiedAt = at,
            CreatedAt = at
        };
        // 3.1) Save record:
        var ctx = await DB.Context();
        ctx.User.Add(record);
        // 3.2) Unique constraints:
        var result = await DB.Save(new() {
            { INDEX_EMAIL, new Error(UserValidator.EMAIL, Checker.FIELD_EXISTS) },
            { INDEX_NAME, new Error(UserValidator.NAME, Checker.FIELD_EXISTS) }
        });
        Checker.CheckValues(result.errors);
        // 4) Return record:
        return record;
    }

    // Read -------------------------------------------------------------------------------------------------------------------------------
    public static async Task<UserEntity?> ByID(string id) {
        // 1) Validation:
        UserValidator.CheckID(id);
        // 2) Select record:
        var ctx = await DB.Context();
        var record = await ctx.User
            .FirstOrDefaultAsync(o => o.ID == id);
        // 3) Return record:
        return record;
    }
    public static async Task<UserEntity?> ByIDLeftJoinActivation(string id) {
        // 1) Validation:
        UserValidator.CheckID(id);
        // 2) Select record:
        var ctx = await DB.Context();
        var record = ctx.User
            .Include(o => o.Activation)
            .FirstOrDefault(o => o.ID == id);
        // 3) Return record:
        return record;
    }

    public static async Task<UserEntity?> ByEmail(string email) {
        // 1) Validation:
        UserValidator.CheckEmail(email);
        // 2) Select record:
        var ctx = await DB.Context();
        var record = await ctx.User
            .FirstOrDefaultAsync(o => o.Email == email);
        // 3) Return record:
        return record;
    }
    public static async Task<UserEntity?> ByEmailLeftJoinPassword(string email) {
        // 1) Validation:
        UserValidator.CheckEmail(email);
        // 2) Select record:
        var ctx = await DB.Context();
        var record = ctx.User
            .Include(o => o.Password)
            .FirstOrDefault(o => o.Email == email);
        // 3) Return record:
        return record;
    }
}

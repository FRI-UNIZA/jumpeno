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

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static async Task<User?> SelectUser(string id) {
        var user = await ByIDLeftJoinActivation(id);
        return user != null ? new(Guid.Parse(user.ID), user.Email, user.Name, (SKIN)user.Skin, user.Activation == null) : null;
    }

    public static async Task<User> SelectCurrentUser() => await SelectUser(Token.Access.sub) ?? throw EXCEPTION.NOT_AUTHENTICATED;

    public static async Task<User> SelectCurrentActivatedUser() {
        var user = await SelectCurrentUser();
        if (!user.Activated) throw EXCEPTION.CLIENT.SetInfo("Account is not activated!");
        return user;
    }

    // Create -----------------------------------------------------------------------------------------------------------------------------
    public static async Task<UserEntity> Create(
        string email, string name,
        string emailID = "", string nameID = ""
    ) {
        // 1) Validation:
        var errors = UserValidator.ValidateEmail(email, emailID);
        errors.AddRange(UserValidator.ValidateName(name, true, nameID));
        Checker.Assert(errors, EXCEPTION.VALUES);
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
            { INDEX_EMAIL, ERROR.EXISTS.SetID(emailID) },
            { INDEX_NAME, ERROR.EXISTS.SetID(nameID) }
        });
        Checker.Assert(result.errors, EXCEPTION.VALUES);
        // 4) Return record:
        return record;
    }

    // Read -------------------------------------------------------------------------------------------------------------------------------
    public static async Task<UserEntity?> ByID(
        string id,
        string idID = ""
    ) {
        // 1) Validation:
        UserValidator.AssertID(id, idID);
        // 2) Select record:
        var ctx = await DB.Context();
        var record = await ctx.User
            .FirstOrDefaultAsync(o => o.ID == id);
        // 3) Return record:
        return record;
    }
    public static async Task<UserEntity?> ByIDLeftJoinActivation(
        string id,
        string idID = ""
    ) {
        // 1) Validation:
        UserValidator.AssertID(id, idID);
        // 2) Select record:
        var ctx = await DB.Context();
        var record = ctx.User
            .Include(o => o.Activation)
            .FirstOrDefault(o => o.ID == id);
        // 3) Return record:
        return record;
    }

    public static async Task<UserEntity?> ByEmail(
        string email,
        string emailID = ""
    ) {
        // 1) Validation:
        UserValidator.AssertEmail(email, emailID);
        // 2) Select record:
        var ctx = await DB.Context();
        var record = await ctx.User
            .FirstOrDefaultAsync(o => o.Email == email);
        // 3) Return record:
        return record;
    }
    public static async Task<UserEntity?> ByEmailLeftJoinPassword(
        string email,
        string emailID = ""
    ) {
        // 1) Validation:
        UserValidator.AssertEmail(email, emailID);
        // 2) Select record:
        var ctx = await DB.Context();
        var record = ctx.User
            .Include(o => o.Password)
            .FirstOrDefault(o => o.Email == email);
        // 3) Return record:
        return record;
    }
}

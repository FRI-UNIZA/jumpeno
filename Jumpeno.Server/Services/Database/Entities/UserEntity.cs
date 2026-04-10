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

    // Relations --------------------------------------------------------------------------------------------------------------------------
    public ICollection<RefreshEntity> Refresh { get; set; } = [];
    public ActivationEntity? Activation { get; set; }
    public PasswordEntity? Password { get; set; }

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static async Task<User?> SelectUser(string id) {
        var user = await ByIDLeftJoinActivation(id);
        return user != null ? new(Guid.Parse(user.ID), user.Email, user.Name, (Skin)user.Skin, user.Activation == null) : null;
    }

    public static async Task<User> SelectCurrentUser() => await SelectUser(Token.Access.sub) ?? throw Exceptions.NOT_AUTHENTICATED;

    public static async Task<User> SelectCurrentActivatedUser() {
        var user = await SelectCurrentUser();
        if (!user.Activated) throw Exceptions.CLIENT.SetInfo("Account is not activated!");
        return user;
    }

    // Create -----------------------------------------------------------------------------------------------------------------------------
    public static async Task<UserEntity> Create(
        // Parameters:
        string email, string name,
        // Exceptions:
        string emailID = "", string nameID = ""
    ) {
        // 1) Validation:
        var errors = UserValidator.ValidateEmail(email, emailID);
        errors.AddRange(UserValidator.ValidateName(name, true, nameID));
        Checker.Assert(errors, Exceptions.VALUES);
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
            { INDEX_EMAIL, Errors.EXISTS.SetID(emailID) },
            { INDEX_NAME, Errors.EXISTS.SetID(nameID) }
        });
        Checker.Assert(result.errors, Exceptions.VALUES);
        // 4) Return record:
        return record;
    }

    // Read -------------------------------------------------------------------------------------------------------------------------------
    public static async Task<UserEntity?> ByID(
        // Parameters:
        string id,
        // Exceptions:
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
        // Parameters:
        string id,
        // Exceptions:
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
        // Parameters:
        string email,
        // Exceptions:
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
        // Parameters:
        string email,
        // Exceptions:
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

    // Delete -----------------------------------------------------------------------------------------------------------------------------
    public static async Task<bool> Delete(
        // Parameters:
        string id,
        // Exceptions:
        string idID = ""
    ) {
        // 1) Validation:
        UserValidator.AssertID(id, idID);
        // 2) Delete record:
        var ctx = await DB.Context();
        var afectedRows= await ctx.User
            .Where(x => x.ID == id)
            .ExecuteDeleteAsync();
        // 3) True if deleted:
        return afectedRows > 0;
    }

    // Update -----------------------------------------------------------------------------------------------------------------------------
    public static async Task<bool> Modify(
        // Parameters:
        string id,
        string? email = null,
        string? name = null,
        Skin? skin = null,
        // Exceptions:
        string idID = "",
        string emailID = "",
        string nameID = "",
        string skinID = ""
    ) {
        // 1) Validation:
        var errors = new List<Error>();
        errors.AddRange(UserValidator.ValidateID(id, idID));
        if (email is not null) errors.AddRange(UserValidator.ValidateEmail(email, emailID));
        if (name is not null) errors.AddRange(UserValidator.ValidateName(name, false, nameID));
        if (skin is not null) errors.AddRange(UserValidator.ValidateSkin(skin, skinID));
        Checker.Assert(errors, Exceptions.VALUES);
    
        // 2) Modify record:
        var ctx = await DB.Context();
        var result = await DB.Update(async () => await ctx.User
            .Where(o => o.ID == id)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(o => o.Email, o => email ?? o.Email)
                .SetProperty(o => o.Name, o => name ?? o.Name)
                .SetProperty(o => o.Skin, o => skin != null ? (int)skin : o.Skin)
                .SetProperty(o => o.ModifiedAt, o => DateTime.UtcNow)
        ), new() {
            { INDEX_EMAIL, Errors.EXISTS.SetID(emailID) },
            { INDEX_NAME, Errors.EXISTS.SetID(nameID) }
        }
        );
    
        // 3) Unique constraints:
        Checker.Assert(result.errors, Exceptions.VALUES);

        // 4) True if modified:
        return result.rows > 0;
    }
}

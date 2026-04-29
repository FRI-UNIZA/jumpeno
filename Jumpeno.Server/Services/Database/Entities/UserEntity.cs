namespace Jumpeno.Server.Models;

[Index(nameof(Email), IsUnique = true)]
[Index(nameof(Name), IsUnique = true)]
[Index(nameof(CreatedAt))]
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

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static async Task<User?> SelectUser(string id) {
        var user = await ByIDLeftJoinActivation(id);
        return user != null ? new(Guid.Parse(user.Id), user.Email, user.Name, (Skin)user.Skin, user.Activation == null) : null;
    }

    public static async Task<User> SelectCurrentUser() => await SelectUser(Token.Access.sub) ?? throw Exceptions.NotAuthenticated;

    public static async Task<User> SelectCurrentActivatedUser() {
        var user = await SelectCurrentUser();
        if (!user.Activated) throw Exceptions.Client.SetInfo("Account is not activated!");
        return user;
    }

    // Create -----------------------------------------------------------------------------------------------------------------------------
    public static async Task<UserEntity> Create(
        // Parameters:
        string email, string name,
        // Exceptions:
        string emailID = "", string nameId = ""
    ) {
        // 1) Validation:
        var errors = UserValidator.ValidateEmail(email, emailID);
        errors.AddRange(UserValidator.ValidateName(name, true, nameId));
        Checker.Assert(errors, Exceptions.Values);
        // 2) Create record:
        var at = DateTime.UtcNow;
        var record = new UserEntity() {
            Id = Guid.NewGuid().ToString(),
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
            { IndexEmail, Errors.Exists.SetID(emailID) },
            { IndexName, Errors.Exists.SetID(nameId) }
        });
        Checker.Assert(result.errors, Exceptions.Values);
        // 4) Return record:
        return record;
    }

    // Read -------------------------------------------------------------------------------------------------------------------------------
    public static async Task<UserEntity?> ByID(
        // Parameters:
        string id,
        // Exceptions:
        string idId = ""
    ) {
        // 1) Validation:
        UserValidator.AssertID(id, idId);
        // 2) Select record:
        var ctx = await DB.Context();
        var record = await ctx.User
            .FirstOrDefaultAsync(o => o.Id == id);
        // 3) Return record:
        return record;
    }

    public static async Task<UserEntity?> ByIDLeftJoinActivation(
        // Parameters:
        string id,
        // Exceptions:
        string idId = ""
    ) {
        // 1) Validation:
        UserValidator.AssertID(id, idId);
        // 2) Select record:
        var ctx = await DB.Context();
        var record = ctx.User
            .Include(o => o.Activation)
            .FirstOrDefault(o => o.Id == id);
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
            .Where(x => x.Id == id)
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
        string emailId = "",
        string nameId = "",
        string skinId = ""
    ) {
        // 1) Validation:
        var errors = new List<Error>();
        errors.AddRange(UserValidator.ValidateID(id, idID));
        if (email is not null) errors.AddRange(UserValidator.ValidateEmail(email, emailId));
        if (name is not null) errors.AddRange(UserValidator.ValidateName(name, false, nameId));
        if (skin is not null) errors.AddRange(UserValidator.ValidateSkin(skin, skinId));
        Checker.Assert(errors, Exceptions.Values);
    
        // 2) Modify record:
        var ctx = await DB.Context();
        var result = await DB.Update(async () => await ctx.User
            .Where(o => o.Id == id)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(o => o.Email, o => email ?? o.Email)
                .SetProperty(o => o.Name, o => name ?? o.Name)
                .SetProperty(o => o.Skin, o => skin != null ? (int)skin : o.Skin)
                .SetProperty(o => o.ModifiedAt, o => DateTime.UtcNow)
        ), new() {
            { IndexEmail, Errors.Exists.SetID(emailId) },
            { IndexName, Errors.Exists.SetID(nameId) }
        }
        );
    
        // 3) Unique constraints:
        Checker.Assert(result.errors, Exceptions.Values);

        // 4) True if modified:
        return result.rows > 0;
    }
}

namespace Jumpeno.Server.Services;

public class UserService(DB dbContext, TransactionService transactionService, RequestStorage requestStorage) 
{
    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public async Task<User?> SelectUser(string id)
    {
        var user = await ByIDLeftJoinActivation(id);
        return user != null ? new(Guid.Parse(user.Id), user.Email, user.Name, (Skin)user.Skin, user.Activation == null) : null;
    }

    public async Task<User> SelectCurrentUser()
    {
        var token = requestStorage.Get<Token.Data>(RequestStorageKeys.TokenAccess)?.sub ?? throw Exceptions.NotAuthenticated;
        return await SelectUser(token) ?? throw Exceptions.NotAuthenticated;
    }

    public async Task<User> SelectCurrentActivatedUser()
    {
        var user = await SelectCurrentUser();
        if (!user.Activated) throw Exceptions.Client.SetInfo("Account is not activated!");
        return user;
    }

    // Create -----------------------------------------------------------------------------------------------------------------------------
    public async Task<UserEntity> Create(
        // Parameters:
        string email, string name,
        // Exceptions:
        string emailID = "", string nameId = ""
    )
    {
        // 1) Validation:
        var errors = UserValidator.ValidateEmail(email, emailID);
        errors.AddRange(UserValidator.ValidateName(name, true, nameId));
        Checker.Assert(errors, Exceptions.Values);
        // 2) Create record:
        var at = DateTime.UtcNow;
        var record = new UserEntity()
        {
            Id = Guid.NewGuid().ToString(),
            Email = email,
            Name = name,
            Skin = (int)User.GenerateSkin(),
            ModifiedAt = at,
            CreatedAt = at
        };
        // 3.1) Save record:
        dbContext.User.Add(record);
        // 3.2) Unique constraints:
        var result = await transactionService.SaveWithDuplicationCheck(new() {
            { UserEntity.IndexEmail, Errors.Exists.SetID(emailID) },
            { UserEntity.IndexName, Errors.Exists.SetID(nameId) }
        });
        Checker.Assert(result.errors, Exceptions.Values);
        // 4) Return record:
        return record;
    }

    // Read -------------------------------------------------------------------------------------------------------------------------------
    public async Task<UserEntity?> ByID(
        // Parameters:
        string id,
        // Exceptions:
        string idId = ""
    )
    {
        // 1) Validation:
        UserValidator.AssertID(id, idId);
        // 2) Select record:
        var record = await dbContext.User
            .FirstOrDefaultAsync(o => o.Id == id);
        // 3) Return record:
        return record;
    }

    public async Task<UserEntity?> ByIDLeftJoinActivation(
        // Parameters:
        string id,
        // Exceptions:
        string idId = ""
    )
    {
        // 1) Validation:
        UserValidator.AssertID(id, idId);
        // 2) Select record:
        var record = await dbContext.User
            .Include(o => o.Activation)
            .FirstOrDefaultAsync(o => o.Id == id);
        // 3) Return record:
        return record;
    }

    public async Task<UserEntity?> ByEmail(
        // Parameters:
        string email,
        // Exceptions:
        string emailID = ""
    )
    {
        // 1) Validation:
        UserValidator.AssertEmail(email, emailID);
        // 2) Select record:
        var record = await dbContext.User
            .FirstOrDefaultAsync(o => o.Email == email);
        // 3) Return record:
        return record;
    }

    public async Task<UserEntity?> ByEmailLeftJoinPassword(
        // Parameters:
        string email,
        // Exceptions:
        string emailID = ""
    )
    {
        // 1) Validation:
        UserValidator.AssertEmail(email, emailID);
        // 2) Select record:
        var record = await dbContext.User
            .Include(o => o.Password)
            .FirstOrDefaultAsync(o => o.Email == email);
        // 3) Return record:
        return record;
    }

    // Delete -----------------------------------------------------------------------------------------------------------------------------
    public async Task<bool> Delete(
        // Parameters:
        string id,
        // Exceptions:
        string idID = ""
    )
    {
        // 1) Validation:
        UserValidator.AssertID(id, idID);
        // 2) Delete record:
        var afectedRows = await dbContext.User
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync();
        // 3) True if deleted:
        return afectedRows > 0;
    }

    // Update -----------------------------------------------------------------------------------------------------------------------------
    public async Task<bool> Modify(
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
    )
    {
        // 1) Validation:
        var errors = new List<Error>();
        errors.AddRange(UserValidator.ValidateID(id, idID));
        if (email is not null) errors.AddRange(UserValidator.ValidateEmail(email, emailId));
        if (name is not null) errors.AddRange(UserValidator.ValidateName(name, false, nameId));
        if (skin is not null) errors.AddRange(UserValidator.ValidateSkin(skin, skinId));
        Checker.Assert(errors, Exceptions.Values);

        // 2) Modify record:
        var result = await transactionService.UpdateWithDuplicationCheck(async () => await dbContext.User
            .Where(o => o.Id == id)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(o => o.Email, o => email ?? o.Email)
                .SetProperty(o => o.Name, o => name ?? o.Name)
                .SetProperty(o => o.Skin, o => skin != null ? (int)skin : o.Skin)
                .SetProperty(o => o.ModifiedAt, o => DateTime.UtcNow)
        ), new() {
            { UserEntity.IndexEmail, Errors.Exists.SetID(emailId) },
            { UserEntity.IndexName, Errors.Exists.SetID(nameId) }
        }
        );

        // 3) Unique constraints:
        Checker.Assert(result.errors, Exceptions.Values);

        // 4) True if modified:
        return result.rows > 0;
    }
}

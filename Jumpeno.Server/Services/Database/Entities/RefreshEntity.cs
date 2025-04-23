namespace Jumpeno.Server.Models;

[Index(nameof(Origin))]
[Index(nameof(Expires))]
public class RefreshEntity {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public const string INDEX_TOKEN = "PRIMARY";
    [Key]
    [Column(TypeName = "VARCHAR(512)")]
    public required string Token { get; set; }

    public const string INDEX_ID = "IX_Refresh_ID";
    [ForeignKey(nameof(User))]
    [Column(TypeName = "VARCHAR(255)")]
    public string? ID { get; set; }
    public UserEntity? User { get; set; }

    public const string INDEX_ORIGIN = "IX_Refresh_Origin";
    [Column(TypeName = "VARCHAR(512)")]
    public string? Origin { get; set; }
    
    public const string INDEX_EXPIRES = "IX_Refresh_Expires";
    public required DateTime Expires { get; set; }

    // Create -----------------------------------------------------------------------------------------------------------------------------
    public static async Task<RefreshEntity> Create(string token, string? id = null, string? origin = null) {
        // 1) Validation:
        var errors = TokenValidator.ValidateToken(token);
        if (id != null) errors.AddRange(UserValidator.ValidateID(id));
        Checker.CheckValues(errors);
        if (token == origin) throw new InvalidDataException($"Parameter \"{nameof(token)}\" equals \"{nameof(origin)}\".");
        // 2) Read token:
        var data = Shared.Utils.Token.Decode(token) ?? throw Exceptions.NotAuthenticated;
        if (id != null && id != data.sub) throw new InvalidDataException(nameof(UserEntity.ID));
        // 3) Create record:
        var record = new RefreshEntity() {
            Token = token,
            ID = id,
            Origin = origin,
            Expires = data.exp
        };
        // 4) Save record:
        var ctx = await DB.Context();
        ctx.Refresh.Add(record);
        await DB.Save();
        // 5) Return record:
        return record;
    }

    // Read -------------------------------------------------------------------------------------------------------------------------------
    public static async Task<bool> IsValid(string token) {
        // 1) Validation:
        TokenValidator.CheckToken(token);
        // 2) Select record:
        var ctx = await DB.Context();
        var record = await ctx.Refresh
            .FirstOrDefaultAsync(
                o => o.Token == token
                && o.Expires > DateTime.UtcNow
            );
        // 3) True if valid:
        return record != null;
    }

    public static async Task<RefreshEntity?> ByToken(string token) {
        // 1) Validation:
        TokenValidator.CheckToken(token);
        // 2) Select record:
        var ctx = await DB.Context();
        var record = await ctx.Refresh
            .FirstOrDefaultAsync(o => o.Token == token);
        // 3) Return record:
        return record;
    }

    // Delete -----------------------------------------------------------------------------------------------------------------------------
    public static async Task<bool> Delete(string token) {
        // 1) Validation:
        TokenValidator.CheckToken(token);
        // 2) Delete record:
        var ctx = await DB.Context();
        int rows = await ctx.Refresh
            .Where(o => o.Token == token)
            .ExecuteDeleteAsync();
        // 3) True if deleted:
        return rows > 0;
    }

    public static async Task<bool> DeleteByOrigin(string origin, string? except = null) {
        // 1) Validation:
        var errors = TokenValidator.ValidateToken(origin);
        if (except != null) errors.AddRange(TokenValidator.ValidateToken(except));
        Checker.CheckValues(errors);
        // 2) Delete records:
        var ctx = await DB.Context();
        int rows = await ctx.Refresh
            .Where(o => o.Origin == origin)
            .Where(o => o.Token != except)
            .ExecuteDeleteAsync();
        // 3) True if deleted:
        return rows > 0;
    }

    public static async Task<bool> DeleteExpired() {
        // 1) Delete records:
        var ctx = await DB.Context();
        int rows = await ctx.Refresh
            .Where(o => o.Expires <= DateTime.UtcNow)
            .ExecuteDeleteAsync();
        // 2) True if deleted:
        return rows > 0;
    }
}

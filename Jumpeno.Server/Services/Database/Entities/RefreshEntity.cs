namespace Jumpeno.Server.Models;

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
    public required string ID { get; set; }
    public required UserEntity User { get; set; }
    
    public const string INDEX_EXPIRES = "IX_Refresh_Expires";
    public required DateTime Expires { get; set; }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static async Task<RefreshEntity> Create(UserEntity user, string token) {
        // 1) Validation:
        var errors = TokenValidator.ValidateToken(token);
        Checker.CheckValues(errors);
        // 2) Read token:
        var data = Shared.Utils.Token.Decode(token) ?? throw Exceptions.NotAuthenticated;
        if (user.ID != data.sub) throw new InvalidDataException(nameof(UserEntity.ID));
        // 3) Create record:
        var record = new RefreshEntity() {
            Token = token,
            ID = user.ID,
            User = user,
            Expires = DateTimeOffset.FromUnixTimeSeconds(data.exp).UtcDateTime
        };
        // 4) Save record:
        var ctx = await DB.Context();
        ctx.Refresh.Add(record);
        await DB.Save();
        // 5) Return record:
        return record;
    }
}

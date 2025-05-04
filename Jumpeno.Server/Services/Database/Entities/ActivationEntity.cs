namespace Jumpeno.Server.Models;

public class ActivationEntity {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static readonly int EXPIRES = From.HourToMS(ServerSettings.Expiration.ActivationToken.Hours); // ms

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public const string INDEX_ID = "PRIMARY";
    [Key]
    [ForeignKey(nameof(User))]
    [Column(TypeName = "VARCHAR(255)")]
    public required string ID { get; set; }
    public required UserEntity User { get; set; }

    // Create -----------------------------------------------------------------------------------------------------------------------------
    public static async Task<ActivationEntity> Create(
        string id,
        string idID = ""
    ) {
        // 1) Validation:
        UserValidator.AssertID(id, idID);
        // 2) Create record:
        var record = new ActivationEntity() {
            ID = id,
            User = null!
        };
        // 3) Save record:
        var ctx = await DB.Context();
        ctx.Activation.Add(record);
        await DB.Save();
        // 4) Return record:
        return record;
    }

    // Delete -----------------------------------------------------------------------------------------------------------------------------
    public static async Task<bool> Delete(
        string id,
        string idID = ""
    ) {
        // 1) Validation:
        UserValidator.AssertID(id, idID);
        // 2) Delete record:
        var ctx = await DB.Context();
        int rows = await ctx.Activation
            .Where(o => o.ID == id)
            .ExecuteDeleteAsync();
        // 3) True if deleted:
        return rows > 0;
    }

    public static async Task<bool> DeleteExpired() {
        // 1) Delete records:
        var ctx = await DB.Context();
        int rows = await ctx.User
            .Where(o => o.Activation != null)
            .Where(o => o.CreatedAt < DateTime.UtcNow.AddMilliseconds(-EXPIRES))
            .ExecuteDeleteAsync();
        // 2) True if deleted:
        return rows > 0;
    }
}

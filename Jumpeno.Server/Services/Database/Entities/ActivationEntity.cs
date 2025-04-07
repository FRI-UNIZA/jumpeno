namespace Jumpeno.Server.Models;

public class ActivationEntity {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public const string INDEX_ID = "PRIMARY";
    [Key]
    [ForeignKey(nameof(User))]
    [Column(TypeName = "VARCHAR(255)")]
    public required string ID { get; set; }
    public required UserEntity User { get; set; }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static async Task<ActivationEntity> Create(UserEntity user) {
        // 1) Validation:
        var errors = UserValidator.ValidateID(user.ID);
        Checker.CheckValues(errors);
        // 2) Create record:
        var record = new ActivationEntity() {
            ID = user.ID,
            User = user
        };
        // 3) Save record:
        var ctx = await DB.Context();
        ctx.Activation.Add(record);
        await DB.Save();
        // 4) Return record:
        return record;
    }

    public static async Task<bool> Delete(string id) {
        // 1) Validation:
        var errors = UserValidator.ValidateID(id);
        Checker.CheckValues(errors);
        // 2) Delete record:
        var ctx = await DB.Context();
        int rows = await ctx.Activation
        .Where(o => o.ID == id)
        .ExecuteDeleteAsync();
        // 3) True if deleted:
        return rows > 0;
    }
}

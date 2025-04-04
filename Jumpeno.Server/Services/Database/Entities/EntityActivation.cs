namespace Jumpeno.Server.Models;

public class EntityActivation {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    [Key]
    [ForeignKey(nameof(User))]
    [Column(TypeName = "VARCHAR(255)")]
    public required string ID { get; set; }
    
    public required EntityUser User { get; set; }
}

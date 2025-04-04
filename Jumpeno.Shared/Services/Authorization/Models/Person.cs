using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jumpeno.Shared.Models;

public class Person {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Skin { get; set; }
    public bool Trust { get; set; }
    public string? ActivationCode { get; set; }
}

namespace Jumpeno.Shared.Models;

public class Person {
    public int Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Skin { get; set; }
    public bool Trust { get; set; }
    public string? ActivationCode { get; set; }
}

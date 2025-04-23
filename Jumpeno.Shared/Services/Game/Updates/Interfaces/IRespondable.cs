namespace Jumpeno.Shared.Models;

public interface IRespondable<R> {
    [JsonIgnore]
    public R Response { get; set; }
}

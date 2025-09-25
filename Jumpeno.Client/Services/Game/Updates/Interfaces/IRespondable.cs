namespace Jumpeno.Client.Models;

public interface IRespondable<R> {
    [JsonIgnore]
    public R Response { get; set; }
}

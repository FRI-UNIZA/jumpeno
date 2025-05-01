namespace Jumpeno.Client.Models;

public class SSRState<T>(COMPONENT_STATE state, T data) {
    public required COMPONENT_STATE State { get; set; } = state;
    public T Data { get; set; } = data;
}

namespace Jumpeno.Client.Interfaces;

public interface IValidable<T> {
    public abstract List<Error> Validate();
    public abstract T Assert(AppException? exception = null);
}

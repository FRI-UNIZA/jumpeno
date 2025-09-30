namespace Jumpeno.Client.Models;

public class HTTPField<T> {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public T? Data { get; set; } = (T?)(object?)null;
    public bool Error { get; set; } = false;
    public bool Loading { get; set; } = false;

    // Load -------------------------------------------------------------------------------------------------------------------------------
    public async Task Load(Component component, Func<Task<HTTPResult<T>>> request) {
        try {
            Loading = true;
            component.Notify();
            var body = (await request()).Body;
            Data = body is IValidable<T> dto ? dto.Assert() : body;
            Error = false;
        } catch {
            Data = (T?)(object?)null;
            Error = true;
            throw;
        } finally {
            Loading = false;
            component.Notify();
        }
    }
    
    // Load [SSR] -------------------------------------------------------------------------------------------------------------------------
    public async Task Load(Component component, Func<Task<HTTPResult<T>>> request, HTTPField<T> field) {
        try {
            Loading = true;
            component.Notify();
            if (field.Error) {
                Data = (T?)(object?)null;
                Error = true;
            } else {
                var body = field.Data ?? (await request()).Body;
                Data = body is IValidable<T> dto ? dto.Assert() : body;
                Error = false;
            }
        } catch {
            Data = (T?)(object?)null;
            Error = true;
            throw;
        } finally {
            Loading = false;
            component.Notify();
            SSRStorage.Commit(field, this);
        }
    }
}

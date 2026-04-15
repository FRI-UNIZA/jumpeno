namespace Jumpeno.Client.Services;

public class RequestStorage(IHttpContextAccessor httpContextAccessor) {
    
    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public T? Get<T>(string key) {
        Checker.CheckEmptyString(key, name: "key");
        var context = httpContextAccessor.HttpContext;

        if (context?.Items.TryGetValue(key, out object? value) == true && value is T typedValue) 
            return typedValue;
        
        return default;
    }

    public T Access<T>(string key, T initial) {
        var data = Get<T>(key);
        if (data != null) return data;
        
        data = initial!;
        Set(key, data);
        return data;
    }

    public void Set(string key, object o) {
        Checker.CheckEmptyString(key, name: "key");
        var context = httpContextAccessor.HttpContext ?? throw new InvalidOperationException("No HttpContext available.");
    
        context.Items[key] = o;
    }

    public bool Delete(string key) {
        Checker.CheckEmptyString(key, name: "key");
        var context = httpContextAccessor.HttpContext ?? throw new InvalidOperationException("No HttpContext available.");
        return context.Items.Remove(key);
    }
}

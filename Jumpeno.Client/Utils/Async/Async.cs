namespace Jumpeno.Client.Utils;

public static class Async {
    public static void Fire(Func<Task> action) => action();    
}

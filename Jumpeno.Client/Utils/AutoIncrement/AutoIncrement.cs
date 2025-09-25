namespace Jumpeno.Client.Utils;

public class AutoIncrement {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private long Value;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public AutoIncrement(long value) => Value = value;
    public AutoIncrement() : this(0) {}

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public long Current() => Value;
    public long Next() => ++Value;
}

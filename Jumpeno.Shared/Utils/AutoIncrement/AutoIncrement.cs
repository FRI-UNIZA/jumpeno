namespace Jumpeno.Client.Utils;

public class AutoIncrement {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private long Value;

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public AutoIncrement(long value) {
        Value = value;
    }
    public AutoIncrement(): this(0) {}

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public long Current() { return Value; }
    public long Next() { return ++Value; }
}

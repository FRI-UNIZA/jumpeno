namespace Jumpeno.Server.Services;

using Microsoft.Extensions.Caching.Memory;

public class AttemptService(IMemoryCache cache, IHttpContextAccessor httpContext) : IDisposable
{
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string IpUnknown = "unknown";

    // User block threshold for 1 minute:
    public const int EmailBlockThreshold = 4;

    // IP block thresholds per category for 1 minute:
    public readonly Dictionary<AttemptsCategory, int> IpBlockTreshold = new()
    {
        { AttemptsCategory.Login, 10 },
        { AttemptsCategory.Register, 7 }
    };

    // Structures -------------------------------------------------------------------------------------------------------------------------
    /// <summary>Thread-safe counter for IP attempts. Its purpose is to allow atomic increments with no overflow.</summary>
    private class Counter
    {
        public int Count { get; private set; } = 0;

        public int Increment()
        {
            if (Count >= int.MaxValue) return Count;
            return ++Count;
        }
    }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly Locker Lock = new();

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public void Dispose() {
        Lock.DisposeSafe();
        GC.SuppressFinalize(this);
    }

    // Actions [email] --------------------------------------------------------------------------------------------------------------------
    private int IncrementFailedEmail(string email)
    {
        var key = MemoryCaches.USER_ATTEMPT(email);

        return Lock.Exclusive(() => {
            var counter = cache.GetOrCreate(key, e => 
            {
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return new Counter();
            })!;
            return counter.Increment();
        });
    }

    public bool IncrementAndCheckIfEmailBlocked(string email)
    {
        int count = IncrementFailedEmail(email);
        return count >= EmailBlockThreshold;
    }

    // Actions [IP] -----------------------------------------------------------------------------------------------------------------------
    private int IncrementFailedIp(AttemptsCategory category)
    {
        string ip = httpContext.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? IpUnknown;
        var key = MemoryCaches.IP_ATTEMPT(category, ip);
        
        return Lock.Exclusive(() => {
            var counter = cache.GetOrCreate(key, e => 
            {
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return new Counter();
            })!;
            return counter.Increment();
        });
    }

    public bool IncrementAndCheckIfIpBlocked(AttemptsCategory category)
    {
        int count = IncrementFailedIp(category);
        return count >= IpBlockTreshold[category];
    }
}

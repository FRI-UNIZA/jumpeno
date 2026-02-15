namespace Jumpeno.Server.Services;

using Microsoft.Extensions.Caching.Memory;

public class AttemptService(IMemoryCache cache, IHttpContextAccessor httpContext) : IDisposable
{
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string IP_UNKNOWN = "unknown";

    // User block threshold for 1 minute:
    public const int EMAIL_BLOCK_THRESHOLD = 4;

    // IP block thresholds per category for 1 minute:
    public readonly Dictionary<ATTEMPTS_CATEGORY, int> IP_BLOCK_TRESHOLD = new()
    {
        { ATTEMPTS_CATEGORY.LOGIN, 10 },
        { ATTEMPTS_CATEGORY.REGISTER, 7 }
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
        var key = MEMORY_CACHE.USER_ATTEMPT(email);

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
        return count >= EMAIL_BLOCK_THRESHOLD;
    }

    // Actions [IP] -----------------------------------------------------------------------------------------------------------------------
    private int IncrementFailedIP(ATTEMPTS_CATEGORY category)
    {
        string ip = httpContext.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? IP_UNKNOWN;
        var key = MEMORY_CACHE.IP_ATTEMPT(category, ip);
        
        return Lock.Exclusive(() => {
            var counter = cache.GetOrCreate(key, e => 
            {
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return new Counter();
            })!;
            return counter.Increment();
        });
    }

    public bool IncrementAndCheckIfIPBlocked(ATTEMPTS_CATEGORY category)
    {
        int count = IncrementFailedIP(category);
        return count >= IP_BLOCK_TRESHOLD[category];
    }
}

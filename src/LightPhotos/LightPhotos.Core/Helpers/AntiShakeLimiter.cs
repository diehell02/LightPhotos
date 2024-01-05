using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace LightPhotos.Core.Helpers;
public class AntiShakeLimiter(TimeSpan period)
{
    private readonly RateLimiter _limiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions()
    {
        TokenLimit = 1,
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        QueueLimit = 1,
        ReplenishmentPeriod = period,
        TokensPerPeriod = 1,
        AutoReplenishment = true
    });

    private System.Timers.Timer? _timer = null;
    private readonly object _syncLock = new();

    public bool Execute(Action action)
    {
        if (_limiter.AttemptAcquire().IsAcquired)
        {
            action();
            return true;
        }
        else
        {
            lock (_syncLock)
            {
                _timer?.Stop();
                _timer = new System.Timers.Timer(period);
                _timer.Elapsed += (s, e) =>
                {
                    lock (_syncLock)
                    {
                        _timer?.Stop();
                        action();
                    }
                };
                _timer.Start();
            }
        }
        return false;
    }
}

using System.Diagnostics.Metrics;

namespace Branch.Platform.Application.Orders;

public interface ICacheMetrics
{
    void RecordHit();
    void RecordMiss();
}

public sealed class CacheMetrics : ICacheMetrics
{
    private static readonly Meter Meter = new("Branch.Platform.Cache");
    private static readonly Counter<long> HitsCounter = Meter.CreateCounter<long>("cache_hits");
    private static readonly Counter<long> MissesCounter = Meter.CreateCounter<long>("cache_misses");

    public void RecordHit() => HitsCounter.Add(1);
    public void RecordMiss() => MissesCounter.Add(1);
}

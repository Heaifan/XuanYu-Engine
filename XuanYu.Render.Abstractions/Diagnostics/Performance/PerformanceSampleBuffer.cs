using System.Threading.Channels;

namespace XuanYu.Render.Abstractions.Diagnostics.Performance;

public sealed class PerformanceSampleBuffer
{
    readonly Channel<PerformanceFrameSample> _channel;
    readonly bool _enabled;
    string? _sessionId;
    long _dropped;
    long _nextSequence;

    public PerformanceSampleBuffer(int capacity, bool enabled = true)
    {
        if (capacity < 1) throw new ArgumentOutOfRangeException(nameof(capacity));
        _enabled = enabled;
        _channel = Channel.CreateBounded<PerformanceFrameSample>(
            new BoundedChannelOptions(capacity)
            { FullMode = BoundedChannelFullMode.Wait, SingleReader = true, SingleWriter = false });
        Capacity = capacity;
    }

    public int Capacity { get; }
    public bool Enabled => _enabled;
    public long DroppedCount => Interlocked.Read(ref _dropped);

    public bool TryWrite(PerformanceFrameSample sample)
    {
        ArgumentNullException.ThrowIfNull(sample);
        if (!_enabled) return false;
        var sessionId = Interlocked.CompareExchange(
            ref _sessionId, sample.Identity.SessionId, null) ?? sample.Identity.SessionId;
        if (!StringComparer.Ordinal.Equals(sessionId, sample.Identity.SessionId))
            throw new InvalidOperationException(
                "A performance sample buffer belongs to one session.");
        var sequenced = sample with
        { SampleSequence = Interlocked.Increment(ref _nextSequence) };
        if (_channel.Writer.TryWrite(sequenced)) return true;
        Interlocked.Increment(ref _dropped);
        return false;
    }

    public IReadOnlyList<PerformanceFrameSample> Drain()
    {
        var rows = new List<PerformanceFrameSample>();
        while (_channel.Reader.TryRead(out var sample)) rows.Add(sample);
        return rows;
    }
}

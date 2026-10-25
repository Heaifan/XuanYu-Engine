using System.Diagnostics;

namespace XuanYu.Render.Abstractions.Diagnostics.Performance;

public static class PerformanceClock
{
    public static long Frequency => Stopwatch.Frequency;
    public static long GetTimestamp() => Stopwatch.GetTimestamp();

    public static double? ElapsedNanoseconds(long start, long end, long frequency)
    {
        if (frequency <= 0 || end < start) return null;
        return ((double)(end - start) / frequency) * 1_000_000_000d;
    }

    public static PerformanceTimingScope Begin(PerformanceSampleBuffer? buffer,
        PerformanceFrameIdentity identity, PerformanceMetric metric) =>
        buffer?.Enabled == true ? new(buffer, identity, metric) : default;
}

public readonly struct PerformanceTimingScope : IDisposable
{
    readonly PerformanceSampleBuffer? _buffer;
    readonly PerformanceFrameIdentity? _identity;
    readonly PerformanceMetric _metric;
    readonly long _start;
    readonly int _thread;

    internal PerformanceTimingScope(PerformanceSampleBuffer buffer,
        PerformanceFrameIdentity identity, PerformanceMetric metric)
    {
        _buffer = buffer; _identity = identity; _metric = metric;
        _start = PerformanceClock.GetTimestamp(); _thread = Environment.CurrentManagedThreadId;
    }

    public void Dispose()
    {
        if (_buffer is null || _identity is null) return;
        var end = PerformanceClock.GetTimestamp();
        var value = PerformanceClock.ElapsedNanoseconds(_start, end, PerformanceClock.Frequency);
        if (!value.HasValue) return;
        _buffer.TryWrite(PerformanceFrameSample.Valid(_identity, _metric, value.Value,
            PerformanceUnit.Nanoseconds, _thread, Environment.CurrentManagedThreadId,
            _start, end, PerformanceClock.Frequency, 0, 0));
    }
}

public sealed record GpuTimestampFrameToken(PerformanceFrameIdentity Identity,
    uint StartQueryIndex, uint EndQueryIndex);

public static class GpuTimestampConverter
{
    public static PerformanceFrameSample CreateSample(GpuTimestampFrameToken token,
        ulong start, ulong end, uint validBits, double periodNanoseconds, bool queryReady)
    {
        ArgumentNullException.ThrowIfNull(token);
        if (!queryReady)
            return PerformanceFrameSample.Unavailable(token.Identity,
                PerformanceMetric.GpuRenderPass,
                PerformanceSampleStatus.Missing, "TimestampQueryNotReady");
        if (validBits == 0 || !double.IsFinite(periodNanoseconds) ||
            periodNanoseconds <= 0)
            return PerformanceFrameSample.Unavailable(token.Identity,
                PerformanceMetric.GpuRenderPass,
                PerformanceSampleStatus.Unsupported, "TimestampUnavailable");
        if (validBits > 64)
            return PerformanceFrameSample.Unavailable(token.Identity,
                PerformanceMetric.GpuRenderPass,
                PerformanceSampleStatus.Invalid, "TimestampValidBitsOutOfRange");
        var delta = Delta(start, end, validBits);
        return PerformanceFrameSample.Valid(token.Identity, PerformanceMetric.GpuRenderPass,
            delta * periodNanoseconds, PerformanceUnit.Nanoseconds, 0, 0,
            unchecked((long)start), unchecked((long)end), 0, periodNanoseconds, validBits);
    }

    static ulong Delta(ulong start, ulong end, uint validBits)
    {
        if (validBits == 64) return unchecked(end - start);
        var mask = (1UL << (int)validBits) - 1;
        return (end - start) & mask;
    }
}

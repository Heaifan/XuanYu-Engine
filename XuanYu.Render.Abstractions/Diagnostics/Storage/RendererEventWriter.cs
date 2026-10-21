using System.Threading.Channels;

namespace XuanYu.Render.Abstractions.Diagnostics;

internal sealed class RendererEventWriter : IAsyncDisposable
{
    readonly Channel<RendererEvent> _queue;
    readonly Func<RendererEvent, ValueTask> _writeAsync;
    readonly Func<long, RendererEvent> _summaryFactory;
    readonly IAsyncDisposable? _sink;
    readonly int _normalCapacity;
    readonly Task _worker;
    long _dropped;
    long _writeFailures;
    long _normalQueued;

    public RendererEventWriter(int capacity, Func<RendererEvent, ValueTask> writeAsync,
        Func<long, RendererEvent> summaryFactory, IAsyncDisposable? sink = null,
        int reservedErrorCapacity = 0)
    {
        if (capacity < 1) throw new ArgumentOutOfRangeException(nameof(capacity));
        _queue = Channel.CreateBounded<RendererEvent>(new BoundedChannelOptions(capacity)
        { FullMode = BoundedChannelFullMode.Wait, SingleReader = true, SingleWriter = false });
        _writeAsync = writeAsync;
        _summaryFactory = summaryFactory;
        _sink = sink;
        _normalCapacity = capacity - Math.Clamp(reservedErrorCapacity, 0, capacity - 1);
        _worker = DrainAsync();
    }

    public long DroppedCount => Interlocked.Read(ref _dropped);
    public long WriteFailureCount => Interlocked.Read(ref _writeFailures);

    public bool TryWrite(RendererEvent item)
    {
        var priority = IsPriority(item);
        if (!priority && Interlocked.Increment(ref _normalQueued) > _normalCapacity)
        { Interlocked.Decrement(ref _normalQueued); return Drop(); }
        if (_queue.Writer.TryWrite(item)) return true;
        if (!priority) Interlocked.Decrement(ref _normalQueued);
        return Drop();
    }

    bool Drop() { Interlocked.Increment(ref _dropped); return false; }
    static bool IsPriority(RendererEvent item) => item.Level >= RendererEventLevel.Error ||
        item.EventCode == RendererEventCodes.SessionClosed;

    public ValueTask DisposeAsync()
    {
        _queue.Writer.TryComplete();
        return new ValueTask(_worker);
    }

    async Task DrainAsync()
    {
        await foreach (var item in _queue.Reader.ReadAllAsync().ConfigureAwait(false))
        {
            if (!IsPriority(item)) Interlocked.Decrement(ref _normalQueued);
            await WriteOneAsync(item).ConfigureAwait(false);
        }
        await WriteOneAsync(_summaryFactory(DroppedCount)).ConfigureAwait(false);
        if (_sink is not null)
        {
            try { await _sink.DisposeAsync().ConfigureAwait(false); }
            catch { Interlocked.Increment(ref _writeFailures); Interlocked.Increment(ref _dropped); }
        }
    }

    async Task WriteOneAsync(RendererEvent item)
    {
        try { await _writeAsync(item).ConfigureAwait(false); }
        catch { Interlocked.Increment(ref _writeFailures); Interlocked.Increment(ref _dropped); }
    }
}

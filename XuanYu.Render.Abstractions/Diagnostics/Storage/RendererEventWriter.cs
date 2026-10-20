using System.Threading.Channels;

namespace XuanYu.Render.Abstractions.Diagnostics;

internal sealed class RendererEventWriter : IAsyncDisposable
{
    readonly Channel<RendererEvent> _queue;
    readonly Func<RendererEvent, ValueTask> _writeAsync;
    readonly Func<long, RendererEvent> _summaryFactory;
    readonly IAsyncDisposable? _sink;
    readonly Task _worker;
    long _dropped;

    public RendererEventWriter(int capacity, Func<RendererEvent, ValueTask> writeAsync,
        Func<long, RendererEvent> summaryFactory, IAsyncDisposable? sink = null)
    {
        if (capacity < 1) throw new ArgumentOutOfRangeException(nameof(capacity));
        _queue = Channel.CreateBounded<RendererEvent>(new BoundedChannelOptions(capacity)
        { FullMode = BoundedChannelFullMode.Wait, SingleReader = true, SingleWriter = false });
        _writeAsync = writeAsync;
        _summaryFactory = summaryFactory;
        _sink = sink;
        _worker = DrainAsync();
    }

    public long DroppedCount => Interlocked.Read(ref _dropped);

    public bool TryWrite(RendererEvent item)
    {
        if (_queue.Writer.TryWrite(item)) return true;
        Interlocked.Increment(ref _dropped);
        return false;
    }

    public ValueTask DisposeAsync()
    {
        _queue.Writer.TryComplete();
        return new ValueTask(_worker);
    }

    async Task DrainAsync()
    {
        await foreach (var item in _queue.Reader.ReadAllAsync().ConfigureAwait(false))
            await WriteOneAsync(item).ConfigureAwait(false);
        await WriteOneAsync(_summaryFactory(DroppedCount)).ConfigureAwait(false);
        if (_sink is not null)
        {
            try { await _sink.DisposeAsync().ConfigureAwait(false); }
            catch { Interlocked.Increment(ref _dropped); }
        }
    }

    async Task WriteOneAsync(RendererEvent item)
    {
        try { await _writeAsync(item).ConfigureAwait(false); }
        catch { Interlocked.Increment(ref _dropped); }
    }
}

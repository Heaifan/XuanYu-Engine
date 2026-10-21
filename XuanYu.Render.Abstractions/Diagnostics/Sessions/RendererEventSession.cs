namespace XuanYu.Render.Abstractions.Diagnostics;

public sealed partial class RendererEventSession : IAsyncDisposable
{
    readonly string _runId;
    readonly string _buildIdentity;
    readonly RendererEventWriter _runtime;
    readonly RendererEventWriter? _probe;
    int _disposed;

    RendererEventSession(RendererEventSessionOptions options, string sessionId)
    {
        _runId = options.RunId;
        _buildIdentity = options.BuildIdentity;
        SessionId = sessionId;
        var root = Path.Combine(options.Directory, sessionId);
        var runtimeSink = new RendererEventFileSink(Path.Combine(root, "runtime"), "renderer-runtime",
            options.SegmentBytes, options.RetainedSegments);
        RuntimeFilePath = runtimeSink.InitialPath;
        _runtime = CreateWriter(runtimeSink, options.RuntimeCapacity, RendererEventChannel.Runtime);
        if (options.ProbeEnabled)
        {
            var probeSink = new RendererEventFileSink(Path.Combine(root, "probe"), "renderer-probe",
                options.SegmentBytes, options.RetainedSegments);
            ProbeFilePath = probeSink.InitialPath;
            _probe = CreateWriter(probeSink, options.ProbeCapacity, RendererEventChannel.Probe);
        }
        PublishBoth(RendererEventCodes.SessionStarted, RendererEventLevel.Info);
    }

    public string SessionId { get; }
    public string RuntimeFilePath { get; }
    public string? ProbeFilePath { get; }
    public long RuntimeDroppedCount => _runtime.DroppedCount;
    public long ProbeDroppedCount => _probe?.DroppedCount ?? 0;
    public long RuntimeWriteFailureCount => _runtime.WriteFailureCount;
    public long ProbeWriteFailureCount => _probe?.WriteFailureCount ?? 0;
    public bool ProbeEnabled => _probe is not null;

    public static RendererEventSession Create(RendererEventSessionOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (string.IsNullOrWhiteSpace(options.Directory) || string.IsNullOrWhiteSpace(options.RunId) ||
            string.IsNullOrWhiteSpace(options.BuildIdentity) || options.RuntimeCapacity < 1 ||
            options.ProbeCapacity < 1 || options.SegmentBytes < 1 || options.RetainedSegments < 1)
            throw new ArgumentException("Renderer event session options are invalid.", nameof(options));
        return new RendererEventSession(options, Guid.NewGuid().ToString("N"));
    }

    public bool Publish(RendererEvent item)
    {
        if (item.RunId != _runId || item.SessionId != SessionId)
            throw new ArgumentException("Renderer event identity does not match this session.", nameof(item));
        return item.Channel == RendererEventChannel.Runtime ? _runtime.TryWrite(item) :
            _probe?.TryWrite(item) ?? false;
    }

    public void PublishFrameEvent(string code, RendererEventLevel level, long frameId,
        uint imageIndex, long cameraRevision)
    {
        var item = NewEvent(code, RendererEventChannel.Runtime, level, frameId, imageIndex, cameraRevision);
        if (code is RendererEventCodes.FrameSubmitted or RendererEventCodes.FramePresented)
        {
            _probe?.TryWrite(item with { Channel = RendererEventChannel.Probe });
            return;
        }
        _runtime.TryWrite(item);
        if (_probe is not null) _probe.TryWrite(item with { Channel = RendererEventChannel.Probe });
    }

    public ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return ValueTask.CompletedTask;
        PublishBoth(RendererEventCodes.SessionClosed, RendererEventLevel.Info);
        var tasks = new List<Task> { _runtime.DisposeAsync().AsTask() };
        if (_probe is not null) tasks.Add(_probe.DisposeAsync().AsTask());
        return new ValueTask(Task.WhenAll(tasks));
    }

    RendererEventWriter CreateWriter(RendererEventFileSink sink, int capacity, RendererEventChannel channel) =>
        new(capacity, sink.WriteAsync, count => NewEvent(RendererEventCodes.DroppedEventsSummary,
            channel, RendererEventLevel.Warning, dropped: count), sink,
            channel == RendererEventChannel.Runtime ? Math.Min(16, capacity / 8) : 0);

    void PublishBoth(string code, RendererEventLevel level)
    {
        var item = NewEvent(code, RendererEventChannel.Runtime, level);
        _runtime.TryWrite(item);
        if (_probe is not null) _probe.TryWrite(item with { Channel = RendererEventChannel.Probe });
    }

    RendererEvent NewEvent(string code, RendererEventChannel channel, RendererEventLevel level,
        long? frame = null, uint? image = null, long? camera = null, long? dropped = null,
        uint? generation = null, int? requestedWidth = null, int? requestedHeight = null,
        uint? actualWidth = null, uint? actualHeight = null, int? imageViewCount = null,
        string? vulkanResult = null, string? reason = null) =>
        new(code, channel, level, _runId, SessionId, DateTimeOffset.UtcNow, _buildIdentity,
            frame, image, camera, dropped, generation, requestedWidth, requestedHeight,
            actualWidth, actualHeight, imageViewCount, vulkanResult, reason);
}

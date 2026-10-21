using System.Collections.Concurrent;
using XuanYu.Render.Abstractions.Diagnostics;

namespace XuanYu.World.Tests.Diagnostics.Storage;

public sealed class RendererEventReliabilityTests
{
    [Fact]
    public async Task Runtime_errors_use_reserved_capacity_and_keep_normal_queue_bounded()
    {
        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var rows = new ConcurrentQueue<RendererEvent>();
        async ValueTask Write(RendererEvent item)
        {
            if (item.EventCode == "Blocked") { started.SetResult(); await gate.Task; }
            rows.Enqueue(item);
        }
        var writer = new RendererEventWriter(8, Write, _ => New("DroppedEventsSummary"),
            reservedErrorCapacity: 2);
        Assert.True(writer.TryWrite(New("Blocked")));
        await started.Task.WaitAsync(TimeSpan.FromSeconds(5));
        for (var index = 0; index < 6; index++) Assert.True(writer.TryWrite(New($"Info{index}")));
        Assert.False(writer.TryWrite(New("ProbeInfo")));
        Assert.True(writer.TryWrite(New("Fatal", RendererEventLevel.Critical)));
        gate.SetResult();
        await writer.DisposeAsync();
        Assert.Contains(rows, row => row.EventCode == "Fatal");
    }

    [Fact]
    public async Task Writer_exposes_sink_failure_count()
    {
        var writer = new RendererEventWriter(2, item => item.EventCode == "DroppedEventsSummary"
                ? ValueTask.CompletedTask : ValueTask.FromException(new IOException()),
            _ => New("DroppedEventsSummary"));
        Assert.True(writer.TryWrite(New("RuntimeError", RendererEventLevel.Error)));
        await writer.DisposeAsync();
        var count = typeof(RendererEventWriter).GetProperty("WriteFailureCount");
        Assert.NotNull(count);
        Assert.Equal(1L, count!.GetValue(writer));
    }

    [Fact]
    public async Task File_sink_buffers_rows_until_close_and_flushes_them()
    {
        using var directory = new EventTestDirectory();
        var sink = new RendererEventFileSink(directory.Path, "runtime", 4096, 2);
        try
        {
            await sink.WriteAsync(New("Buffered"));
            Assert.Equal(0, new FileInfo(sink.InitialPath).Length);
        }
        finally { await sink.DisposeAsync(); }
        Assert.Contains("Buffered", File.ReadAllText(sink.InitialPath));
    }

    static RendererEvent New(string code, RendererEventLevel level = RendererEventLevel.Info) =>
        new(code, RendererEventChannel.Runtime, level, "run", "session", DateTimeOffset.UtcNow, "HEAD=UNKNOWN");

    sealed class EventTestDirectory : IDisposable
    {
        public EventTestDirectory()
        {
            var root = Environment.GetEnvironmentVariable("XYE_WORKSPACE_ROOT")
                ?? throw new InvalidOperationException("受控测试必须设置 XYE_WORKSPACE_ROOT。");
            Path = System.IO.Path.Combine(root, "Temp", "renderer-writer-tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path);
        }
        public string Path { get; }
        public void Dispose() => Directory.Delete(Path, true);
    }
}

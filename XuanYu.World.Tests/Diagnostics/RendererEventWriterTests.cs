using System.Collections.Concurrent;
using XuanYu.Render.Abstractions.Diagnostics;

namespace XuanYu.World.Tests.Diagnostics;

public sealed class RendererEventWriterTests
{
    [Fact]
    public async Task Runtime_and_probe_write_distinct_files_and_flush_on_close()
    {
        using var directory = new EventTestDirectory();
        await using var session = RendererEventSession.Create(Options(directory.Path));
        var probePath = session.ProbeFilePath ?? throw new InvalidOperationException("Probe 文件路径未创建。");
        Assert.NotEqual(session.RuntimeFilePath, probePath);

        Assert.True(session.Publish(New("RuntimeReady", RendererEventChannel.Runtime, "run-a", session.SessionId)));
        Assert.True(session.Publish(New("ProbeReady", RendererEventChannel.Probe, "run-a", session.SessionId)));
        await session.DisposeAsync();

        Assert.Contains(RendererEventReader.Read(session.RuntimeFilePath), item => item.EventCode == "RuntimeReady");
        Assert.Contains(RendererEventReader.Read(probePath), item => item.EventCode == "ProbeReady");
    }

    [Fact]
    public async Task Full_queue_drops_without_blocking_and_reports_count_on_close()
    {
        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var rows = new ConcurrentQueue<RendererEvent>();
        async ValueTask Write(RendererEvent item)
        {
            if (item.EventCode == "Blocked") { started.SetResult(); await gate.Task; }
            rows.Enqueue(item);
        }
        var writer = new RendererEventWriter(1, Write,
            count => New("DroppedEventsSummary", RendererEventChannel.Runtime, "run-a", "session-a") with { DroppedCount = count });
        Assert.True(writer.TryWrite(New("Blocked", RendererEventChannel.Runtime, "run-a", "session-a")));
        await started.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.True(writer.TryWrite(New("Queued", RendererEventChannel.Runtime, "run-a", "session-a")));
        Assert.False(writer.TryWrite(New("Dropped", RendererEventChannel.Runtime, "run-a", "session-a")));
        Assert.Equal(1, writer.DroppedCount);

        gate.SetResult();
        await writer.DisposeAsync();

        Assert.Contains(rows, item => item.EventCode == "DroppedEventsSummary" && item.DroppedCount == 1);
    }

    [Fact]
    public async Task Rotation_retains_only_configured_number_of_segments()
    {
        using var directory = new EventTestDirectory();
        await using var session = RendererEventSession.Create(Options(directory.Path, segmentBytes: 320));
        for (var index = 0; index < 40; index++)
            session.Publish(New($"Event{index:D2}", RendererEventChannel.Runtime, "run-a", session.SessionId));
        await session.DisposeAsync();

        var files = Directory.GetFiles(Path.Combine(directory.Path, session.SessionId, "runtime"), "*.jsonl");
        Assert.InRange(files.Length, 2, 3);
    }

    [Fact]
    public async Task Disabled_probe_does_not_accept_or_create_probe_output()
    {
        using var directory = new EventTestDirectory();
        var options = Options(directory.Path) with { ProbeEnabled = false };
        await using var session = RendererEventSession.Create(options);

        Assert.False(session.Publish(New("ProbeHidden", RendererEventChannel.Probe, "run-a", session.SessionId)));
        Assert.Null(session.ProbeFilePath);
        session.PublishFrameEvent(RendererEventCodes.FrameSubmitted, RendererEventLevel.Info, 4, 1, 2);
        session.PublishFrameEvent(RendererEventCodes.QueuePresentFailed, RendererEventLevel.Error, 4, 1, 2);
        await session.DisposeAsync();

        var runtime = RendererEventReader.Read(session.RuntimeFilePath);
        Assert.DoesNotContain(runtime, item => item.EventCode == RendererEventCodes.FrameSubmitted);
        Assert.Contains(runtime, item => item.EventCode == RendererEventCodes.QueuePresentFailed);
    }

    static RendererEventSessionOptions Options(string path, long segmentBytes = 8 * 1024 * 1024) =>
        new(path, "run-a", "HEAD=469b8347", true, 512, 128, segmentBytes, 3);

    static RendererEvent New(string code, RendererEventChannel channel, string run, string session) =>
        new(code, channel, RendererEventLevel.Info, run, session, DateTimeOffset.UtcNow, "HEAD=469b8347");

    sealed class EventTestDirectory : IDisposable
    {
        public EventTestDirectory()
        {
            Path = System.IO.Path.Combine(TestSourceRoot.Root, "obj", "renderer-event-tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }
        public void Dispose() => Directory.Delete(Path, recursive: true);
    }
}

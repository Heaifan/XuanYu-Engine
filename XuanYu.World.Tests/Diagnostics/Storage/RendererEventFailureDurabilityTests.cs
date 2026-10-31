using System.Collections.Concurrent;
using XuanYu.Render.Abstractions.Diagnostics;
using XuanYu.Render.Vulkan.Swapchain.Diagnostics;

namespace XuanYu.World.Tests.Diagnostics.Storage;

public sealed class RendererEventFailureDurabilityTests
{
    [Fact]
    public async Task File_sink_flushes_error_rows_before_close()
    {
        using var directory = new EventTestDirectory();
        var sink = new RendererEventFileSink(directory.Path, "runtime", 4096, 2);
        try
        {
            await sink.WriteAsync(New("Buffered"));
            Assert.Empty(RendererEventReader.Read(sink.InitialPath));
            await sink.WriteAsync(New("Fatal", RendererEventLevel.Critical));
            Assert.Contains(RendererEventReader.Read(sink.InitialPath), row => row.EventCode == "Fatal");
        }
        finally { await sink.DisposeAsync(); }
    }

    [Fact]
    public async Task Priority_overflow_retains_one_critical_event_without_blocking()
    {
        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var rows = new ConcurrentQueue<RendererEvent>();
        async ValueTask Write(RendererEvent item)
        {
            if (item.EventCode == "Blocked") { started.SetResult(); await gate.Task; }
            rows.Enqueue(item);
        }
        var writer = new RendererEventWriter(2, Write, _ => New("DroppedEventsSummary"), reservedErrorCapacity: 1);
        Assert.True(writer.TryWrite(New("Blocked")));
        await started.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.True(writer.TryWrite(New("FatalA", RendererEventLevel.Error)));
        Assert.True(writer.TryWrite(New("FatalB", RendererEventLevel.Critical)));
        Assert.True(writer.TryWrite(New("FatalOverflow", RendererEventLevel.Critical)));
        gate.SetResult();
        await writer.DisposeAsync();
        Assert.Contains(rows, row => row.EventCode == "FatalOverflow");
    }

    [Fact]
    public async Task Initial_swapchain_failure_helper_keeps_request_and_result()
    {
        using var directory = new EventTestDirectory();
        var session = RendererEventSession.Create(new RendererEventSessionOptions(
            directory.Path, "run", "HEAD=UNKNOWN", false));
        VulkanSwapchainEvents.PublishFailure(session, 800, 600, "ErrorDeviceLost", "CreateSwapchain");
        await session.DisposeAsync();
        var item = RendererEventReader.Read(session.RuntimeFilePath, RendererEventCodes.SwapchainFailed).Single();
        Assert.Equal(800, item.RequestedWidth);
        Assert.Equal(600, item.RequestedHeight);
        Assert.Equal("ErrorDeviceLost", item.VulkanResult);
        Assert.Equal("CreateSwapchain", item.Reason);
    }

    static RendererEvent New(string code, RendererEventLevel level = RendererEventLevel.Info) =>
        new(code, RendererEventChannel.Runtime, level, "run", "session", DateTimeOffset.UtcNow, "HEAD=UNKNOWN");

    sealed class EventTestDirectory : IDisposable
    {
        public EventTestDirectory()
        {
            Path = System.IO.Path.Combine(TestSourceRoot.Root, "obj", "renderer-durability", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path);
        }
        public string Path { get; }
        public void Dispose() => Directory.Delete(Path, true);
    }
}

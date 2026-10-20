using System.Collections.Concurrent;
using XuanYu.Render.Abstractions.Diagnostics;

namespace XuanYu.World.Tests.Diagnostics;

public sealed class RendererEventWriterContextTests
{
    [Fact]
    public async Task Lifecycle_event_round_trips_structured_resize_facts()
    {
        var root = Environment.GetEnvironmentVariable("XYE_WORKSPACE_ROOT")
            ?? throw new InvalidOperationException("受控测试必须设置 XYE_WORKSPACE_ROOT。");
        var directory = Path.Combine(root, "Temp", "renderer-event-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            await using var session = RendererEventSession.Create(new RendererEventSessionOptions(
                directory, "run-resize", "HEAD=test", false, 32, 32, 4096, 2));
            session.PublishLifecycleEvent(RendererEventCodes.ResizeCompleted,
                RendererEventLevel.Info, 7, 800, 600, 1200, 900);
            await session.DisposeAsync();
            var item = RendererEventReader.Read(session.RuntimeFilePath,
                RendererEventCodes.ResizeCompleted).Single();
            Assert.Equal((uint)7, item.Generation);
            Assert.Equal((800, 600), (item.RequestedWidth, item.RequestedHeight));
            Assert.Equal(((uint)1200, (uint)900), (item.ActualWidth, item.ActualHeight));
        }
        finally { if (Directory.Exists(directory)) Directory.Delete(directory, recursive: true); }
    }

    [Fact]
    public void Synchronous_close_does_not_require_dispatcher_continuations()
    {
        var previous = SynchronizationContext.Current;
        var dispatcher = new QueuedSynchronizationContext();
        var root = Environment.GetEnvironmentVariable("XYE_WORKSPACE_ROOT")
            ?? throw new InvalidOperationException("受控测试必须设置 XYE_WORKSPACE_ROOT。");
        var directory = Path.Combine(root, "Temp", "renderer-event-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        Task? close = null;
        using var completed = new ManualResetEventSlim();
        var completedWithoutPump = false;
        try
        {
            SynchronizationContext.SetSynchronizationContext(dispatcher);
            var options = new RendererEventSessionOptions(directory, "run-a", "HEAD=test",
                false, 32, 32, 4096, 2);
            var session = RendererEventSession.Create(options);
            close = session.DisposeAsync().AsTask();
            close.ContinueWith(_ => completed.Set(), TaskScheduler.Default);
            completedWithoutPump = completed.Wait(TimeSpan.FromMilliseconds(250));
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(previous);
            dispatcher.RunQueued();
            if (close?.Wait(TimeSpan.FromSeconds(5)) == true)
                Directory.Delete(directory, recursive: true);
        }
        Assert.True(completedWithoutPump, "Writer flush waited for the blocked UI dispatcher.");
    }

    sealed class QueuedSynchronizationContext : SynchronizationContext
    {
        readonly ConcurrentQueue<(SendOrPostCallback Callback, object? State)> _callbacks = new();
        public override void Post(SendOrPostCallback callback, object? state) =>
            _callbacks.Enqueue((callback, state));

        public void RunQueued()
        {
            while (_callbacks.TryDequeue(out var item)) item.Callback(item.State);
        }
    }
}

using XuanYu.Render.Abstractions.Diagnostics;

namespace XuanYu.World.Tests.Diagnostics;

public sealed class RendererEventSessionIdentityTests
{
    [Fact]
    public async Task Successful_frame_events_are_probe_only_and_keep_their_identity()
    {
        using var directory = new EventTestDirectory();
        var session = RendererEventSession.Create(new RendererEventSessionOptions(
            directory.Path, "run-frame", "HEAD=469b8347", true));
        session.PublishFrameEvent(RendererEventCodes.FrameSubmitted, RendererEventLevel.Info, 44, 7, 21);
        session.PublishFrameEvent(RendererEventCodes.FramePresented, RendererEventLevel.Info, 44, 7, 21);
        await session.DisposeAsync();

        var runtime = RendererEventReader.Read(session.RuntimeFilePath, runId: "run-frame", sessionId: session.SessionId)
            .Where(item => item.FrameId is not null).ToArray();
        var probe = RendererEventReader.Read(session.ProbeFilePath!, runId: "run-frame", sessionId: session.SessionId)
            .Where(item => item.FrameId is not null).ToArray();
        Assert.Empty(runtime);
        Assert.Equal(2, probe.Length);
        Assert.All(runtime.Concat(probe), item =>
        {
            Assert.Equal(44, item.FrameId);
            Assert.Equal((uint)7, item.ImageIndex);
            Assert.Equal(21, item.CameraRevision);
            Assert.Equal("HEAD=469b8347", item.BuildIdentity);
            Assert.Equal(session.SessionId, item.SessionId);
        });
        Assert.All(runtime, item => Assert.Equal(RendererEventChannel.Runtime, item.Channel));
        Assert.All(probe, item => Assert.Equal(RendererEventChannel.Probe, item.Channel));
    }

    sealed class EventTestDirectory : IDisposable
    {
        public EventTestDirectory()
        {
            var root = Environment.GetEnvironmentVariable("XYE_WORKSPACE_ROOT")
                ?? throw new InvalidOperationException("受控测试必须设置 XYE_WORKSPACE_ROOT。");
            Path = System.IO.Path.Combine(root, "Temp", "renderer-event-tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }
        public void Dispose() => Directory.Delete(Path, recursive: true);
    }
}

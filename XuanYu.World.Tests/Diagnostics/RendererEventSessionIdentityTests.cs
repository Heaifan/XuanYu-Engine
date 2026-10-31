using XuanYu.Render.Abstractions.Diagnostics;

namespace XuanYu.World.Tests.Diagnostics;

public sealed class RendererEventSessionIdentityTests
{
    [Fact]
    public void Build_identity_marks_blank_source_values_unknown()
    {
        var names = new[] { "XYE_SOURCE_HEAD", "XYE_SOURCE_DIRTY", "XYE_SOURCE_FINGERPRINT" };
        var old = names.Select(Environment.GetEnvironmentVariable).ToArray();
        try
        {
            foreach (var name in names) Environment.SetEnvironmentVariable(name, " ");
            var identity = RendererBuildIdentity.Current();
            Assert.Contains("HEAD=UNKNOWN;DIRTY=UNKNOWN;TREE=UNKNOWN;", identity);
        }
        finally
        {
            for (var i = 0; i < names.Length; i++) Environment.SetEnvironmentVariable(names[i], old[i]);
        }
    }

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
            Path = System.IO.Path.Combine(TestSourceRoot.Root, "obj", "renderer-event-tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }
        public void Dispose() => Directory.Delete(Path, recursive: true);
    }
}

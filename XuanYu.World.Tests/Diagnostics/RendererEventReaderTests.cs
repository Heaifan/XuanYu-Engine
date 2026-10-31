using XuanYu.Render.Abstractions.Diagnostics;

namespace XuanYu.World.Tests.Diagnostics;

public sealed class RendererEventReaderTests
{
    [Fact]
    public void Event_json_contains_typed_identity_and_optional_frame_fields()
    {
        var item = New("FramePresented", "run-a", "session-a", 17);
        var json = System.Text.Json.JsonSerializer.Serialize(item, RendererEventJson.Options);

        Assert.Contains("\"eventCode\":\"FramePresented\"", json);
        Assert.Contains("\"channel\":\"Runtime\"", json);
        Assert.Contains("\"runId\":\"run-a\"", json);
        Assert.Contains("\"sessionId\":\"session-a\"", json);
        Assert.Contains("\"frameId\":17", json);
        Assert.Contains("\"buildIdentity\":\"HEAD=469b8347\"", json);
    }

    [Fact]
    public void Read_filters_by_event_code_run_and_session_and_skips_bad_rows()
    {
        using var directory = new EventTestDirectory();
        var path = Path.Combine(directory.Path, "events.jsonl");
        File.WriteAllLines(path,
        [
            "{bad-json",
            System.Text.Json.JsonSerializer.Serialize(New("FrameSubmitted", "run-a", "session-a", 17), RendererEventJson.Options),
            System.Text.Json.JsonSerializer.Serialize(New("FramePresented", "run-a", "session-a", 17), RendererEventJson.Options),
            System.Text.Json.JsonSerializer.Serialize(New("FramePresented", "run-b", "session-b", 18), RendererEventJson.Options)
        ]);

        Assert.Equal(2, RendererEventReader.Read(path, "FramePresented").Count);
        Assert.Equal(2, RendererEventReader.Read(path, runId: "run-a").Count);
        Assert.Equal(2, RendererEventReader.Read(path, sessionId: "session-a").Count);
        var result = RendererEventReader.Read(path, "FramePresented", "run-a", "session-a");

        var item = Assert.Single(result);
        Assert.Equal("FramePresented", item.EventCode);
        Assert.Equal("run-a", item.RunId);
        Assert.Equal("session-a", item.SessionId);
    }

    static RendererEvent New(string code, string run, string session, long frame) => new(
        code, RendererEventChannel.Runtime, RendererEventLevel.Info, run, session,
        DateTimeOffset.UtcNow, "HEAD=469b8347", frame, 2, 9);

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

using System.Text.Json;
using System.Text.Json.Serialization;
using XuanYu.Editor.UI.XycControl;

namespace XuanYu.World.Tests.Xyc;

public sealed class XycCommandProcessorTests
{
    [Fact]
    public void Camera_pose_requires_diagnostic_write_permission()
    {
        var processor = new XycCommandProcessor(new FakeRuntime(), false);
        var result = processor.Execute(Request("camera.set-test-pose", Pose()));
        Assert.False(result.Success);
        Assert.Equal("UNAUTHORIZED", result.ErrorCode);
    }

    [Fact]
    public void Camera_pose_returns_before_after_and_revision()
    {
        var processor = new XycCommandProcessor(new FakeRuntime(), true);
        var result = processor.Execute(Request("camera.set-test-pose", Pose(8)));
        Assert.True(result.Success, result.ErrorCode);
        Assert.Equal(2, result.CameraRevision);
        Assert.Contains("before", JsonSerializer.Serialize(result.ObservedState));
        Assert.Contains("after", JsonSerializer.Serialize(result.ObservedState));
    }

    [Fact]
    public void Invalid_pose_and_unknown_command_fail_closed()
    {
        var processor = new XycCommandProcessor(new FakeRuntime(), true);
        Assert.Equal("INVALID_INPUT", processor.Execute(Request("camera.set-test-pose",
            Pose(0, zeroForward: true))).ErrorCode);
        Assert.Equal("INVALID_INPUT", processor.Execute(Request("camera.set-test-pose",
            Pose() with { Up = new(0, 0, -1) })).ErrorCode);
        foreach (var value in new[] { double.NaN, double.PositiveInfinity })
        {
            var options = new JsonSerializerOptions
                { NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals };
            var args = JsonSerializer.SerializeToElement(Pose() with
                { Position = new(value, 0, 0) }, options);
            Assert.Equal("INVALID_INPUT", processor.Execute(Request(
                "camera.set-test-pose", args)).ErrorCode);
        }
        Assert.Equal("UNKNOWN_COMMAND", processor.Execute(Request("not-real")).ErrorCode);
    }

    [Fact]
    public void Request_id_cannot_be_replayed()
    {
        var processor = new XycCommandProcessor(new FakeRuntime(), true);
        var request = Request("runtime.info");
        Assert.True(processor.Execute(request).Success);
        Assert.Equal("DUPLICATE_REQUEST", processor.Execute(request).ErrorCode);
    }

    [Fact]
    public async Task Timed_out_host_read_releases_session()
    {
        var closed = false;
        using var timeout = new CancellationTokenSource(TimeSpan.FromMilliseconds(20));
        var host = new XycStdioHost(new XycCommandProcessor(new FakeRuntime(), false));
        await host.RunAsync(new BlockingReader(), new StringWriter(),
            () => closed = true, timeout.Token);
        Assert.True(closed);
    }

    static XycRequest Request(string command, XycCamera? pose = null) => new(
        "XYC-API/1", Guid.NewGuid().ToString(), "test-instance", command,
        pose is null ? JsonSerializer.SerializeToElement(new { }) :
            JsonSerializer.SerializeToElement(pose));

    static XycRequest Request(string command, JsonElement args) => new(
        "XYC-API/1", Guid.NewGuid().ToString(), "test-instance", command, args);

    static XycCamera Pose(double x = 4, bool zeroForward = false) => new(
        new(x, 2, 3), zeroForward ? new(0, 0, 0) : new(0, 0, -1), new(0, 1, 0),
        75, 0.05, 1000, "Perspective", 0, new(0, 0, 0), 1);

    sealed class BlockingReader : TextReader
    {
        public override async ValueTask<string?> ReadLineAsync(CancellationToken token)
        {
            await Task.Delay(Timeout.Infinite, token);
            return null;
        }
    }
}

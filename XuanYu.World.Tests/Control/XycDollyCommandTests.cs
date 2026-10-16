using System.Text.Json;
using XuanYu.Editor.UI.XycControl;

namespace XuanYu.World.Tests.Xyc;

public sealed class XycDollyCommandTests
{
    [Fact]
    public void Dolly_uses_signed_production_delta_and_returns_constraint_state()
    {
        var runtime = new FakeRuntime();
        var before = runtime.InspectCamera();
        var args = JsonSerializer.SerializeToElement(new { wheelDelta = -2.5 });
        var result = new XycCommandProcessor(runtime, true).Execute(Request(args));
        Assert.True(result.Success, result.ErrorCode);
        Assert.Equal(-2.5, runtime.LastDollyDelta);
        Assert.Equal(2, result.CameraRevision);
        var state = JsonSerializer.Serialize(result.ObservedState);
        Assert.Contains("before", state);
        Assert.Contains("after", state);
        Assert.Contains("constrained", state);
        Assert.Contains("\"constrained\":true", state);
        Assert.Equal(before.Position.Z - 2.5, runtime.InspectCamera().Position.Z);
    }

    [Fact]
    public void Dolly_requires_diagnostic_write_permission()
    {
        var args = JsonSerializer.SerializeToElement(new { wheelDelta = 1d });
        var result = new XycCommandProcessor(new FakeRuntime(), false).Execute(Request(args));
        Assert.Equal("UNAUTHORIZED", result.ErrorCode);
    }

    static XycRequest Request(JsonElement args) => new("XYC-API/1",
        Guid.NewGuid().ToString(), "test-instance", "camera.dolly", args);
}

using System.Text.Json;
using XuanYu.Editor.UI.XycControl;

namespace XuanYu.World.Tests.Xyc;

public sealed class XycOrbitCommandTests
{
    [Fact]
    public void Orbit_requires_write_permission_and_returns_updated_revision()
    {
        var args = JsonSerializer.SerializeToElement(new { deltaX = 40d, deltaY = -20d });
        var readOnly = new XycCommandProcessor(new FakeRuntime(), false);
        Assert.Equal("UNAUTHORIZED", readOnly.Execute(Request(args)).ErrorCode);
        var runtime = new FakeRuntime();
        var result = new XycCommandProcessor(runtime, true).Execute(Request(args));
        Assert.True(result.Success, result.ErrorCode);
        Assert.Equal(2, result.CameraRevision);
        Assert.Equal(40, runtime.InspectCamera().Position.X);
    }

    static XycRequest Request(JsonElement args) => new("XYC-API/1",
        Guid.NewGuid().ToString(), "test-instance", "camera.orbit", args);
}

using System.Text.Json;
using XuanYu.Editor.UI.XycControl;

namespace XuanYu.World.Tests.Xyc;

public sealed class XycCaptureCommandTests
{
    [Fact]
    public void Presented_capture_requires_diagnostic_write_permission()
    {
        var readOnly = new XycCommandProcessor(new FakeRuntime(), false);
        Assert.Equal("UNAUTHORIZED", readOnly.Execute(Request("capture.arm")).ErrorCode);
        Assert.Equal("UNAUTHORIZED", readOnly.Execute(Request("capture.release")).ErrorCode);
        var writable = new XycCommandProcessor(new FakeRuntime(), true);
        Assert.True(writable.Execute(Request("capture.arm",
            JsonSerializer.SerializeToElement(new XycCaptureRequest(7)))).Success);
        Assert.True(writable.Execute(Request("capture.release")).Success);
    }

    static XycRequest Request(string command, JsonElement? args = null) => new(
        "XYC-API/1", Guid.NewGuid().ToString(), "test-instance", command,
        args ?? JsonSerializer.SerializeToElement(new { }));
}

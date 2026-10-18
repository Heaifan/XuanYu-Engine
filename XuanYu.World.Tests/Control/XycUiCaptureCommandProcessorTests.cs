using System.Text.Json;
using XuanYu.Editor.UI.XycControl;

namespace XuanYu.World.Tests.Xyc;

public sealed class XycUiCaptureCommandProcessorTests
{
    [Fact]
    public void Capture_backend_must_be_supported_and_GDI_topmost_must_be_explicit()
    {
        var ui = new XycUiTestRuntime();
        var processor = new XycCommandProcessor(ui, false, ui);
        Assert.Equal("INVALID_INPUT", processor.Execute(Request(new
            { fileName = "Light-1360.png", backend = "unknown" })).ErrorCode);
        Assert.Equal("INVALID_INPUT", processor.Execute(Request(new
            { fileName = "Light-1360.png", temporaryTopmost = true })).ErrorCode);
        Assert.True(processor.Execute(Request(new
        {
            fileName = "Light-1360.png", backend = "gdiCompatibility", temporaryTopmost = true
        })).Success);
        Assert.Equal("gdiCompatibility", ui.LastCaptureBackend);
    }

    [Fact]
    public async Task Cancelled_capture_does_not_start_rendering()
    {
        var ui = new XycUiTestRuntime();
        var processor = new XycCommandProcessor(ui, false, ui);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var response = await processor.ExecuteAsync(Request(new { fileName = "Light-1360.png" }),
            cancellation.Token);
        Assert.Equal("CANCELLED", response.ErrorCode);
        Assert.Equal(0, ui.Captures);
    }

    [Fact]
    public async Task Capture_cancellation_during_backend_work_returns_cancelled()
    {
        var ui = new XycUiTestRuntime();
        var processor = new XycCommandProcessor(ui, false, ui);
        using var cancellation = new CancellationTokenSource();
        ui.OnCaptureStarting = cancellation.Cancel;
        var response = await processor.ExecuteAsync(Request(new { fileName = "Light-1360.png" }),
            cancellation.Token);
        Assert.Equal("CANCELLED", response.ErrorCode);
        Assert.Equal(1, ui.Captures);
    }

    [Fact]
    public async Task Capture_backend_timeout_returns_time_limit_error()
    {
        var ui = new XycUiTestRuntime { WaitForCaptureCancellation = true };
        var processor = new XycCommandProcessor(ui, false, ui);
        var response = await processor.ExecuteAsync(Request(new
            { fileName = "Light-1360.png", timeoutMs = 20 }), CancellationToken.None);
        Assert.Equal("TIMEOUT", response.ErrorCode);
        Assert.Equal(1, ui.Captures);
    }

    [Fact]
    public void Unsafe_capture_names_and_timeouts_are_rejected()
    {
        var ui = new XycUiTestRuntime();
        var processor = new XycCommandProcessor(ui, false, ui);
        Assert.Equal("INVALID_INPUT", processor.Execute(Request(new
            { fileName = "../../capture.png" })).ErrorCode);
        Assert.Equal("INVALID_INPUT", processor.Execute(Request(new
            { fileName = "capture.png" })).ErrorCode);
        Assert.Equal("INVALID_INPUT", processor.Execute(Request(new
            { fileName = "Light-1360.png", timeoutMs = 30001 })).ErrorCode);
    }

    static XycRequest Request(object args) => new("XYC-API/1", Guid.NewGuid().ToString("N"),
        "test-instance", "ui.capture", JsonSerializer.SerializeToElement(args));
}

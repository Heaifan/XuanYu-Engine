using System.Text.Json;
using Avalonia.Controls;
using XuanYu.Editor.UI;
using XuanYu.Editor.UI.XycControl;

namespace XuanYu.World.Tests.Xyc;

public sealed class XycUiControlCommandTests
{
    [Fact]
    public void Production_control_lookup_rejects_duplicate_and_missing_stable_ids()
    {
        var first = new Button(); var second = new Button();
        XYDiagnostic.SetDebugId(first, "XYE.TEST.DUPLICATE");
        XYDiagnostic.SetDebugId(second, "XYE.TEST.DUPLICATE");
        var duplicate = Assert.Throws<XycUiCommandException>(() =>
            XycUiControlLookup.ResolveUnique([first, second], "XYE.TEST.DUPLICATE"));
        Assert.Equal("DUPLICATE_CONTROL_ID", duplicate.Code);
        var missing = Assert.Throws<XycUiCommandException>(() =>
            XycUiControlLookup.ResolveUnique([first], "XYE.TEST.MISSING"));
        Assert.Equal("CONTROL_NOT_FOUND", missing.Code);
    }

    [Fact]
    public async Task Safe_control_invoke_returns_state_and_two_capture_records()
    {
        var runtime = new XycUiTestRuntime();
        var processor = new XycCommandProcessor(runtime, false, runtime);
        Assert.True(processor.Execute(Request("ui.control.list")).Success);
        Assert.True(processor.Execute(Request("ui.control.inspect",
            new { controlId = "XYE.TOP.VIEW.ALL" })).Success);
        var invoked = processor.Execute(Request("ui.control.invoke",
            new { controlId = "XYE.TOP.VIEW.ALL", operation = "invoke" }));
        Assert.True(invoked.Success);
        var state = Assert.IsType<XycUiControlInvokeState>(invoked.ObservedState);
        Assert.Equal(1, state.Before.CameraRevision);
        Assert.Equal(2, state.After.CameraRevision);
        Assert.NotNull(state.BeforeCapture);
        Assert.NotNull(state.AfterCapture);
        Assert.True((await processor.ExecuteAsync(Request("ui.waitUntil",
            new { state = "cameraRevision", expected = 2, timeoutMs = 1000 }),
            CancellationToken.None)).Success);
    }

    [Fact]
    public async Task Session_timeout_and_disappearance_failures_are_structured()
    {
        var runtime = new XycUiTestRuntime();
        var processor = new XycCommandProcessor(runtime, false, runtime);
        Assert.Equal("UNAUTHORIZED", processor.Execute(Request("ui.control.list", sessionId: "other")).ErrorCode);
        var timeout = await processor.ExecuteAsync(Request("ui.waitUntil",
            new { state = "cameraRevision", expected = 99, timeoutMs = 25 }), CancellationToken.None);
        Assert.Equal("TIMEOUT", timeout.ErrorCode);
        runtime.DisappearsBeforeInvoke = true;
        var missing = await processor.ExecuteAsync(Request("ui.control.invoke",
            new { controlId = "XYE.TOP.VIEW.ALL", operation = "invoke" }), CancellationToken.None);
        Assert.Equal("CONTROL_NOT_FOUND", missing.ErrorCode);
    }

    [Theory]
    [InlineData("MISSING", "CONTROL_NOT_FOUND")]
    [InlineData("DUPLICATE", "DUPLICATE_CONTROL_ID")]
    [InlineData("XYE.TOP.EDIT.BOX_SELECT", "CONTROL_DISABLED")]
    [InlineData("XYE.TOP.FILE.SAVE", "OPERATION_NOT_ALLOWED")]
    public void Unsafe_or_unresolvable_control_is_rejected(string id, string error)
    {
        var runtime = new XycUiTestRuntime();
        var response = new XycCommandProcessor(runtime, false, runtime).Execute(
            Request("ui.control.invoke", new { controlId = id, operation = "invoke" }));
        Assert.False(response.Success);
        Assert.Equal(error, response.ErrorCode);
    }

    static XycRequest Request(string command, object? args = null,
        string sessionId = "test-instance") => new("XYC-API/1",
        Guid.NewGuid().ToString("N"), sessionId, command,
        JsonSerializer.SerializeToElement(args ?? new { }));
}

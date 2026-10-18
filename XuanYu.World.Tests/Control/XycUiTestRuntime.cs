using System.Text.Json;
using XuanYu.Editor.UI.XycControl;

namespace XuanYu.World.Tests.Xyc;

internal sealed class XycUiTestRuntime : IXycRuntime, IXycUiRuntime
{
    public int Captures { get; private set; }
    public int Invocations { get; private set; }
    public bool DisappearsBeforeInvoke { get; set; }
    public string InstanceId => "test-instance";
    public XycRuntimeInfo InspectRuntime() => throw new NotImplementedException();
    public XycScene InspectScene() => throw new NotImplementedException();
    public XycCamera InspectCamera() => throw new NotImplementedException();
    public XycCamera SetTestPose(XycCamera pose) => throw new NotImplementedException();
    public XycCamera OrbitCamera(double x, double y) => throw new NotImplementedException();
    public XycDollyResult DollyCamera(double delta) => throw new NotImplementedException();
    public bool ArmPresentedCapture(long revision) => false;
    public bool ReleasePresentedCapture() => false;
    public XycUiWindowState InspectWindow() => new(1, "0x1", true, false,
        "Normal", 1360, 820, 1360, 820, 96, 1);
    public XycUiWindowState ResizeWindow(double w, double h) => InspectWindow();
    public XycUiThemeState InspectTheme() => new("Dark", "Dark");
    public XycUiThemeState SetTheme(string theme) => new(theme, theme);
    public XycUiRegionState InspectTop() => new("TOP", 0, 0, 1360, 69, 0, 0, 1360, 69);
    public XycUiCaptureState CaptureTop(string name, bool temporaryTopmost = false)
    {
        Captures++;
        return new(name, Path.ChangeExtension(name, ".json"), "sha256", 1360, 69,
            InspectTop(), InspectWindow(), InspectTheme(), "build", "test", DateTimeOffset.UtcNow);
    }
    public IReadOnlyList<XycUiControlState> ListControls() => [];
    public XycUiControlState InspectControl(string id) => id switch
    {
        "MISSING" => throw new XycUiCommandException("CONTROL_NOT_FOUND"),
        "DUPLICATE" => throw new XycUiCommandException("DUPLICATE_CONTROL_ID"),
        "XYE.TOP.EDIT.BOX_SELECT" => new(id, "XYToggleButton", true, false, false, "DebugId", "TOP", false),
        _ => new(id, "XYButton", true, true, false, "DebugId", "TOP", null)
    };
    public XycUiState InspectState() => new("选择", true, true, true, "", "就绪", Invocations + 1, 0, 0, 0);
    public Task<XycUiControlInvokeState> InvokeControlAsync(string id, string operation,
        CancellationToken cancellationToken)
    {
        var control = InspectControl(id);
        if (!control.Enabled) throw new XycUiCommandException("CONTROL_DISABLED");
        if (id != "XYE.TOP.VIEW.ALL" || operation != "invoke")
            throw new XycUiCommandException("OPERATION_NOT_ALLOWED");
        if (DisappearsBeforeInvoke) throw new XycUiCommandException("CONTROL_NOT_FOUND");
        var before = InspectState();
        Invocations++;
        return Task.FromResult(new XycUiControlInvokeState(control, before, InspectState(), true,
            CaptureTop("before.png"), CaptureTop("after.png")));
    }
    public Task<XycUiState> WaitUntilAsync(string state, JsonElement expected,
        int timeoutMs, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (timeoutMs < 50) return Task.FromException<XycUiState>(new TimeoutException());
        return Task.FromResult(InspectState());
    }
}

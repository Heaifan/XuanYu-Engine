using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Platform;
using XuanYu.Editor.UI;

namespace XuanYu.Editor.UI.XycControl;

internal sealed partial class XycUiRuntime(Window window, string? evidenceDirectory)
    : IXycUiRuntime
{
    public XycUiWindowState InspectWindow() => OnUi(InspectWindowCore);
    public XycUiWindowState ResizeWindow(double widthDip, double heightDip) =>
        OnUi(() => ResizeWindowCore(widthDip, heightDip));
    public XycUiThemeState InspectTheme() => OnUi(InspectThemeCore);
    public XycUiThemeState SetTheme(string theme) => OnUi(() => SetThemeCore(theme));
    public XycUiRegionState InspectTop() => OnUi(InspectTopCore);
    public XycUiCaptureState CaptureTop(string fileName, string backend = "avaloniaWindow",
        bool temporaryTopmost = false, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = OnUi(() => CaptureTopCore(fileName, backend, temporaryTopmost,
            cancellationToken));
        cancellationToken.ThrowIfCancellationRequested();
        return result;
    }
    public IReadOnlyList<XycUiControlState> ListControls() => OnUi(ListControlsCore);
    public XycUiControlState InspectControl(string id) => OnUi(() => InspectControlCore(id));
    public XycUiState InspectState() => OnUi(InspectStateCore);
    public Task<XycUiControlInvokeState> InvokeControlAsync(string id, string operation,
        CancellationToken cancellationToken) =>
        InvokeControlWithEvidenceAsync(id, operation, cancellationToken);
    public Task<XycUiState> WaitUntilAsync(string state, System.Text.Json.JsonElement expected,
        int timeoutMs, CancellationToken cancellationToken) =>
        WaitUntilCoreAsync(state, expected, timeoutMs, cancellationToken);

    static T OnUi<T>(Func<T> action) => Dispatcher.UIThread.CheckAccess()
        ? action() : Dispatcher.UIThread.InvokeAsync(action).GetAwaiter().GetResult();

    nint GetHwnd()
    {
        var handle = window.TryGetPlatformHandle();
        if (handle is null || handle.HandleDescriptor != "HWND" || handle.Handle == 0)
            throw new InvalidOperationException("生产 Editor HWND 未就绪。");
        var pid = XycUiWindowNative.GetWindowThreadProcessId(handle.Handle, out var owner);
        if (pid == 0 || owner != Environment.ProcessId)
            throw new UnauthorizedAccessException("窗口不属于当前 Editor 进程。");
        return handle.Handle;
    }

    Top FindTop() => window.GetVisualDescendants().OfType<Top>().SingleOrDefault()
        ?? throw new InvalidOperationException("生产 TOP 尚未加载。");
}

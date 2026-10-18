using System.Text.Json;

namespace XuanYu.Editor.UI.XycControl;

public interface IXycUiRuntime
{
    XycUiWindowState InspectWindow();
    XycUiWindowState ResizeWindow(double widthDip, double heightDip);
    XycUiThemeState InspectTheme();
    XycUiThemeState SetTheme(string theme);
    XycUiRegionState InspectTop();
    XycUiCaptureState CaptureTop(string fileName, string backend = "avaloniaWindow",
        bool temporaryTopmost = false, CancellationToken cancellationToken = default);
    IReadOnlyList<XycUiControlState> ListControls();
    XycUiControlState InspectControl(string controlId);
    XycUiState InspectState();
    Task<XycUiControlInvokeState> InvokeControlAsync(string controlId,
        string operation, CancellationToken cancellationToken);
    Task<XycUiState> WaitUntilAsync(string state, JsonElement expected,
        int timeoutMs, CancellationToken cancellationToken);
}

public sealed record XycUiWindowState(int ProcessId, string Hwnd, bool Visible,
    bool Foreground, string WindowState, double WidthDip, double HeightDip,
    int ClientWidthPx, int ClientHeightPx, int Dpi, double DpiScale);
public sealed record XycUiThemeState(string Requested, string Actual);
public sealed record XycUiRegionState(string Region, double X, double Y,
    double WidthDip, double HeightDip, int Xpx, int Ypx, int WidthPx, int HeightPx);
public sealed record XycUiCaptureState(string ImagePath, string MetadataPath,
    string Sha256, int PixelWidth, int PixelHeight, XycUiRegionState Region,
    XycUiWindowState Window, XycUiThemeState Theme, string BuildIdentity,
    string CaptureMethod, DateTimeOffset CaptureTimeUtc,
    bool? ForegroundUnchanged = null, bool? ZOrderUnchanged = null,
    string CaptureBackend = "unknown", bool? ChangesZOrder = null,
    bool? TakesForeground = null, bool? RequiresVisibleWindow = null,
    bool? SupportsOcclusion = null, string FidelityValidation = "NOT_EVALUATED");
public sealed record XycUiControlState(string? ControlId, string Type, bool Visible,
    bool Enabled, bool DuplicateId, string IdSource, string Region, bool? IsChecked);
public sealed record XycUiState(string ActiveTool, bool IsSelectTool, bool IsEditMode,
    bool IsRegionEditMode, string FooterMessage, string FooterState,
    long CameraRevision, double CameraX, double CameraY, double CameraZ);
public sealed record XycUiControlInvokeState(XycUiControlState Control,
    XycUiState Before, XycUiState After, bool Invoked,
    XycUiCaptureState? BeforeCapture, XycUiCaptureState? AfterCapture);
internal sealed class XycUiCommandException(string code) : Exception(code)
{
    public string Code { get; } = code;
}

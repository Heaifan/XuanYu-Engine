namespace XuanYu.Editor.UI.XycControl;

public interface IXycUiRuntime
{
    XycUiWindowState InspectWindow();
    XycUiWindowState ResizeWindow(double widthDip, double heightDip);
    XycUiThemeState InspectTheme();
    XycUiThemeState SetTheme(string theme);
    XycUiRegionState InspectTop();
    XycUiCaptureState CaptureTop(string fileName);
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
    string CaptureMethod, DateTimeOffset CaptureTimeUtc);

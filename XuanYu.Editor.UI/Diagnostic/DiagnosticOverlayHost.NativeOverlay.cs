using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    Popup CreateDiagnosticNativePopup(Control target) => new()
    {
        PlacementTarget = target,
        Placement = PlacementMode.TopEdgeAlignedLeft,
        IsLightDismissEnabled = false,
        TakesFocusFromNativeControl = false,
        ShouldUseOverlayLayer = false,
        Child = new DiagnosticBadge(target, _clipboard),
    };

    void ReassertToolWindowZOrder()
    {
        if (!OperatingSystem.IsWindows() || _toolWindow is not { } tool ||
            tool.TryGetPlatformHandle() is not { Handle: var handle }) return;
        SetWindowPos(handle, HwndTop, 0, 0, 0, 0, NoActivate | NoMove | NoSize | ShowWindow);
    }

    const uint NoSize = 0x0001, NoMove = 0x0002, ShowWindow = 0x0040, NoActivate = 0x0010;
    static readonly nint HwndTop = 0;

    [System.Runtime.InteropServices.DllImport("user32")]
    [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
    static extern bool SetWindowPos(nint hWnd, nint after, int x, int y, int cx, int cy, uint flags);
}

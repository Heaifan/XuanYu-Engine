using System.Runtime.InteropServices;

namespace FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Input.Pointer;

/// <summary>Win32 鼠标跟踪管理。封装 TrackMouseEvent 和跟踪状态。</summary>
sealed class NativeViewportMouseTrack
{
    bool _isTracking;

    public bool IsTracking => _isTracking;
    public bool IsTrackingOrClear => _isTracking;

    public void Begin(nint hwnd)
    {
        if (hwnd == 0 || _isTracking) return;
        var tme = new TRACKMOUSEEVENT
        {
            cbSize = Marshal.SizeOf<TRACKMOUSEEVENT>(),
            dwFlags = 0x00000002u,
            hwndTrack = hwnd
        };
        TrackMouseEvent(ref tme);
        _isTracking = true;
    }

    public void Reset() => _isTracking = false;

    [DllImport("user32.dll", EntryPoint = "TrackMouseEvent", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool TrackMouseEvent(ref TRACKMOUSEEVENT tme);

    [StructLayout(LayoutKind.Sequential)]
    struct TRACKMOUSEEVENT
    {
        public int cbSize;
        public uint dwFlags;
        public nint hwndTrack;
        public uint dwHoverTime;
    }
}

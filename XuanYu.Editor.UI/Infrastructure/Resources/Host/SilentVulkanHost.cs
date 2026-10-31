using Avalonia.Threading;
using XuanYu.Core.Diagnostics;
using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

/// <summary>Attaches the production Editor projection to an invisible Win32 Vulkan surface.</summary>
internal sealed class SilentVulkanHost : IDisposable
{
    public const int ViewportWidth = 776;
    public const int ViewportHeight = 650;
    static readonly object CurrentGate = new();
    static SilentVulkanHost? _current;
    readonly SilentNativeWindow _window;
    readonly INativeHostSurfaceBridge _bridge;
    readonly SilentCaptureEvidence _capture = new();
    readonly string? _attachFailure;
    bool _disposed;

    SilentVulkanHost(SilentNativeWindow window, INativeHostSurfaceBridge bridge, string? attachFailure)
    { _window = window; _bridge = bridge; _attachFailure = attachFailure; }

    public static SilentVulkanHost? Current { get { lock (CurrentGate) return _current; } }
    public static bool IsSilentRequested(string[] args) => args.Contains("--xyc-silent");
    public bool IsAttached => _attachFailure is null;

    public static SilentVulkanHost Start(UiVm vm, INativeHostSurfaceBridgeFactory? factory)
    {
        if (factory is null) throw new InvalidOperationException("Editor Vulkan SurfaceBridge factory 未注入。");
        if (!Dispatcher.UIThread.CheckAccess()) throw new InvalidOperationException("静默宿主必须在 UI Dispatcher 创建。");
        vm.UpdateViewportDpi(1d);
        vm.UpdateViewportFrame(ViewportWidth, ViewportHeight);
        var window = SilentNativeWindow.Create(ViewportWidth, ViewportHeight);
        INativeHostSurfaceBridge? bridge = null;
        try
        {
            var size = window.ClientSize;
            if (size.Width != ViewportWidth || size.Height != ViewportHeight)
                throw new InvalidOperationException($"Hidden HWND client size mismatch: {size.Width}x{size.Height}");
            bridge = factory.Create(message => Console.Error.WriteLine($"[XYC-SILENT-VULKAN] {message}"), vm);
            var attached = bridge.Attach(new NativeHostSurfaceHandle(window.NativeHandle,
                SilentNativeWindow.ModuleHandle, size.Width, size.Height, 1d));
            if (!attached)
                throw new InvalidOperationException("Vulkan bridge Attach returned false.");
            var host = new SilentVulkanHost(window, bridge,
                null);
            Console.Error.WriteLine($"[XYC-SILENT-HOST] HWND=0x{window.Handle:X};Visible={window.Visible};" +
                $"Client={size.Width}x{size.Height};Attached={attached};Mode=HIDDEN-HWND-SWAPCHAIN-PRESENT");
            lock (CurrentGate) _current = host;
            return host;
        }
        catch
        {
            try { bridge?.Dispose(); } finally { window.Dispose(); }
            throw;
        }
    }

    public SilentHostDiagnostics Inspect()
    {
        var frame = ViewportProbe.LastPresented;
        _capture.Update(frame);
        var size = _window.ClientSize;
        return new("HIDDEN-HWND-SWAPCHAIN-PRESENT", _window.Handle, _window.Exists,
            _window.Visible, _window.HasVisibleStyle, size.Width, size.Height,
            ViewportWidth, ViewportHeight, _attachFailure, IsAttached, frame is not null,
            _capture.Captured, ViewportProbe.CurrentFrameId, frame?.FrameId,
            frame?.CameraRevision, frame?.ImageIndex, frame?.CaptureHeld,
            TerrainReplayOptions.Enabled, _window.IsOwnProcessForeground(),
            _capture.Path, _capture.Sha256, ViewportProbeIdentity.Current());
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        try { _bridge.Dispose(); }
        finally
        {
            _window.Dispose();
            lock (CurrentGate) if (ReferenceEquals(_current, this)) _current = null;
            Console.Error.WriteLine($"[XYC-SILENT-HOST-CLOSED] HWND=0x{_window.Handle:X};Exists={_window.Exists}");
        }
    }
}

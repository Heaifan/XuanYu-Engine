namespace FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Input.Arbitration;

/// <summary>Override 导航鼠标捕获状态管理。</summary>
sealed class NativeViewportNavigationCapture
{
    bool _isActive;
    bool _dragCaptured;

    public bool IsActive => _isActive;
    public bool DragCaptured => _dragCaptured;

    public void SetActive() { _isActive = true; _dragCaptured = false; }
    public void BeginDrag() { _isActive = true; _dragCaptured = true; }

    public void End()
    {
        _isActive = false;
        _dragCaptured = false;
    }

    public void ClearState()
    {
        _isActive = false;
        _dragCaptured = false;
    }
}

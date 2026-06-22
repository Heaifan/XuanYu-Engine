namespace FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Input.Arbitration;

/// <summary>场景工具鼠标捕获状态管理。</summary>
sealed class NativeViewportSceneToolCapture
{
    bool _isActive;
    bool _dragCaptured;

    public bool IsActive => _isActive;
    public bool DragCaptured => _dragCaptured;

    public void BeginDrag()
    {
        _isActive = true;
        _dragCaptured = true;
    }

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

using XuanYu.Engine.Editor.Windows.Panels.Viewport.NativeHost.Input.Pointer;
using XuanYu.Engine.Render.ViewportNavigation;

namespace XuanYu.Engine.Editor.Windows.Panels.Viewport.NativeHost.Input.Arbitration;

sealed class NativeViewportInputArbitration
{
    public NativeViewportNavigationCapture NavCapture { get; } = new();
    public NativeViewportSceneToolCapture ToolCapture { get; } = new();

    public NativeViewportInputArbitrationConsumer HandleLeftDown(int mx, int my,
        NativeViewportMouseCapture mouseCapture, nint hwnd,
        Func<int, int, ViewportNavigationPressResult> navigationPressed,
        Func<int, int, ViewportSceneToolPressResult> sceneToolPressed,
        Action<int, int> legacyPickDown,
        Action<int, int, int> rawButtonDown)
    {
        var navResult = navigationPressed(mx, my);
        if (navResult != ViewportNavigationPressResult.NotHandled)
        {
            if (navResult == ViewportNavigationPressResult.BeginDrag)
            { NavCapture.BeginDrag(); mouseCapture.Capture(hwnd, "Overlay导航", "左键"); }
            else NavCapture.SetActive();
            return NativeViewportInputArbitrationConsumer.Navigation;
        }
        var toolResult = sceneToolPressed(mx, my);
        if (toolResult == ViewportSceneToolPressResult.BeginDrag)
        { ToolCapture.BeginDrag(); mouseCapture.Capture(hwnd, "MoveGizmo", "左键"); return NativeViewportInputArbitrationConsumer.SceneTool; }
        legacyPickDown(mx, my);
        rawButtonDown(1, mx, my);
        return NativeViewportInputArbitrationConsumer.Legacy;
    }

    public void HandleLeftUp(int mx, int my,
        NativeViewportMouseCapture mouseCapture,
        Action navigationReleased,
        Action<int, int> sceneToolReleased,
        Action<int, int> legacyPickUp,
        Action<int, int, int> rawButtonUp)
    {
        if (NavCapture.IsActive)
        {
            var wasDrag = NavCapture.DragCaptured;
            NavCapture.End();
            navigationReleased();
            if (wasDrag) mouseCapture.Release("WM_LBUTTONUP");
        }
        else if (ToolCapture.IsActive)
        {
            var wasDrag = ToolCapture.DragCaptured;
            ToolCapture.End();
            sceneToolReleased(mx, my);
            if (wasDrag) mouseCapture.Release("WM_LBUTTONUP");
        }
        else
        { legacyPickUp(mx, my); rawButtonUp(1, mx, my); }
    }

    public void HandleKillFocus(
        Action legacyKillFocus,
        bool rawDragWasActive,
        Action rawFocusLost,
        Action navCaptureLost,
        Action toolFocusLost)
    {
        legacyKillFocus();
        if (rawDragWasActive) rawFocusLost();
        var hadNav = NavCapture.DragCaptured || NavCapture.IsActive;
        NavCapture.ClearState();
        var hadTool = ToolCapture.DragCaptured || ToolCapture.IsActive;
        ToolCapture.ClearState();
        if (hadNav) navCaptureLost();
        if (hadTool) toolFocusLost();
    }

    public void HandleCaptureChanged(
        NativeViewportMouseCapture mouseCapture,
        Action navigationCaptureLost,
        Action rawFocusLost,
        bool rawDragWasActive)
    {
        if (NavCapture.DragCaptured || NavCapture.IsActive)
        { NavCapture.ClearState(); navigationCaptureLost(); }
        if (ToolCapture.DragCaptured || ToolCapture.IsActive)
        { ToolCapture.ClearState(); rawFocusLost(); }
    }

    public void Reset()
    {
        NavCapture.ClearState();
        ToolCapture.ClearState();
    }
}

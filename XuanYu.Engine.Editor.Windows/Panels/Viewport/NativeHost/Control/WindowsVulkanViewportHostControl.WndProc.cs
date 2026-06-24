using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Input.Pointer;
using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Input.Keyboard;
using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Input.Focus;
using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Win32;
using FluidWarfare.Render.ViewportNavigation;

namespace FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost;

partial class WindowsVulkanViewportHostControl
{
    nint DispatchWndProc(nint hwnd, uint msg, nint wParam, nint lParam)
    {
        var parsed = _pointerMessages.Parse(msg, wParam, lParam);
        if (parsed is not null)
        {
            switch (parsed.Action)
            {
                case NativeViewportPointerAction.LeftDown: return HandleLeftDown(parsed.X, parsed.Y);
                case NativeViewportPointerAction.LeftUp: return HandleLeftUp(parsed.X, parsed.Y);
                case NativeViewportPointerAction.MiddleDown: return HandleMiddleDown(parsed.X, parsed.Y);
                case NativeViewportPointerAction.MiddleUp: return HandleMiddleUp(parsed.X, parsed.Y);
                case NativeViewportPointerAction.Move: return HandlePointerMove(parsed.X, parsed.Y);
                case NativeViewportPointerAction.Leave: _mouseTrack.Reset(); PointerLeft?.Invoke(); return 0;
                case NativeViewportPointerAction.Wheel: return HandleWheel(parsed.X, parsed.Y, parsed.WheelDelta, parsed.ModifierFlags);
                case NativeViewportPointerAction.CaptureChanged: HandleCaptureChanged(); return 0;
            }
        }
        var key = _keyboardMessages.Parse(msg, wParam);
        if (key is not null)
        {
            _focusMessages.SetFocusTo(_windowHandle);
            if (key.Action == NativeViewportKeyboardAction.Down) RawKeyDown?.Invoke(key.VirtualKeyCode);
            else RawKeyUp?.Invoke(key.VirtualKeyCode);
            return 0;
        }
        if (_focusMessages.IsKillFocus(msg))
        {
            _arbitration.HandleKillFocus(() => _pickInput.OnKillFocus(), _rawPointerDragCaptured,
                () => RawInputFocusLost?.Invoke(), () => NavigationCaptureLost?.Invoke(),
                () => RawInputFocusLost?.Invoke());
            _rawPointerDragCaptured = false;
            return 0;
        }
        return NativeViewportHitTestMessages.IsHitTest(msg)
            ? Win32ViewportDefaultProc.DefWindowProc(hwnd, msg, wParam, lParam)
            : Win32ViewportDefaultProc.DefWindowProc(hwnd, msg, wParam, lParam);
    }

    nint HandleMiddleDown(int x, int y)
    {
        _focusMessages.SetFocusTo(_windowHandle);
        Trace($"[InputTrace-NativeHost] WM_MBUTTONDOWN code=4(Middle) x={x} y={y}");
        _mouseCapture.Capture(_windowHandle); _rawPointerDragCaptured = true;
        RawPointerButtonDown?.Invoke(NativeViewportPointerMessages.VkMButton, x, y); return 0;
    }
    nint HandleMiddleUp(int x, int y)
    {
        Trace($"[InputTrace-NativeHost] WM_MBUTTONUP code=4(Middle)");
        _mouseCapture.ClearState();
        RawPointerButtonUp?.Invoke(NativeViewportPointerMessages.VkMButton, x, y); return 0;
    }
    nint HandlePointerMove(int x, int y)
    {
        _mouseTrack.Begin(_windowHandle);
        var navConsumed = NavigationPointerMoved?.Invoke(x, y) == true;
        if (_arbitration.NavCapture.DragCaptured) navConsumed = true;
        if (!navConsumed) { RawPointerMoved?.Invoke(x, y); PointerMoved?.Invoke(x, y); }
        return 0;
    }
    nint HandleWheel(int x, int y, int d, int m)
    { Trace($"[InputTrace-NativeHost] WM_MOUSEWHEEL delta={d} mk=0x{m:X4}"); RawMouseWheel?.Invoke(d, m); return 0; }
    nint HandleLeftDown(int mx, int my)
    {
        _focusMessages.SetFocusTo(_windowHandle);
        _arbitration.HandleLeftDown(mx, my, _mouseCapture, _windowHandle,
            (x, y) => NavigationPointerPressed?.Invoke(x, y) ?? ViewportNavigationPressResult.NotHandled,
            (x, y) => SceneToolPointerPressed?.Invoke(x, y) ?? ViewportSceneToolPressResult.NotHandled,
            (x, y) => _pickInput.OnDown(x, y),
            (c, x, y) => RawPointerButtonDown?.Invoke(c, x, y));
        return 0;
    }
    nint HandleLeftUp(int mx, int my)
    {
        _arbitration.HandleLeftUp(mx, my, _mouseCapture,
            () => NavigationPointerReleased?.Invoke(), (x, y) => SceneToolPointerReleased?.Invoke(x, y),
            (x, y) => _pickInput.OnUp(x, y), (c, x, y) => RawPointerButtonUp?.Invoke(c, x, y));
        return 0;
    }
    void HandleCaptureChanged()
    {
        if (_rawPointerDragCaptured) { _rawPointerDragCaptured = false; RawInputFocusLost?.Invoke(); }
        _arbitration.HandleCaptureChanged(_mouseCapture, () => NavigationCaptureLost?.Invoke(), () => RawInputFocusLost?.Invoke(), false);
    }
}

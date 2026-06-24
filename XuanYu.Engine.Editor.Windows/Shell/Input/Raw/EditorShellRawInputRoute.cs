using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost;

namespace FluidWarfare.Editor.Windows.Shell.Input.Raw;

/// <summary>原始输入转发路由。负责将原生视口事件的原始参数转发到 ViewportInputRoute。</summary>
sealed class EditorShellRawInputRoute(
    EditorViewportInputRoute viewportInputRoute,
    Func<EditorViewportInputKind, int, int, int, int, int, EditorViewportInputRequest> buildRequest)
{
    public void HandleRawKeyDown(int virtualKeyCode) =>
        viewportInputRoute.HandleKeyDown(buildRequest(EditorViewportInputKind.KeyDown, virtualKeyCode, 0, 0, 0, 0));

    public void HandleRawKeyUp(int virtualKeyCode) =>
        viewportInputRoute.HandleKeyUp(buildRequest(EditorViewportInputKind.KeyUp, virtualKeyCode, 0, 0, 0, 0));

    public void HandleRawPointerButtonDown(int buttonCode, int x, int y) =>
        viewportInputRoute.HandlePointerDown(buildRequest(EditorViewportInputKind.PointerDown, 0, buttonCode, x, y, 0));

    public void HandleRawPointerMoved(int x, int y) =>
        viewportInputRoute.HandlePointerMoved(buildRequest(EditorViewportInputKind.PointerMoved, 0, 0, x, y, 0));

    public void HandleRawPointerButtonUp(int buttonCode, int x, int y) =>
        viewportInputRoute.HandlePointerUp(buildRequest(EditorViewportInputKind.PointerUp, 0, buttonCode, x, y, 0));

    public void HandleRawInputFocusLost() =>
        viewportInputRoute.HandleFocusLost(buildRequest(EditorViewportInputKind.FocusLost, 0, 0, 0, 0, 0));

    public void HandleRawMouseWheel(int delta, int packedModifiers) =>
        viewportInputRoute.HandleMouseWheel(buildRequest(EditorViewportInputKind.MouseWheel, 0, 0, 0, 0, delta));

    public ViewportSceneToolPressResult HandleSceneToolPointerPressed(int x, int y) =>
        viewportInputRoute.HandleSceneToolPressed(buildRequest(EditorViewportInputKind.PointerDown, 0, 0, x, y, 0));

    public void HandleSceneToolPointerReleased(int x, int y) =>
        viewportInputRoute.HandleSceneToolReleased(buildRequest(EditorViewportInputKind.PointerUp, 0, 0, x, y, 0));
}

using XuanYu.Engine.Render.Camera;
using XuanYu.Engine.Render.Camera.Navigation;
using XuanYu.Engine.Render.ViewportNavigation;
using XuanYu.Engine.Editor.Windows.Viewport.Camera;
namespace XuanYu.Engine.Editor.Windows.Viewport.Navigation;

/// <summary>Viewport 右上角导航 Overlay 输入路由。拥有拖拽/悬停/选中状态，通过 Press/Move/Release/ClearHover 处理输入。</summary>
public sealed class ViewportNavigationRoute
{
    private ViewportNavigationDragMode _dragMode;
    private ViewportNavigationElement _activeElement;
    private ViewportNavigationElement _hoverElement;
    private int _lastPixelX;
    private int _lastPixelY;
    public ViewportNavigationDragMode DragMode => _dragMode;
    public ViewportNavigationElement HoverElement => _hoverElement;

    public ViewportNavigationPressResponse Press(int pixelX, int pixelY, ViewportNavigationLayout layout)
    {
        var element = ViewportNavigationHitTest.HitTest(pixelX, pixelY, layout);
        if (element == ViewportNavigationElement.None)
            return new(ViewportNavigationPressResult.NotHandled, element, null, null);
        _activeElement = element;
        _lastPixelX = pixelX;
        _lastPixelY = pixelY;
        var action = ViewportNavigationHitTest.ElementToAction(element);
        if (IsSnapAction(action))
            return new(ViewportNavigationPressResult.HandledClick, element,
                new ViewportCameraCommand.SnapToView(ToSceneView(action)), null);
        if (action == ViewportNavigationAction.Orbit) { _dragMode = ViewportNavigationDragMode.GizmoOrbit; return BeginDrag(element); }
        if (action == ViewportNavigationAction.Pan) { _dragMode = ViewportNavigationDragMode.Pan; return BeginDrag(element); }
        if (action == ViewportNavigationAction.Frame)
            return new(ViewportNavigationPressResult.HandledClick, element, new ViewportCameraCommand.FrameAll(), null);
        if (action == ViewportNavigationAction.ToggleProjection)
            return new(ViewportNavigationPressResult.HandledClick, element, new ViewportCameraCommand.ToggleProjection(), null);
        return new(ViewportNavigationPressResult.HandledClick, element, null, null);
    }
    public ViewportNavigationMoveResponse Move(int pixelX, int pixelY,
        ViewportNavigationLayout? layout, ViewportCameraRoute cameraRoute, int viewportHeightFallback)
    {
        if (_dragMode != ViewportNavigationDragMode.None)
        {
            var deltaX = pixelX - _lastPixelX;
            var deltaY = pixelY - _lastPixelY;
            _lastPixelX = pixelX;
            _lastPixelY = pixelY;
            if (deltaX == 0 && deltaY == 0)
                return new(true, false, false, _hoverElement, _activeElement);
            var vh = layout?.ViewportHeight ?? Math.Max(1, viewportHeightFallback);
            var prev = cameraRoute.LastCameraState;
            var newState = _dragMode switch
            {
                ViewportNavigationDragMode.GizmoOrbit => SceneOrbitCameraMotion.Orbit(prev, -deltaX, -deltaY),
                ViewportNavigationDragMode.Pan => SceneOrbitCameraMotion.Pan(prev, deltaX, deltaY, vh),
                ViewportNavigationDragMode.Zoom => SceneOrbitCameraMotion.Dolly(prev, -deltaY),
                _ => prev
            };
            if (newState != prev) { cameraRoute.SetState(newState); return new(true, true, false, _hoverElement, _activeElement); }
            return new(true, false, false, _hoverElement, _activeElement);
        }

        if (layout is null) return new(false, false, false, _hoverElement, _activeElement);
        var element = ViewportNavigationHitTest.HitTest(pixelX, pixelY, layout);
        if (element != _hoverElement) { _hoverElement = element; return new(true, false, true, element, _activeElement); }
        return new(element != ViewportNavigationElement.None, false, false, _hoverElement, _activeElement);
    }
    public ViewportNavigationReleaseResponse Release(bool requestCleanupFrame)
    {
        var hadState = _activeElement != ViewportNavigationElement.None || _dragMode != ViewportNavigationDragMode.None;
        _dragMode = ViewportNavigationDragMode.None;
        _activeElement = ViewportNavigationElement.None;
        _lastPixelX = 0;
        _lastPixelY = 0;
        return new(hadState && requestCleanupFrame);
    }
    public ViewportNavigationMoveResponse ClearHover()
    {
        if (_dragMode != ViewportNavigationDragMode.None) return new(false, false, false, _hoverElement, _activeElement);
        _hoverElement = ViewportNavigationElement.None;
        _activeElement = ViewportNavigationElement.None;
        return new(false, false, true, ViewportNavigationElement.None, ViewportNavigationElement.None);
    }

    private static ViewportNavigationPressResponse BeginDrag(ViewportNavigationElement e) =>
        new(ViewportNavigationPressResult.BeginDrag, e, null, null);
    private static bool IsSnapAction(ViewportNavigationAction a) => a is
        ViewportNavigationAction.SnapPositiveX or ViewportNavigationAction.SnapNegativeX or
        ViewportNavigationAction.SnapPositiveY or ViewportNavigationAction.SnapNegativeY or
        ViewportNavigationAction.SnapPositiveZ or ViewportNavigationAction.SnapNegativeZ;
    private static SceneNavigationView ToSceneView(ViewportNavigationAction a) => a switch
    {
        ViewportNavigationAction.SnapPositiveX => SceneNavigationView.PositiveX,
        ViewportNavigationAction.SnapNegativeX => SceneNavigationView.NegativeX,
        ViewportNavigationAction.SnapPositiveY => SceneNavigationView.PositiveY,
        ViewportNavigationAction.SnapNegativeY => SceneNavigationView.NegativeY,
        ViewportNavigationAction.SnapPositiveZ => SceneNavigationView.PositiveZ,
        ViewportNavigationAction.SnapNegativeZ => SceneNavigationView.NegativeZ,
        _ => SceneNavigationView.Free
    };
}

using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Core.Gizmo;
using XuanYu.Editor.Input;
using XuanYu.Editor.Camera;

namespace XuanYu.World.Tests.Viewport.InputIntegration;

public sealed partial class CursorAnchoredZoomRegressionTests
{
    static CameraState Apply(CameraState camera, Vector3d center, Vector3d anchor,
        double wheelDelta, int count, out Vector3d resultCenter)
    {
        resultCenter = center;
        var current = camera;
        for (var i = 0; i < count; i++)
        {
            Assert.True(CameraNavigation.TryDolly(current, resultCenter, wheelDelta,
                current.Revision + 1, anchor, out var result, out var reason), reason);
            current = result.Camera;
            resultCenter = result.ObservationCenter;
        }
        return current;
    }

    static double AnchorDrift(CameraState camera, Vector3d center, ViewportState viewport,
        Vector3d anchor, double x, double y)
    {
        var screen = ViewProjectionState.Create(camera, viewport).ProjectWorldPoint(anchor);
        return Math.Sqrt(Math.Pow(screen.X - x, 2) + Math.Pow(screen.Y - y, 2));
    }

    static Vector3d ReferencePlaneAnchor(ViewProjectionState projection, double x, double y, double elevation)
    {
        var ray = WorldRayFactory.FromViewportPoint(projection, x, y);
        var distance = (elevation - ray.Origin.Z) / ray.Direction.Z;
        Assert.True(double.IsFinite(distance) && distance >= 0);
        return ray.Origin + ray.Direction * distance;
    }

    static double Distance(ScreenPoint a, ScreenPoint b) =>
        Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));

    static EditorPointerEvent Wheel(double x, double y, double delta) => new(
        EditorPointerEventKind.Wheel,
        new(x, y),
        EditorPointerButtons.None,
        EditorPointerModifiers.None,
        delta,
        1,
        new("test"),
        1);
}

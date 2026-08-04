using XuanYu.Core.Gizmo;
using XuanYu.Core.Identity;
using XuanYu.Core.Space;
using XuanYu.Editor.UI;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.World;

public sealed partial class ScaleTransformUiTests
{
    static (double X, double Y, ViewportState Viewport) ScaleHit(UiVm vm, ScaleGizmoHandle handle)
    {
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var camera = vm.RenderSnapshot.CameraState;
        var origin = vm.RenderSnapshot.Entity.Transform.Position;
        var length = ScaleGizmoScreenSize.ComputeWorldAxisLength(camera, viewport, origin);
        var state = ViewProjectionState.Create(camera, viewport);
        var layout = ScaleGizmoLayout.Project(state, origin, length, default);
        var p = handle == ScaleGizmoHandle.Uniform
            ? layout.Center
            : layout.AxisEnd[handle == ScaleGizmoHandle.X ? 0 : (handle == ScaleGizmoHandle.Y ? 1 : 2)];
        return (p.X, p.Y, viewport);
    }

    static (double X, double Y) ScalePull(UiVm vm, ScaleGizmoHandle handle,
        ViewportState viewport, double d)
    {
        var camera = vm.RenderSnapshot.CameraState;
        var origin = vm.RenderSnapshot.Entity.Transform.Position;
        var length = ScaleGizmoScreenSize.ComputeWorldAxisLength(camera, viewport, origin);
        var layout = ScaleGizmoLayout.Project(
            ViewProjectionState.Create(camera, viewport), origin, length, default);
        if (handle == ScaleGizmoHandle.Uniform) return (layout.Center.X, layout.Center.Y - d);
        var i = handle == ScaleGizmoHandle.X ? 0 : (handle == ScaleGizmoHandle.Y ? 1 : 2);
        var dx = layout.AxisEnd[i].X - layout.Center.X;
        var dy = layout.AxisEnd[i].Y - layout.Center.Y;
        var len = System.Math.Sqrt(dx * dx + dy * dy);
        return (layout.AxisEnd[i].X + (dx / len) * d, layout.AxisEnd[i].Y + (dy / len) * d);
    }

    static (double X, double Y, ViewportState Viewport) ScreenOf(
        SceneStateOwner scene, EntityId key, UiVm vm)
    {
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var state = ViewProjectionState.Create(vm.RenderSnapshot.CameraState, viewport);
        scene.TryGetEntity(key, out var entity);
        var center = state.ProjectWorldPoint(entity.Transform.Position);
        return (center.X, center.Y, viewport);
    }
}

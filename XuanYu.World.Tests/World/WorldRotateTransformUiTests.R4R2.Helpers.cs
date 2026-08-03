using System.Reflection;
using XuanYu.Core.Gizmo;
using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Space;
using XuanYu.Editor.UI;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.World;

public sealed partial class WorldRotateTransformUiTests
{
    static UiVm RotateVmTwoEntities(out SceneStateOwner scene, out EntityId bKey)
    {
        var vm = new UiVm(null, () => true);
        var field = typeof(UiVm).GetField("_sceneState",
            BindingFlags.Instance | BindingFlags.NonPublic);
        scene = (SceneStateOwner)field!.GetValue(vm)!;
        bKey = scene.CreateEntity("实体B", "Unit",
            new CommittedTransform(new Vector3d(5, 0, 0), Vector3d.Zero, new Vector3d(1, 1, 1))).EntityKey;
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(i => i.Key == "EntityId(1)");
        vm.SelectToolCommand.Execute("旋转");
        vm.UpdateViewportFrame(800, 600); // 触发 FrameAll，把 A+B 框入视野，B 才会投影在视口内
        return vm;
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

    static (double StartX, double StartY, double MidX, double MidY, double EndX, double EndY)
        RingHitFor(UiVm vm, ViewportState viewport)
    {
        var camera = vm.RenderSnapshot.CameraState;
        var origin = vm.RenderSnapshot.Entity.Transform.Position;
        var worldRadius = RotateGizmoScreenRadius.ComputeWorldRadius(camera, viewport, origin);
        var state = ViewProjectionState.Create(camera, viewport);
        var layout = RotateGizmoLayout.Project(state, origin, worldRadius);
        var ring = layout.Rings.Single(r => r.Axis == RotateGizmoAxis.Z);
        var start = ring.Points[6];
        var mid = ring.Points[9];
        var end = ring.Points[15];
        return (start.X, start.Y, mid.X, mid.Y, end.X, end.Y);
    }
}

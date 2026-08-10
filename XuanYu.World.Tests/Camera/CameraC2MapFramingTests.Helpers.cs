using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.World;

static class CameraC2MapFramingTestsHelpers
{
    public static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);

    public static UiVm MapVm(bool seedInitialScene = true)
    {
        var vm = new UiVm(null, () => true, seedInitialScene);
        vm.RightTabIndex = 1;
        vm.UpdateViewportFrame(800, 600);
        return vm;
    }

    public static (UiVm Vm, MapPoint[] Points) DraftVm(int count)
    {
        var vm = MapVm(false);
        vm.SelectToolCommand.Execute("区域绘制");
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.CameraState, Viewport);
        var points = new List<MapPoint>();
        foreach (var x in Enumerable.Range(1, 19).Select(value => value * 40.0))
        foreach (var y in Enumerable.Range(1, 14).Select(value => value * 40.0))
        {
            if (!MapSurfacePicker.TryPick(vm.MapSession.CurrentMap, projection, x, y, out var point)) continue;
            if (points.Contains(point)) continue;
            Assert.True(vm.RegionDrawingPointerPressed(x, y, Viewport));
            points.Add(point);
            if (points.Count == count) return (vm, points.ToArray());
        }
        throw new InvalidOperationException($"无法建立 {count} 个不同 Draft 顶点。");
    }

    public static Vector3d[] MapCorners(UiVm vm)
    {
        var map = vm.MapSession.CurrentMap;
        var halfWidth = map.SizeMeters.Width / 2.0;
        var halfDepth = map.SizeMeters.Depth / 2.0;
        var height = map.Surface.BaseHeightMeters;
        return
        [
            new(-halfWidth, -halfDepth, height), new(halfWidth, -halfDepth, height),
            new(-halfWidth, halfDepth, height), new(halfWidth, halfDepth, height)
        ];
    }

    public static void AssertMapCornersVisible(UiVm vm)
    {
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.CameraState, Viewport);
        foreach (var corner in MapCorners(vm))
        {
            Assert.True(projection.TryProjectWorldPoint(corner, out var point));
            Assert.InRange(point.X, 0.0, Viewport.LogicalWidth);
            Assert.InRange(point.Y, 0.0, Viewport.LogicalHeight);
        }
    }

    public static void AssertFinite(CameraState camera)
    {
        Assert.True(double.IsFinite(camera.Position.X));
        Assert.True(double.IsFinite(camera.Position.Y));
        Assert.True(double.IsFinite(camera.Position.Z));
        Assert.True(double.IsFinite(camera.OrthographicScale));
        Assert.True(camera.Position.DistanceTo(camera.Position + camera.Forward) > 0);
    }
}

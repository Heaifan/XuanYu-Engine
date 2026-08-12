using System.Reflection;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Tests;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class RegionDrawingF1CStabilityTests
{
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);

    [Fact]
    public void C_R01_focus_during_draft_focuses_draft_safely()
    {
        var vm = DraftVm();

        vm.RunCommand.Execute("聚焦");

        var camera = vm.RenderSnapshot.CameraState;
        Assert.True(camera.Position.DistanceTo(vm.ObservationCenter) > 50.0);
        Assert.Equal("聚焦：当前区域草稿已进入视野。", vm.FooterMessage);
    }

    [Fact]
    public void C_R03_pointer_move_ignores_first_vertex_behind_camera()
    {
        var vm = DraftVm();
        var first = vm.LastRegionDrawingHit!.Value;
        var camera = new CameraState(
            new Vector3d(first.X - 2000, first.Y, 100),
            new Vector3d(-0.5, 0, -0.866025403784), Vector3d.UnitY,
            45, 0.1, 1000, 7);
        typeof(UiVm).GetField("_camera", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(vm, camera);

        var error = Record.Exception(() => vm.RegionDrawingPointerMoved(400, 300, Viewport));

        Assert.Null(error);
        Assert.False(vm.IsRegionDrawingCloseCandidate);
    }

    [Fact]
    public void C_R06_camera_navigation_and_region_pointer_move_do_not_throw()
    {
        var vm = DraftVm();

        Assert.True(vm.BeginCameraNavigation(1, 400, 300, false, 800, 600));
        Assert.True(vm.PreviewCameraNavigation(1, 420, 320));
        Assert.True(vm.EndCameraNavigation(1));
        Assert.Null(Record.Exception(() => vm.RegionDrawingPointerMoved(400, 300, Viewport)));
        Assert.True(vm.DollyCamera(1));
        Assert.Null(Record.Exception(() => vm.RegionDrawingPointerMoved(400, 300, Viewport)));
        Assert.True(vm.BeginCameraNavigation(2, 400, 300, true, 800, 600));
        Assert.True(vm.PreviewCameraNavigation(2, 420, 320));
        Assert.True(vm.EndCameraNavigation(2));
        Assert.Null(Record.Exception(() => vm.RegionDrawingPointerMoved(400, 300, Viewport)));
    }

    [Fact]
    public void Region_drawing_logs_only_low_frequency_actions()
    {
        var vm = DraftVm();
        vm.RegionDrawingPointerMoved(400, 300, Viewport);
        vm.CancelRegionDrawingFromEscape();
        var text = string.Join("\n", vm.LogItems.Select(item => item.Message));

        Assert.Contains("开始区域绘制", text);
        Assert.Contains("已取消区域绘制", text);
        Assert.DoesNotContain("F1TRACE", text);
        Assert.DoesNotContain("PointerMoved", text);
        Assert.DoesNotContain("Ray", text);
    }

    static UiVm DraftVm()
    {
        var vm = RegionDrawingTestVm.Create();
        vm.SelectToolCommand.Execute("区域绘制");
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport);
        var hit = Enumerable.Range(0, 17).SelectMany(ix => Enumerable.Range(0, 13)
            .Select(iy => (X: ix * 50.0, Y: iy * 50.0)))
            .First(p => MapSurfacePicker.TryPick(vm.MapSession.CurrentMap, projection, p.X, p.Y, out _));
        Assert.True(vm.RegionDrawingPointerPressed(hit.X, hit.Y, Viewport));
        return vm;
    }
}

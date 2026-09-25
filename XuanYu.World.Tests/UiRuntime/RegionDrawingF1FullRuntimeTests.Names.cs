using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Tests;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class RegionDrawingF1FullRuntimeTests
{
    [Fact]
    public void Real_region_creation_uses_sequential_names()
    {
        var vm = RegionDrawingTestVm.Create();
        CreateRegion(vm); CreateRegion(vm);

        Assert.Equal(["区域1", "区域2"], vm.MapSession.CurrentMap.Regions.Select(x => x.DisplayName));
    }

    static void CreateRegion(UiVm vm)
    {
        vm.SelectToolCommand.Execute("区域绘制");
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, new(0, 0, 800, 600, 800, 600, 1, 1));
        var points = Enumerable.Range(0, 20).SelectMany(x => Enumerable.Range(0, 15).Select(y => (x * 40d, y * 40d)))
            .Where(point => MapSurfacePicker.TryPick(vm.MapSession.CurrentMap, projection, point.Item1, point.Item2, out _)).Take(3).ToArray();
        foreach (var point in points) vm.RegionDrawingPointerPressed(point.Item1, point.Item2, new(0, 0, 800, 600, 800, 600, 1, 1));
        Assert.True(vm.CommitRegionDrawingFromEnter());
    }
}

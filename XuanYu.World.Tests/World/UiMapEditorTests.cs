using XuanYu.Editor.MapDocument;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.World;

// MAP-A-R1-D5-A：正式地图编辑器——新建/保存/打开/卸载闭环（复用 D2 存储）。
public sealed class UiMapEditorTests
{
    [Fact]
    public void New_map_loads_into_world_and_marks_dirty()
    {
        var vm = new UiVm(null, () => true);
        vm.NewMap();

        Assert.True(vm.HasMap);
        Assert.Equal("TestBattlefield", vm.MapName);
        Assert.Equal("未保存", vm.MapStatusText);
        Assert.True(vm.MapWorld.HasMap);
        Assert.Equal(2000.0, vm.MapWorld.CurrentMap!.WidthMeters);
    }

    [Fact]
    public void Save_then_open_round_trips_full_document()
    {
        var vm = new UiVm(null, () => true);
        vm.NewMap();
        var dir = Path.Combine(Path.GetTempPath(), "xuanYuMapEditorTests");
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, "roundtrip.xymap");

        Assert.True(vm.SaveMapAsync(path).GetAwaiter().GetResult());
        Assert.Equal("已保存", vm.MapStatusText);
        var savedMapId = vm.MapIdText;

        var reopened = new UiVm(null, () => true);
        Assert.True(reopened.OpenMapAsync(path).GetAwaiter().GetResult());
        Assert.Equal("TestBattlefield", reopened.MapName);
        Assert.Equal(savedMapId, reopened.MapIdText);
        Assert.Equal("已保存", reopened.MapStatusText);
        Assert.True(reopened.MapWorld.HasMap);
    }

    [Fact]
    public void Unload_from_editor_clears_world_and_document()
    {
        var vm = new UiVm(null, () => true);
        vm.NewMap();
        vm.UnloadMapFromEditor();

        Assert.False(vm.HasMap);
        Assert.Equal("未加载", vm.MapStatusText);
        Assert.False(vm.MapWorld.HasMap);
    }

    [Fact]
    public void Open_missing_file_fails_without_touching_current_map()
    {
        var vm = new UiVm(null, () => true);
        vm.NewMap();
        var missing = Path.Combine(Path.GetTempPath(), "missing-map.xymap");

        Assert.False(vm.OpenMapAsync(missing).GetAwaiter().GetResult());
        Assert.True(vm.HasMap, "失败时原地图保持不变");
        Assert.Equal("TestBattlefield", vm.MapName);
    }
}

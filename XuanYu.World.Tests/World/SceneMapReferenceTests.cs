using XuanYu.Editor.MapDocument;
using XuanYu.Editor.SceneDocument;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.World;

// MAP-A-R1-D5-B：.xyscene mapReference 闭环——保存携带、打开恢复、缺失失效、旧场景兼容。
public sealed class SceneMapReferenceTests
{
    [Fact]
    public async Task Scene_save_carries_map_reference()
    {
        var vm = new UiVm(null, () => true);
        vm.NewMap();
        await vm.SaveMapAsync(Path.Combine(Path.GetTempPath(), "sceneRefMap.xymap"));
        var scenePath = Path.Combine(Path.GetTempPath(), "sceneRef.xyscene");

        await vm.SaveSceneAsync(scenePath);

        var text = File.ReadAllText(scenePath);
        Assert.Contains("mapReference", text);
        Assert.Contains(vm.MapIdText, text);
        Assert.DoesNotContain("gentleHillsV1\":{\"baseHeightMeters\":0", text);
    }

    [Fact]
    public async Task Scene_open_restores_map_reference()
    {
        var vm = new UiVm(null, () => true);
        vm.NewMap();
        var mapPath = Path.Combine(Path.GetTempPath(), "sceneRefMap2.xymap");
        await vm.SaveMapAsync(mapPath);
        var scenePath = Path.Combine(Path.GetTempPath(), "sceneRef2.xyscene");
        await vm.SaveSceneAsync(scenePath);

        var reopened = new UiVm(null, () => true);
        await reopened.OpenSceneAsync(scenePath);

        Assert.True(reopened.HasMap);
        Assert.Equal(vm.MapIdText, reopened.MapIdText);
    }

    [Fact]
    public async Task Scene_without_map_reference_opens_normally()
    {
        var vm = new UiVm(null, () => true);
        var scenePath = Path.Combine(Path.GetTempPath(), "noRef.xyscene");
        await vm.SaveSceneAsync(scenePath);

        var reopened = new UiVm(null, () => true);
        await reopened.OpenSceneAsync(scenePath);

        Assert.False(reopened.HasMap);
        Assert.Equal("未加载", reopened.MapStatusText);
    }

    [Fact]
    public async Task Missing_map_file_marks_reference_invalid_without_crashing()
    {
        var vm = new UiVm(null, () => true);
        vm.NewMap();
        var mapPath = Path.Combine(Path.GetTempPath(), "willBeDeleted.xymap");
        await vm.SaveMapAsync(mapPath);
        var scenePath = Path.Combine(Path.GetTempPath(), "brokenRef.xyscene");
        await vm.SaveSceneAsync(scenePath);
        File.Delete(mapPath);

        var reopened = new UiVm(null, () => true);
        await reopened.OpenSceneAsync(scenePath);

        Assert.False(reopened.HasMap);
        Assert.Contains("引用失效", reopened.FooterMessage);
        Assert.False(reopened.MapWorld.HasMap, "不自动创建默认地图");
    }
}

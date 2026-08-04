using XuanYu.Editor.MapDocument;
using XuanYu.Editor.SceneDocument;
using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.World;

// MAP-A-R1-D5-B（D3 适配）：.xyscene mapReference 闭环——保存携带、打开恢复、缺失失效、旧场景兼容。
// D3 起 UI 保存/打开按钮禁用，测试改用 MapStorageService + ReplaceCurrentMap 直接驱动会话。
public sealed class SceneMapReferenceTests
{
    static string TempPath(string name) => Path.Combine(Path.GetTempPath(), name);

    static async Task<(UiVm Vm, string MapPath)> VmWithSavedMapAsync(string mapName)
    {
        var mapPath = TempPath(mapName);
        await new MapStorageService().SaveAsync(mapPath, MapDocument.CreateNew("TestBattlefield", 2000, 2000));
        var loaded = await new MapStorageService().LoadAsync(mapPath);
        Assert.NotNull(loaded.Value);
        var vm = new UiVm(null, () => true);
        var replace = vm.MapSession.ReplaceCurrentMap(
            MapDocumentAggregateBridge.ToAggregate(loaded.Value!), markSaved: true, mapPath);
        Assert.True(replace.IsSuccess);
        return (vm, mapPath);
    }

    [Fact]
    public async Task Scene_save_carries_map_reference()
    {
        var (vm, _) = await VmWithSavedMapAsync("sceneRefMap.xymap");
        var scenePath = TempPath("sceneRef.xyscene");

        await vm.SaveSceneAsync(scenePath);

        var text = File.ReadAllText(scenePath);
        Assert.Contains("mapReference", text);
        Assert.Contains(vm.MapIdText, text);
        Assert.DoesNotContain("gentleHillsV1\":{\"baseHeightMeters\":0", text);
    }

    [Fact]
    public async Task Scene_open_restores_map_reference()
    {
        var (vm, _) = await VmWithSavedMapAsync("sceneRefMap2.xymap");
        var scenePath = TempPath("sceneRef2.xyscene");
        await vm.SaveSceneAsync(scenePath);

        var reopened = new UiVm(null, () => true);
        await reopened.OpenSceneAsync(scenePath);

        Assert.True(reopened.HasMap);
        Assert.Equal(vm.MapIdText, reopened.MapIdText);
        Assert.Equal("已保存", reopened.MapStatusText);
    }

    [Fact]
    public async Task Scene_without_map_reference_opens_with_default_map()
    {
        var vm = new UiVm(null, () => true);
        var scenePath = TempPath("noRef.xyscene");
        await vm.SaveSceneAsync(scenePath);

        var reopened = new UiVm(null, () => true);
        await reopened.OpenSceneAsync(scenePath);

        // D3 会话语义：编辑器恒有当前地图（默认 10 km），非 R1 的"未加载"空状态。
        Assert.True(reopened.HasMap);
        Assert.Equal("未保存", reopened.MapStatusText);
        Assert.Equal(10000.0, reopened.MapSession.CurrentMap.SizeMeters.Width);
    }

    [Fact]
    public async Task Missing_map_file_marks_reference_invalid_without_crashing()
    {
        var (vm, mapPath) = await VmWithSavedMapAsync("willBeDeleted.xymap");
        var scenePath = TempPath("brokenRef.xyscene");
        await vm.SaveSceneAsync(scenePath);
        File.Delete(mapPath);

        var reopened = new UiVm(null, () => true);
        await reopened.OpenSceneAsync(scenePath);

        Assert.Contains("引用失效", reopened.FooterMessage);
        Assert.NotNull(reopened.MapReferenceError);
        // D3：引用失效时错误提示 + 会话默认地图保持（不伪造引用地图、不崩溃）。
        Assert.Equal("未命名地图", reopened.MapName);
        Assert.True(reopened.MapWorld.HasMap);
    }
}

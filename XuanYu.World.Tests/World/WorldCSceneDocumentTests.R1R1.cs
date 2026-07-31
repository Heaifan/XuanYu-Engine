using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.World;

public sealed partial class WorldCSceneDocumentTests
{
    [Fact]
    public async Task Repository_sample_opens_through_production_ui_chain()
    {
        var path = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..",
            "samples", "world-c-r1-ten-triangles.xyscene"));
        Assert.True(File.Exists(path));
        var vm = new UiVm(null, () => true, seedInitialScene: false);

        var opened = await vm.OpenSceneAsync(path);

        Assert.True(opened);
        Assert.False(vm.IsSceneDirty);
        Assert.Equal(10, vm.RenderSnapshot.Entities.Count);
        Assert.Equal(10, vm.HierarchyItems.Count(x => x.Key.StartsWith("EntityId(", StringComparison.Ordinal)));
        Assert.Contains(vm.RenderSnapshot.Entities, x => x.Name == "测试实体 10");
        Assert.Contains(vm.LogItems, x => x.Message == "场景加载成功" && x.Detail.Contains("Entities=10", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Failed_load_preserves_non_empty_world_path_dirty_selection_and_history()
    {
        var good = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.xyscene");
        var bad = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.xyscene");
        await File.WriteAllTextAsync(bad, "{ broken");
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        await vm.OpenSceneAsync(await CreateOneEntityScene(good));
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(n => n.Key == "EntityId(1)");
        Assert.True(vm.TryCommitInspectorTransformValue("位置", "X", "2"));
        var pathBefore = vm.CurrentScenePath;

        var opened = await vm.OpenSceneAsync(bad);

        Assert.False(opened);
        Assert.Single(vm.RenderSnapshot.Entities);
        Assert.Equal(pathBefore, vm.CurrentScenePath);
        Assert.True(vm.IsSceneDirty);
        Assert.Equal("EntityId(1)", vm.SelectionKey);
        Assert.Equal(1, vm.TransformHistoryCount);
    }
}

using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.World;

public sealed partial class WorldCSceneDocumentTests
{
    [Fact]
    public async Task Save_as_success_updates_status_title_and_log()
    {
        var source = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.xyscene");
        var target = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.xyscene");
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        await vm.OpenSceneAsync(await CreateOneEntityScene(source));

        Assert.True(await vm.SaveSceneAsync(target, saveAs: true));

        Assert.Equal(target, vm.CurrentScenePath);
        Assert.False(vm.IsSceneDirty);
        Assert.Equal("状态：另存为成功", vm.DocumentStatusText);
        Assert.Contains(Path.GetFileName(target), vm.DocumentTitle);
        Assert.Contains(vm.LogItems, x => x.Message == $"场景另存为成功：{Path.GetFileName(target)}" &&
            x.Detail == $"Path={target}");
    }

    [Fact]
    public async Task Dirty_scene_uses_chinese_unsaved_status_and_save_highlight()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.xyscene");
        var vm = await OpenDirtyScene(path);

        Assert.True(vm.IsSceneDirty);
        Assert.Contains("（未保存）", vm.DocumentTitle);
        Assert.Equal("状态：未保存", vm.DocumentStatusText);
        Assert.True(vm.IsSaveButtonHighlighted);
    }

    [Fact]
    public async Task Normal_save_clears_dirty_and_writes_success_log()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.xyscene");
        var vm = await OpenDirtyScene(path);

        Assert.True(await vm.SaveSceneAsync());

        Assert.False(vm.IsSceneDirty);
        Assert.Equal("状态：保存成功", vm.DocumentStatusText);
        Assert.False(vm.DocumentTitle.Contains("（未保存）", StringComparison.Ordinal));
        Assert.Contains(vm.LogItems, x => x.Message == $"场景保存成功：{Path.GetFileName(path)}" &&
            x.Detail == $"Path={path}");
    }

    [Fact]
    public async Task Undo_to_save_point_is_clean_and_redo_is_dirty()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.xyscene");
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        await vm.OpenSceneAsync(await CreateOneEntityScene(path));
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(n => n.Key == "EntityId(1)");

        Assert.True(vm.TryCommitInspectorTransformValue("位置", "X", "2"));
        vm.TryUndoFromShortcut();
        Assert.False(vm.IsSceneDirty);
        Assert.Equal("状态：就绪", vm.DocumentStatusText);

        vm.TryRedoFromShortcut();
        Assert.True(vm.IsSceneDirty);
        Assert.Equal("状态：未保存", vm.DocumentStatusText);
    }

    [Fact]
    public async Task Save_failure_preserves_dirty_and_reports_same_error_code()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.xyscene");
        var vm = await OpenDirtyScene(path);
        var badTarget = Path.GetTempPath();

        Assert.False(await vm.SaveSceneAsync(badTarget));

        Assert.True(vm.IsSceneDirty);
        Assert.Equal("状态：保存失败", vm.DocumentStatusText);
        Assert.True(vm.IsSaveButtonHighlighted);
        Assert.Contains("（未保存）", vm.DocumentTitle);
        Assert.Contains(vm.LogItems, x => x.Message == "场景保存失败" &&
            x.Detail.Contains("Code=InvalidScenePath", StringComparison.Ordinal));
    }

    static async Task<UiVm> OpenDirtyScene(string path)
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        await vm.OpenSceneAsync(await CreateOneEntityScene(path));
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(n => n.Key == "EntityId(1)");
        Assert.True(vm.TryCommitInspectorTransformValue("位置", "X", "2"));
        return vm;
    }
}

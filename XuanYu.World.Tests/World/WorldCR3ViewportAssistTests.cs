using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.World;

public sealed class WorldCR3ViewportAssistTests
{
    [Fact]
    public void Assist_toggles_do_not_dirty_history_selection_or_tool()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var tool = vm.ActiveTool;
        var selection = vm.SelectionKey;

        Assert.False(vm.ShowWorldAxes);
        vm.RunCommand.Execute("显示构造网格");
        vm.RunCommand.Execute("显示世界原点");
        vm.RunCommand.Execute("显示世界坐标轴");
        vm.RunCommand.Execute("显示编辑器背景");

        Assert.False(vm.ShowGrid);
        Assert.False(vm.ShowOrigin);
        Assert.True(vm.ShowWorldAxes);
        Assert.False(vm.ShowEditorBackground);
        Assert.False(vm.IsSceneDirty);
        Assert.Equal(0, vm.TransformHistoryCount);
        Assert.Equal(selection, vm.SelectionKey);
        Assert.Equal(tool, vm.ActiveTool);

        vm.RunCommand.Execute("显示世界坐标轴");
        Assert.False(vm.ShowWorldAxes);
        Assert.Equal(selection, vm.SelectionKey);
        Assert.Equal(tool, vm.ActiveTool);
    }

    [Fact]
    public async Task Assist_toggles_are_not_serialized_to_scene_file()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.xyscene");
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.RunCommand.Execute("显示构造网格");
        vm.RunCommand.Execute("显示世界原点");
        vm.RunCommand.Execute("显示世界坐标轴");
        vm.RunCommand.Execute("显示编辑器背景");

        Assert.True(await vm.SaveSceneAsync(path, saveAs: true));
        var json = await File.ReadAllTextAsync(path);

        Assert.DoesNotContain("Grid", json);
        Assert.DoesNotContain("WorldOrigin", json);
        Assert.DoesNotContain("WorldAxes", json);
        Assert.DoesNotContain("EditorBackground", json);
        Assert.DoesNotContain("ViewportCamera", json);
    }
}

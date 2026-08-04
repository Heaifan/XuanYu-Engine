using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.World;

public sealed class CommandSmokeTests
{
    [Fact]
    public void Top_file_commands_still_raise_file_requests()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var requests = new List<string>();
        vm.FileCommandRequested += requests.Add;

        foreach (var name in new[] { "新建", "打开", "保存", "另存为" })
            vm.RunCommand.Execute(name);

        Assert.Equal(["新建", "打开", "保存", "另存为"], requests);
        Assert.False(vm.IsSceneDirty);
        Assert.Equal(0, vm.TransformHistoryCount);
    }

    [Fact]
    public void Toolbar_and_environment_commands_remain_callable_without_dirtying_scene()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.SelectToolCommand.Execute("选择");
        vm.SelectToolCommand.Execute("移动");
        vm.SelectToolCommand.Execute("旋转");
        vm.SelectToolCommand.Execute("缩放");
        vm.ToggleSnapCommand.Execute(null);
        vm.RunCommand.Execute("运行");
        vm.RunCommand.Execute("停止");
        vm.RunCommand.Execute("显示构造网格");
        vm.RunCommand.Execute("显示世界原点");
        vm.RunCommand.Execute("显示世界坐标轴");
        vm.RunCommand.Execute("显示编辑器背景");

        Assert.Equal("缩放", vm.ActiveTool);
        Assert.True(vm.IsSnapEnabled);
        Assert.True(vm.ShowWorldAxes);
        Assert.False(vm.IsSceneDirty);
        Assert.Equal(0, vm.TransformHistoryCount);
    }
}

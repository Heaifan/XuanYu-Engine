using System.Collections.Generic;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D5：危险操作确认流——注入确认处理器后删除图层先请求确认；
// 确认后执行；未注入保持原行为（兼容既有测试）。
public sealed class UiD5DangerFlowTests
{
    static UiVm NewVm() => new(null, () => true);

    [Fact]
    public void Delete_layer_without_handler_executes_directly()
    {
        var vm = NewVm();
        vm.RunCommand.Execute("添加图层");
        var count = vm.LayerItems.Count;
        vm.RunCommand.Execute("删除图层");
        Assert.Equal(count - 1, vm.LayerItems.Count); // 无处理器：直接执行（兼容既有测试）
        Assert.False(vm.IsDangerousCommandPending("删除图层"));
    }

    [Fact]
    public void Delete_layer_with_handler_requests_confirmation_first()
    {
        var vm = NewVm();
        vm.RunCommand.Execute("添加图层");
        var count = vm.LayerItems.Count;
        var requested = new List<string>();
        vm.DangerousCommandConfirmRequested += requested.Add;

        vm.RunCommand.Execute("删除图层");
        // 危险操作先请求确认，图层尚未删除
        Assert.Equal(["删除图层"], requested);
        Assert.True(vm.IsDangerousCommandPending("删除图层"));
        Assert.Equal(count, vm.LayerItems.Count);
    }

    [Fact]
    public void Confirm_dangerous_command_executes_pending_delete()
    {
        var vm = NewVm();
        vm.RunCommand.Execute("添加图层");
        var count = vm.LayerItems.Count;
        vm.DangerousCommandConfirmRequested += _ => { };
        vm.RunCommand.Execute("删除图层");
        vm.ConfirmDangerousCommand("删除图层");
        Assert.Equal(count - 1, vm.LayerItems.Count);
        Assert.False(vm.IsDangerousCommandPending("删除图层"));
    }

    [Fact]
    public void Confirm_without_pending_is_ignored()
    {
        var vm = NewVm();
        vm.RunCommand.Execute("添加图层");
        var count = vm.LayerItems.Count;
        vm.ConfirmDangerousCommand("删除图层"); // 无待确认请求：忽略
        Assert.Equal(count, vm.LayerItems.Count);
    }

    [Fact]
    public void New_map_confirmation_flow_is_registered_in_window_layer()
    {
        // UiWin 层接线断言：新建地图走 ShowDanger 确认；删除图层文案含不可撤销说明
        var scene = System.IO.File.ReadAllText(System.IO.Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..",
            "XuanYu.Editor.UI", "Win", "UiWin.SceneCommands.cs"));
        Assert.Contains("ShowDanger(\"新建地图\"", scene);
        Assert.Contains("新建地图将替换当前地图属性并清空地图修改历史", scene);
        var win = System.IO.File.ReadAllText(System.IO.Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..",
            "XuanYu.Editor.UI", "Win", "UiWin.DialogHost.cs"));
        Assert.Contains("OnDangerousCommandRequested", win);
        Assert.Contains("ShowDanger(\"危险操作\"", win);
    }
}

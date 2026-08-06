using System.Collections.Generic;
using System.Linq;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D5：危险操作确认流——注入确认处理器后删除图层先请求确认；
// 确认后执行；未注入保持原行为（兼容既有测试）。
public sealed class UiD5DangerFlowTests
{
    static UiVm NewVm() => new(null, () => true);

    static string Logs(UiVm vm) => string.Join("\n", vm.LogItems.Select(e => e.Message));

    [Fact]
    public void Delete_layer_without_handler_is_blocked_fail_closed()
    {
        var vm = NewVm();
        vm.RunCommand.Execute("添加图层");
        var count = vm.LayerItems.Count;
        vm.RunCommand.Execute("删除图层");
        // D5 纠偏（fail-closed）：确认处理器缺失 → 不执行 + 记录错误
        Assert.Equal(count, vm.LayerItems.Count);
        Assert.False(vm.IsDangerousCommandPending("删除图层"));
        Assert.Contains("缺少确认处理器", Logs(vm));
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
        // UiWin 层接线断言：新建地图走未保存三选流程（保存并新建/不保存并新建/取消）；危险按钮写具体动作
        var scene = System.IO.File.ReadAllText(System.IO.Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..",
            "XuanYu.Editor.UI", "Win", "UiWin.SceneCommands.cs"));
        Assert.Contains("ShowUnsavedMapChangesDialog", scene);
        Assert.Contains("HasUnsavedMapChanges", scene);
        var unsaved = System.IO.File.ReadAllText(System.IO.Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..",
            "XuanYu.Editor.UI", "Win", "UiWin.UnsavedDialog.cs"));
        Assert.Contains("保存并新建", unsaved);
        Assert.Contains("不保存并新建", unsaved);
        var win = System.IO.File.ReadAllText(System.IO.Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..",
            "XuanYu.Editor.UI", "Win", "UiWin.DialogHost.Danger.cs"));
        Assert.Contains("OnDangerousCommandRequested", win);
        Assert.Contains("删除图层将移除该图层及其中的对象", win);
        Assert.Contains("CancelDangerousCommand", win);
    }
}

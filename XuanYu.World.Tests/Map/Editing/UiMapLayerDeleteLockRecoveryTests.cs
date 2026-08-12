using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Map.Editing;

public sealed class UiMapLayerDeleteLockRecoveryTests
{
    [Fact]
    public void Cancel_delete_restores_command_surface()
    {
        var vm = NewVm();
        vm.RunCommand.Execute("添加图层");
        vm.DangerousCommandConfirmRequested += vm.CancelDangerousCommand;

        vm.RunCommand.Execute("删除图层");

        Assert.False(vm.IsDangerousCommandPending("删除图层"));
        Assert.Equal(4, vm.LayerItems.Count);
        vm.RunCommand.Execute("添加图层");
        Assert.Equal(5, vm.LayerItems.Count);
    }

    [Fact]
    public void Confirm_delete_updates_selection_and_allows_follow_up_edit()
    {
        var vm = NewVm();
        vm.RunCommand.Execute("添加图层");
        var deleted = vm.SelectedLayer!.LayerId;
        vm.DangerousCommandConfirmRequested += vm.ConfirmDangerousCommand;

        vm.RunCommand.Execute("删除图层");

        Assert.DoesNotContain(vm.LayerItems, row => row.LayerId == deleted);
        Assert.Null(vm.SelectedLayer);
        vm.RunCommand.Execute("添加图层");
        Assert.Equal(4, vm.LayerItems.Count);
    }

    [Fact]
    public void Rejected_delete_clears_pending_and_allows_follow_up_edit()
    {
        var vm = NewVm();
        vm.SelectedLayer = vm.LayerItems[0];
        vm.DangerousCommandConfirmRequested += vm.ConfirmDangerousCommand;

        vm.RunCommand.Execute("删除图层");

        Assert.False(vm.IsDangerousCommandPending("删除图层"));
        Assert.Contains("至少保留一个区域图层", vm.MapEditError);
        vm.RunCommand.Execute("添加图层");
        Assert.Equal(4, vm.LayerItems.Count);
    }

    static UiVm NewVm() => new(null, () => true);
}

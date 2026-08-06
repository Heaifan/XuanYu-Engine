using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map.Editing;

// MAP-A-R2-D4：图层面板行为——显隐/锁定/删除/排序/活动图层/撤销重做（真实命令链）。
public sealed partial class UiMapLayerPanelTests
{
    [Fact]
    public void Visibility_toggle_updates_session_and_logs_chinese()
    {
        var vm = NewVm();
        vm.SelectedLayer = vm.LayerItems[1];
        vm.LayerInspectorVisible = false;
        var boundary = vm.MapSession.CurrentMap.Layers.First(l => l.Kind == MapLayerKind.Boundary);
        Assert.False(boundary.IsVisible);
        Assert.Contains("图层可见性：边界=隐藏", Logs(vm));
        vm.LayerInspectorVisible = true;
        Assert.True(vm.MapSession.CurrentMap.Layers.First(l => l.Kind == MapLayerKind.Boundary).IsVisible);
        Assert.Contains("图层可见性：边界=显示", Logs(vm));
    }
    [Fact]
    public void Lock_toggle_updates_session_and_logs_chinese()
    {
        var vm = NewVm();
        vm.SelectedLayer = vm.LayerItems[0];
        vm.LayerInspectorLocked = true;
        var region = vm.MapSession.CurrentMap.Layers.First(l => l.Kind == MapLayerKind.Region);
        Assert.True(region.IsLocked);
        Assert.Contains("锁定图层：区域 1（区域）", Logs(vm));
        vm.LayerInspectorLocked = false;
        Assert.False(vm.MapSession.CurrentMap.Layers.First(l => l.Kind == MapLayerKind.Region).IsLocked);
        Assert.Contains("解锁图层：区域 1（区域）", Logs(vm));
    }
    [Fact]
    public void Delete_last_region_layer_rejected_with_chinese_error()
    {
        var vm = NewVm();
        vm.DangerousCommandConfirmRequested += vm.ConfirmDangerousCommand; // 测试批准确认服务
        vm.SelectedLayer = vm.LayerItems[0];
        vm.RunCommand.Execute("删除图层");
        Assert.Equal(3, vm.LayerItems.Count);
        Assert.Contains("至少保留一个区域图层", vm.MapEditError);
        Assert.Contains("图层删除失败：至少保留一个区域图层", Logs(vm));
    }
    [Fact]
    public void Delete_region_layer_transfers_active_and_logs()
    {
        var vm = NewVm();
        vm.DangerousCommandConfirmRequested += vm.ConfirmDangerousCommand; // 测试批准确认服务
        vm.RunCommand.Execute("添加图层");
        vm.RunCommand.Execute("删除图层");
        Assert.Equal(3, vm.LayerItems.Count);
        Assert.Equal("区域 1", vm.LayerItems[0].Name);
        Assert.Contains("删除图层：区域 2", Logs(vm));
    }
    [Fact]
    public void Move_up_down_command_chain_reorders_region_layers()
    {
        var vm = NewVm();
        vm.RunCommand.Execute("添加图层");
        vm.RunCommand.Execute("添加图层");
        Assert.Equal("区域 3", vm.LayerItems[0].Name);
        vm.RunCommand.Execute("下移图层");
        Assert.Equal("区域 2", vm.LayerItems[0].Name);
        Assert.Equal("区域 3", vm.LayerItems[1].Name);
        Assert.Contains("调整图层顺序：区域 3，下移", Logs(vm));
        vm.RunCommand.Execute("上移图层");
        Assert.Equal("区域 3", vm.LayerItems[0].Name);
        Assert.Contains("调整图层顺序：区域 3，上移", Logs(vm));
    }
    [Fact]
    public void Set_active_layer_command_chain_marks_active()
    {
        var vm = NewVm();
        vm.RunCommand.Execute("添加图层");
        vm.SelectedLayer = vm.LayerItems[1];
        vm.RunCommand.Execute("设为当前图层");
        Assert.Equal(vm.LayerItems[1].LayerId, vm.MapSession.ActiveRegionLayerId);
        Assert.True(vm.LayerItems[1].IsActive);
        Assert.False(vm.LayerItems[0].IsActive);
        Assert.Contains("设置当前图层：区域 1", Logs(vm));
    }
    [Fact]
    public void Undo_redo_via_top_toolbar_restores_layer_state()
    {
        var vm = NewVm();
        vm.RunCommand.Execute("添加图层");
        vm.SelectedLayer = vm.LayerItems[0];
        vm.LayerInspectorNameText = "主战区";
        vm.CommitLayerRename(vm.LayerInspectorNameText);
        vm.RunCommand.Execute("撤销地图修改");
        Assert.Equal("区域 2", vm.LayerItems[0].Name);
        vm.RunCommand.Execute("重做地图修改");
        Assert.Equal("主战区", vm.LayerItems[0].Name);
        Assert.Equal(4, vm.LayerItems.Count);
    }
}

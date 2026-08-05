using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map.Editing;

// MAP-A-R2-D4：图层面板 ViewModel——默认列表/添加/按钮状态/系统层只读/重命名（真实命令链）。
public sealed partial class UiMapLayerPanelTests
{
    static UiVm NewVm() => new(null, () => true);

    static string Logs(UiVm vm) => string.Join("\n", vm.LogItems.Select(e => e.Message));

    [Fact]
    public void Default_list_shows_three_layers_region_first()
    {
        var vm = NewVm();
        Assert.Equal(3, vm.LayerItems.Count);
        Assert.Equal("区域 1", vm.LayerItems[0].Name);
        Assert.Equal("边界", vm.LayerItems[1].Name);
        Assert.Equal("地面", vm.LayerItems[2].Name);
        Assert.True(vm.LayerItems[0].IsActive);
        Assert.False(vm.LayerItems[1].IsActive);
        Assert.True(vm.LayerItems[0].IsVisible);
    }

    [Fact]
    public void Add_layer_command_chain_adds_and_selects_new_layer()
    {
        var vm = NewVm();
        vm.RunCommand.Execute("添加图层");
        Assert.Equal(4, vm.LayerItems.Count);
        var added = vm.LayerItems[0];
        Assert.Equal("区域 2", added.Name);
        Assert.True(added.IsActive);
        Assert.Equal(added.LayerId, vm.SelectedLayer?.LayerId);
        Assert.Contains("添加图层：名称=区域 2", Logs(vm));
    }

    [Fact]
    public void Toolbar_enabled_states_follow_selection()
    {
        var vm = NewVm();
        Assert.False(vm.CanMoveLayerUp);
        Assert.False(vm.CanMoveLayerDown);
        Assert.False(vm.CanDeleteLayer);
        vm.SelectedLayer = vm.LayerItems[0];
        Assert.False(vm.CanMoveLayerUp);
        Assert.False(vm.CanMoveLayerDown);
        Assert.False(vm.CanDeleteLayer);
        vm.RunCommand.Execute("添加图层");
        var top = vm.LayerItems[0];
        Assert.True(vm.CanDeleteLayer);
        Assert.False(vm.CanMoveLayerUp);
        Assert.True(vm.CanMoveLayerDown);
        vm.SelectedLayer = vm.LayerItems[1];
        Assert.True(vm.CanMoveLayerUp);
        Assert.False(vm.CanMoveLayerDown); // 区域 1 是最下方区域图层，下移禁用
    }

    [Fact]
    public void System_layers_are_readonly_and_not_deletable()
    {
        var vm = NewVm();
        vm.SelectedLayer = vm.LayerItems[1];
        Assert.True(vm.LayerInspectorIsSystem);
        Assert.False(vm.CanDeleteLayer);
        Assert.False(vm.CanMoveLayerUp);
        vm.SelectedLayer = vm.LayerItems[2];
        Assert.True(vm.LayerInspectorIsSystem);
        Assert.False(vm.CanDeleteLayer);
    }

    [Fact]
    public void Rename_commits_via_inspector_text()
    {
        var vm = NewVm();
        vm.SelectedLayer = vm.LayerItems[0];
        vm.LayerInspectorNameText = "主战区";
        vm.CommitLayerRename(vm.LayerInspectorNameText);
        Assert.Equal("主战区", vm.LayerItems[0].Name);
        Assert.Contains("重命名图层：区域 1 → 主战区", Logs(vm));
    }

    [Fact]
    public void Invalid_name_shows_error_and_keeps_state()
    {
        var vm = NewVm();
        vm.SelectedLayer = vm.LayerItems[0];
        vm.CommitLayerRename("   ");
        Assert.Contains("图层名称不能为空", vm.MapEditError);
        Assert.Equal("区域 1", vm.LayerItems[0].Name);
        Assert.Contains("图层重命名失败：图层名称不能为空", Logs(vm));
    }
}

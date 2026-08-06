using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map.Editing;

// MAP-A-R2-D4-F3：区域图层拖动 UI 入口（U01～U06 + L03 通知）。
public sealed class UiMapLayerDragTests
{
    static UiVm NewVm() => new(null, () => true);

    static string Messages(UiVm vm) => string.Join("\n", vm.LogItems.Select(e => e.Message));

    [Fact]
    public void U01_region_layer_has_drag_capability()
    {
        var vm = NewVm();
        Assert.True(vm.LayerItems[0].IsRegion);
        Assert.True(vm.LayerItems[0].IsVisible);
        Assert.False(vm.CanReorderLayers);
        Assert.False(vm.LayerItems[0].IsDragEnabled);
        Assert.Contains("至少需要两个用户图层", vm.LayerItems[0].DragHandleToolTip);
        vm.RunCommand.Execute("添加图层");
        Assert.True(vm.CanReorderLayers);
        Assert.True(vm.LayerItems[0].IsDragEnabled);
    }

    [Fact]
    public void U02_system_layers_are_not_draggable()
    {
        var vm = NewVm();
        Assert.False(vm.LayerItems[1].IsRegion); // 边界
        Assert.False(vm.LayerItems[2].IsRegion); // 地面
    }

    [Fact]
    public void U03_drop_commits_single_session_command()
    {
        var vm = NewVm();
        vm.RunCommand.Execute("添加图层");
        vm.RunCommand.Execute("添加图层");
        var region = vm.LayerItems[1];
        var stateId = vm.MapSession.CurrentStateId;
        vm.CommitLayerDrag(region.LayerId.Value, 0);
        Assert.Equal(stateId + 1, vm.MapSession.CurrentStateId);
        Assert.Equal(region.LayerId, vm.LayerItems[0].LayerId);
        Assert.Single(vm.LogItems, e => e.Message.StartsWith("调整图层顺序："));
    }

    [Fact]
    public void U04_invalid_target_does_not_commit()
    {
        var vm = NewVm();
        var stateId = vm.MapSession.CurrentStateId;
        vm.CommitLayerDrag(vm.LayerItems[1].LayerId.Value, 0); // 系统层
        vm.CommitLayerDrag("00000000000000000000000000000000", 0); // 未知
        Assert.Equal(stateId, vm.MapSession.CurrentStateId);
        Assert.False(vm.MapSession.CanUndo);
    }

    [Fact]
    public void U05_up_down_buttons_still_available()
    {
        var vm = NewVm();
        vm.RunCommand.Execute("添加图层");
        vm.RunCommand.Execute("添加图层");
        vm.SelectedLayer = vm.LayerItems[1]; // 区域 2（中间层）
        Assert.True(vm.CanMoveLayerUp);
        Assert.True(vm.CanMoveLayerDown);
        vm.RunCommand.Execute("上移图层");
        Assert.Equal("区域 2", vm.LayerItems[0].Name);
        vm.RunCommand.Execute("下移图层");
        Assert.Equal("区域 3", vm.LayerItems[0].Name);
    }

    [Fact]
    public void U06_drag_log_appears_once_with_position()
    {
        var vm = NewVm();
        vm.RunCommand.Execute("添加图层");
        vm.RunCommand.Execute("添加图层");
        vm.CommitLayerDrag(vm.LayerItems[1].LayerId.Value, 0);
        var text = Messages(vm);
        Assert.Contains("调整图层顺序：区域 2 → 第 1 位", text);
        Assert.Single(vm.LogItems, e => e.Message.StartsWith("调整图层顺序："));
    }

    [Fact]
    public void L03_drag_notice_kept_in_summary()
    {
        var vm = NewVm();
        vm.RunCommand.Execute("添加图层");
        vm.RunCommand.Execute("添加图层");
        vm.CommitLayerDrag(vm.LayerItems[1].LayerId.Value, 0);
        Assert.Contains("调整图层顺序", vm.LogSummary);
    }
}

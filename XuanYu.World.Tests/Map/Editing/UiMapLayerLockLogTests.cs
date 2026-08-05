using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map.Editing;

// MAP-A-R2-D4-F2：图层锁定日志细化（L01～L06）+ 添加立方体单次创建（C01）。
// 消息列：锁定/解锁图层：名称（类型）；详情列：LayerId + 状态变化。
public sealed class UiMapLayerLockLogTests
{
    static UiVm NewVm() => new(null, () => true);

    static string Messages(UiVm vm) => string.Join("\n", vm.LogItems.Select(e => e.Message));

    static string Details(UiVm vm) => string.Join("\n", vm.LogItems.Select(e => e.Detail));

    [Fact]
    public void L01_lock_region_layer_logs_chinese_action()
    {
        var vm = NewVm();
        vm.SelectedLayer = vm.LayerItems[0];
        vm.LayerInspectorLocked = true;
        Assert.Contains("锁定图层：区域 1（区域）", Messages(vm));
    }

    [Fact]
    public void L02_unlock_region_layer_logs_chinese_action()
    {
        var vm = NewVm();
        vm.SelectedLayer = vm.LayerItems[0];
        vm.LayerInspectorLocked = true;
        vm.LayerInspectorLocked = false;
        Assert.Contains("解锁图层：区域 1（区域）", Messages(vm));
    }

    [Fact]
    public void L03_system_layer_lock_logs_system_kind()
    {
        var vm = NewVm();
        vm.SelectedLayer = vm.LayerItems[1];
        vm.LayerInspectorLocked = true;
        Assert.Contains("锁定图层：边界（系统）", Messages(vm));
    }

    [Fact]
    public void L04_detail_contains_layer_id_and_state_change()
    {
        var vm = NewVm();
        vm.SelectedLayer = vm.LayerItems[1];
        vm.LayerInspectorLocked = true;
        var boundary = vm.MapSession.CurrentMap.Layers.First(l => l.Kind == MapLayerKind.Boundary);
        var detail = Details(vm);
        Assert.Contains($"LayerId={boundary.LayerId.Value}", detail);
        Assert.Contains("状态：未锁定 → 已锁定", detail);
    }

    [Fact]
    public void L05_same_value_noop_adds_no_log()
    {
        var vm = NewVm();
        vm.SelectedLayer = vm.LayerItems[0];
        vm.LayerInspectorLocked = true;
        var count = vm.LogItems.Count;
        vm.LayerInspectorLocked = true; // 同值 No-op
        Assert.Equal(count, vm.LogItems.Count);
    }

    [Fact]
    public void L06_one_click_produces_single_layer_action_log()
    {
        var vm = NewVm();
        vm.SelectedLayer = vm.LayerItems[0];
        vm.LayerInspectorLocked = true;
        Assert.Single(vm.LogItems, e => e.Message.Contains("锁定图层"));
    }

    [Fact]
    public void C01_add_cube_command_creates_single_entity()
    {
        var vm = NewVm();
        var before = vm.RenderSnapshot.Entities.Count;
        vm.RunCommand.Execute("添加立方体");
        Assert.Equal(before + 1, vm.RenderSnapshot.Entities.Count);
        vm.RunCommand.Execute("添加立方体");
        Assert.Equal(before + 2, vm.RenderSnapshot.Entities.Count);
    }
}

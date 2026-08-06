using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.World;

// MAP-A-R2-D3：地图属性入口——会话恒有默认地图、应用修改、非法输入保护、取景数据源。
public sealed class UiMapEditorTests
{
    [Fact]
    public void Default_session_has_default_map()
    {
        var vm = new UiVm(null, () => true);
        Assert.True(vm.HasMap);
        Assert.Equal("未命名地图", vm.MapName);
        Assert.Equal("已保存", vm.MapStatusText); // D5 二次纠偏：默认地图建立基线保存点（初始不误判未保存）
        Assert.Equal(10000.0, vm.MapSession.CurrentMap.SizeMeters.Width);
        Assert.Equal(10000.0, vm.MapSession.CurrentMap.SizeMeters.Depth);
        Assert.Equal(0.0, vm.MapSession.CurrentMap.Surface.BaseHeightMeters);
        Assert.True(vm.MapWorld.HasMap);
        Assert.Equal(10000.0, vm.MapWorld.CurrentMap!.WidthMeters);
    }
    [Fact]
    public void New_map_resets_session_and_world()
    {
        var vm = new UiVm(null, () => true);
        vm.MapWidthText = "20000"; vm.MapDepthText = "8000";
        vm.ApplyMapProperties();
        vm.NewMap();
        Assert.Equal("未命名地图", vm.MapName);
        Assert.Equal("10000", vm.MapWidthText);
        Assert.Equal(10000.0, vm.MapWorld.CurrentMap!.WidthMeters);
    }
    [Fact]
    public void Apply_properties_updates_session_world_and_dirty()
    {
        var vm = new UiVm(null, () => true);
        vm.MapWidthText = "20000"; vm.MapDepthText = "8000"; vm.MapBaseHeightText = "100";
        vm.ApplyMapProperties();
        Assert.Equal(20000.0, vm.MapSession.CurrentMap.SizeMeters.Width);
        Assert.Equal(8000.0, vm.MapSession.CurrentMap.SizeMeters.Depth);
        Assert.Equal(100.0, vm.MapSession.CurrentMap.Surface.BaseHeightMeters);
        Assert.Equal(20000.0, vm.MapWorld.CurrentMap!.WidthMeters);
        Assert.Equal(100.0, vm.MapWorld.CurrentMap!.BaseHeightMeters);
        Assert.Equal("", vm.MapEditError);
        Assert.Equal("未保存", vm.MapStatusText);
    }
    [Fact]
    public void Invalid_width_rejected_without_state_change()
    {
        var vm = new UiVm(null, () => true);
        vm.MapWidthText = "50"; vm.MapDepthText = "8000";
        vm.ApplyMapProperties();
        Assert.Contains("宽度", vm.MapEditError);
        Assert.Equal(10000.0, vm.MapSession.CurrentMap.SizeMeters.Width);
        Assert.Equal(10000.0, vm.MapSession.CurrentMap.SizeMeters.Depth);
        Assert.Equal(10000.0, vm.MapWorld.CurrentMap!.WidthMeters);
        Assert.False(vm.MapSession.CanUndo, "失败不得产生历史");
    }
    [Fact]
    public void Non_numeric_input_shows_chinese_error()
    {
        var vm = new UiVm(null, () => true);
        vm.MapBaseHeightText = "abc";
        vm.ApplyMapProperties();
        Assert.Equal("基础高度必须是有限数字。", vm.MapEditError);
        Assert.Equal(0.0, vm.MapSession.CurrentMap.Surface.BaseHeightMeters);
    }
    [Fact]
    public void Resize_too_small_rejected_as_whole()
    {
        var vm = new UiVm(null, () => true);
        vm.MapWidthText = "99"; vm.MapDepthText = "99";
        vm.ApplyMapProperties();
        Assert.NotEqual("", vm.MapEditError);
        Assert.Equal(10000.0, vm.MapSession.CurrentMap.SizeMeters.Width);
    }
    [Fact]
    public void Focus_map_reads_session_bounds()
    {
        var vm = new UiVm(null, () => true);
        vm.MapWidthText = "30000"; vm.MapDepthText = "30000";
        vm.ApplyMapProperties();
        vm.FocusMap();
        // 取景后相机 Far 随地图距离扩展（非固定 100），应能容纳 30 km 地图。
        Assert.True(vm.RenderProjection.Projection.Camera.FarPlane > 5000);
    }
}

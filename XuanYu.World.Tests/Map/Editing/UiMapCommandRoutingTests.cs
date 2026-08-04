using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.World;

// MAP-A-R2-D3-F1：真实按钮链测试（RunCommand.Execute → MapSession）。
public sealed class UiMapCommandRoutingTests
{
    static UiVm NewVm() => new(null, () => true);
    static void Execute(UiVm vm, string name) => vm.RunCommand.Execute(name);
    [Fact]
    public void Execute_apply_properties_reaches_map_session()
    {
        var vm = NewVm();
        vm.MapWidthText = "200"; vm.MapDepthText = "200"; vm.MapBaseHeightText = "0";
        Execute(vm, "应用地图属性");
        Assert.Equal(200.0, vm.MapSession.CurrentMap.SizeMeters.Width); Assert.Equal(200.0, vm.MapSession.CurrentMap.SizeMeters.Depth);
        Assert.Equal(0.0, vm.MapSession.CurrentMap.Surface.BaseHeightMeters); Assert.True(vm.MapSession.CanUndo);
        Assert.False(vm.MapSession.CanRedo);
        Assert.Equal(1, vm.MapSession.ChangeSequence); Assert.Equal(200.0, vm.RenderProjection.Projection.Map.WidthMeters); Assert.Equal(200.0, vm.RenderProjection.Projection.Map.DepthMeters); Assert.Equal("", vm.MapEditError);
    }
    [Fact]
    public void Execute_undo_restores_map_and_snapshot()
    {
        var vm = NewVm();
        vm.MapWidthText = "200"; vm.MapDepthText = "200"; vm.MapBaseHeightText = "0";
        Execute(vm, "应用地图属性");
        Execute(vm, "撤销地图修改");
        Assert.Equal(10000.0, vm.MapSession.CurrentMap.SizeMeters.Width); Assert.Equal(10000.0, vm.MapSession.CurrentMap.SizeMeters.Depth);
        Assert.False(vm.MapSession.CanUndo); Assert.True(vm.MapSession.CanRedo);
        Assert.Equal("10000", vm.MapWidthText); Assert.Equal("10000", vm.MapDepthText); Assert.Equal("0", vm.MapBaseHeightText);
        Assert.Equal(10000.0, vm.RenderProjection.Projection.Map.WidthMeters);
    }
    [Fact]
    public void Execute_redo_restores_applied_values()
    {
        var vm = NewVm();
        vm.MapWidthText = "200"; vm.MapDepthText = "200"; vm.MapBaseHeightText = "0";
        Execute(vm, "应用地图属性");
        Execute(vm, "撤销地图修改");
        Execute(vm, "重做地图修改");
        Assert.Equal(200.0, vm.MapSession.CurrentMap.SizeMeters.Width); Assert.Equal(200.0, vm.MapSession.CurrentMap.SizeMeters.Depth);
        Assert.True(vm.MapSession.CanUndo); Assert.False(vm.MapSession.CanRedo);
        Assert.Equal("200", vm.MapWidthText); Assert.Equal(200.0, vm.RenderProjection.Projection.Map.WidthMeters);
    }
    [Fact]
    public void Execute_invalid_size_rejects_with_zero_pollution()
    {
        var vm = NewVm();
        vm.MapWidthText = "50"; vm.MapDepthText = "200"; vm.MapBaseHeightText = "0";
        var sequence = vm.MapSession.ChangeSequence;
        Execute(vm, "应用地图属性");
        Assert.Equal(10000.0, vm.MapSession.CurrentMap.SizeMeters.Width);
        Assert.Equal(sequence, vm.MapSession.ChangeSequence); Assert.False(vm.MapSession.CanUndo);
        Assert.Equal(10000.0, vm.RenderProjection.Projection.Map.WidthMeters);
        Assert.Contains("宽度", vm.MapEditError);
    }
    [Fact]
    public void Execute_non_finite_values_rejected()
    {
        foreach (var text in new[] { "NaN", "Infinity", "-Infinity" })
        {
            var vm = NewVm();
            vm.MapWidthText = text; vm.MapDepthText = "200"; vm.MapBaseHeightText = "0";
            Execute(vm, "应用地图属性");
            Assert.Equal(10000.0, vm.MapSession.CurrentMap.SizeMeters.Width); Assert.False(vm.MapSession.CanUndo);
            Assert.Equal(0, vm.MapSession.ChangeSequence);
        }
    }
    [Fact]
    public void Execute_routes_new_and_focus_commands()
    {
        var vm = NewVm();
        Execute(vm, "新建地图");
        Assert.Equal(10000.0, vm.MapSession.CurrentMap.SizeMeters.Width);
        var positionBefore = vm.RenderProjection.Projection.Camera.Position;
        vm.FocusMap(); // 直接调用对照：取景链自身生效
        Assert.NotEqual(positionBefore, vm.RenderProjection.Projection.Camera.Position);
        var positionDirect = vm.RenderProjection.Projection.Camera.Position;
        Execute(vm, "聚焦地图"); // 命令路由再次聚焦：幂等（位置不变）
        Assert.Equal(positionDirect, vm.RenderProjection.Projection.Camera.Position);
    }
    [Fact]
    public void Execute_unknown_command_falls_back_without_breaking()
    {
        var vm = NewVm();
        Execute(vm, "完全不存在的命令");
        Assert.False(vm.MapSession.CanUndo); Assert.Contains("已执行：完全不存在的命令", vm.FooterMessage);
    }
    [Fact]
    public void Logs_contain_submit_and_snapshot_publish()
    {
        var vm = NewVm();
        vm.MapWidthText = "200"; vm.MapDepthText = "200"; vm.MapBaseHeightText = "0";
        Execute(vm, "应用地图属性");
        var text = string.Join("\n", vm.LogItems.Select(e => e.Message));
        Assert.Contains("地图命令收到", text); Assert.Contains("地图属性提交开始", text);
        Assert.Contains("地图属性提交成功", text); Assert.Contains("地图渲染快照已发布", text);
    }
}

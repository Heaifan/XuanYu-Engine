using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.World;

// MAP-A-R2-D3-A1 入口补接：地图撤销/重做按钮路由到 MapSession 独立历史。
public sealed class UiMapHistoryTests
{
    static UiVm VmWithAppliedProperties()
    {
        var vm = new UiVm(null, () => true);
        vm.MapWidthText = "20000"; vm.MapDepthText = "8000"; vm.MapBaseHeightText = "100";
        vm.ApplyMapProperties();
        return vm;
    }

    [Fact]
    public void Map_undo_restores_properties_and_syncs_texts()
    {
        var vm = VmWithAppliedProperties();
        Assert.True(vm.CanUndo);

        vm.MapUndo();

        Assert.Equal(10000.0, vm.MapSession.CurrentMap.SizeMeters.Width);
        Assert.Equal(10000.0, vm.MapSession.CurrentMap.SizeMeters.Depth);
        Assert.Equal(0.0, vm.MapSession.CurrentMap.Surface.BaseHeightMeters);
        Assert.Equal("10000", vm.MapWidthText);
        Assert.Equal("10000", vm.MapDepthText);
        Assert.Equal("0", vm.MapBaseHeightText);
        Assert.False(vm.CanUndo);
        Assert.True(vm.CanRedo);
        Assert.Equal(10000.0, vm.MapWorld.CurrentMap!.WidthMeters); // World 查询随会话恢复
    }

    [Fact]
    public void Map_redo_restores_properties_and_syncs_texts()
    {
        var vm = VmWithAppliedProperties();
        vm.MapUndo();
        Assert.True(vm.CanRedo);

        vm.MapRedo();

        Assert.Equal(20000.0, vm.MapSession.CurrentMap.SizeMeters.Width);
        Assert.Equal(8000.0, vm.MapSession.CurrentMap.SizeMeters.Depth);
        Assert.Equal(100.0, vm.MapSession.CurrentMap.Surface.BaseHeightMeters);
        Assert.Equal("20000", vm.MapWidthText);
        Assert.Equal("8000", vm.MapDepthText);
        Assert.Equal("100", vm.MapBaseHeightText);
        Assert.False(vm.CanRedo);
    }

    [Fact]
    public void Undo_without_history_fails_without_changes()
    {
        var vm = new UiVm(null, () => true);

        vm.MapUndo();

        Assert.False(vm.CanUndo);
        Assert.Equal(10000.0, vm.MapSession.CurrentMap.SizeMeters.Width);
        Assert.Contains("没有可撤销", vm.MapEditError); // 按钮禁用态下不可达；防御性调用明确报错
    }

    [Fact]
    public void Undo_drives_render_snapshot_via_content_changed()
    {
        var vm = VmWithAppliedProperties();
        var before = vm.RenderProjection.Projection.Map;

        vm.MapUndo();

        var after = vm.RenderProjection.Projection.Map;
        Assert.Equal(10000.0, after.WidthMeters);
        Assert.NotEqual(before.WidthMeters, after.WidthMeters);
        Assert.True(after.SourceChangeSequence > before.SourceChangeSequence);
    }
}

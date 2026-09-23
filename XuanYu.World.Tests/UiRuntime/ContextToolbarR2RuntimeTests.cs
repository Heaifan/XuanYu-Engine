using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class ContextToolbarR2RuntimeTests
{
    [Fact]
    public void Drawing_state_exposes_session_last_tool_and_transaction_projection()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.NotNull(typeof(UiVm).GetProperty("LastDrawTool"));
        Assert.NotNull(typeof(UiVm).GetProperty("IsDrawingTransactionActive"));
        Assert.NotNull(typeof(UiVm).GetProperty("DrawingTransactionLabel"));
    }

    [Fact]
    public void Initial_split_main_action_has_no_implicit_drawing_tool()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.Null(typeof(UiVm).GetProperty("LastDrawTool")?.GetValue(vm));
    }

    [Fact]
    public async Task Selecting_road_directly_enters_drawing_without_manual_mode_setup()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.ToggleEditorModeCommand.Execute(null);
        Assert.True(await vm.BeginContextDrawingAsync("道路"));
        Assert.True(vm.IsRoadDrawingTool);
        Assert.Equal("道路", vm.LastDrawTool);
        Assert.Equal("道路", vm.DrawButtonLabel);
        Assert.True(vm.IsDrawingTransactionActive);
    }

    [Fact]
    public async Task Selecting_region_updates_real_draw_label()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false); vm.ToggleEditorMode();
        Assert.True(await vm.BeginContextDrawingAsync("区域面"));
        Assert.Equal("区域面", vm.LastDrawTool); Assert.Equal("区域", vm.DrawButtonLabel);
    }

    [Fact]
    public async Task Active_transaction_rejects_second_tool_and_cancel_preserves_memory()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.ToggleEditorModeCommand.Execute(null);
        Assert.True(await vm.BeginContextDrawingAsync("道路"));
        Assert.False(await vm.BeginContextDrawingAsync("区域面"));
        Assert.True(vm.CancelRoadDrawing());
        Assert.False(vm.IsDrawingTransactionActive);
        Assert.Equal("道路", vm.LastDrawTool);
    }

    [Fact]
    public async Task Main_split_repeats_last_real_tool()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false); vm.ToggleEditorMode();
        Assert.True(await vm.BeginContextDrawingAsync("道路")); Assert.True(vm.CancelRoadDrawing());
        Assert.True(await vm.BeginLastDrawToolAsync()); Assert.True(vm.IsRoadDrawingTool); Assert.Equal("道路", vm.LastDrawTool);
    }

    [Fact]
    public async Task Selecting_marker_updates_real_draw_label()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false); vm.ToggleEditorMode();
        Assert.True(await vm.BeginContextDrawingAsync("地图标记"));
        Assert.Equal("地图标记", vm.LastDrawTool); Assert.Equal("点标记", vm.DrawButtonLabel);
    }

    [Fact]
    public void Draw_button_label_maps_real_domain_tools()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.Equal("绘制", vm.DrawButtonLabel);
    }
}

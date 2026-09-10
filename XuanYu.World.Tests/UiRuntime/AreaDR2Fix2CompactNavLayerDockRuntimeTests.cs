using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XYUI.Avalonia;
using XYUI.Avalonia.Controls;
using AvaloniaPath = Avalonia.Controls.Shapes.Path;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class AreaDR2Fix2CompactNavLayerDockRuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public AreaDR2Fix2CompactNavLayerDockRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Left_and_right_top_tabs_use_compact_runtime_height()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var left = new Left { DataContext = new UiVm(null, seedInitialScene: false) };
            var right = new EditorRightTabs { DataContext = new UiVm(null, seedInitialScene: false) };
            host.Show(left, 300, 720); left.UpdateLayout();
            host.Show(right, 300, 720); right.UpdateLayout();
            var leftTabs = UiRuntimeTestHost.Descendants<XYTabs>(left).Single();
            var rightTabs = UiRuntimeTestHost.Descendants<XYTabs>(right).Single();
            return (LeftHeight: leftTabs.Bounds.Height, RightHeight: rightTabs.Bounds.Height,
                LeftSize: XY.GetSize(leftTabs), RightDensity: XY.GetDensity(rightTabs));
        });
        Assert.InRange(state.LeftHeight, 20, 28); Assert.InRange(state.RightHeight, 20, 28);
        Assert.Equal(XYSize.Compact, state.LeftSize); Assert.Equal(XYDensity.Compact, state.RightDensity);
    }

    [Fact]
    public void Map_meter_fields_show_integer_values_without_rounding_current_map()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            Assert.True(vm.MapSession.UpdateMapProperties(100.0343, 100.0000, -15.4).IsSuccess);
            vm.MapWidthDraft = 100.0343; vm.MapDepthDraft = 100.0000; vm.MapBaseHeightDraft = -15.4;
            var form = new MapFormPanel { DataContext = vm }; host.Show(form, 480, 640); form.UpdateLayout();
            var fields = UiRuntimeTestHost.Descendants<XYNumberField>(form).ToArray();
            return (DecimalPlaces: fields.Select(x => x.DecimalPlaces).ToArray(),
                Steps: fields.Select(x => (x.Step, x.SmallStep)).ToArray(),
                Texts: fields.Select(x => x.Text ?? "").ToArray(), MapSize: vm.MapSizeText,
                MapWidth: vm.MapSession.CurrentMap.SizeMeters.Width, MapDepth: vm.MapSession.CurrentMap.SizeMeters.Depth,
                MapHeight: vm.MapSession.CurrentMap.Surface.BaseHeightMeters);
        });
        Assert.Equal(3, state.DecimalPlaces.Length); Assert.All(state.DecimalPlaces, x => Assert.Equal(0, x));
        Assert.All(state.Steps, x => { Assert.Equal(1, x.Step); Assert.Equal(1, x.SmallStep); });
        Assert.Equal(["100", "100", "-15"], state.Texts);
        Assert.Equal("100 × 100 米", state.MapSize); Assert.Equal(100.0343, state.MapWidth);
        Assert.Equal(100, state.MapDepth); Assert.Equal(-15.4, state.MapHeight);
    }

    [Theory]
    [InlineData(300)]
    [InlineData(360)]
    [InlineData(480)]
    public void Expanded_layer_dock_has_browsable_space_and_collapse_still_works(double width)
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.ToggleEditorMode();
            var right = new Right { DataContext = vm }; host.Show(right, width, 720); right.UpdateLayout();
            var dock = right.FindControl<EditorLayerDock>("LayerWorkspace")!;
            var expanded = (dock.Bounds.Height, dock.FindControl<Grid>("LayerContent")!.IsEffectivelyVisible);
            var pane = dock.FindControl<XYCollapsiblePane>("Pane")!;
            pane.IsCollapsed = true;
            right.UpdateLayout();
            return (ExpandedHeight: expanded.Item1, ExpandedVisible: expanded.Item2,
                Collapsed: pane.IsCollapsed && !dock.FindControl<Grid>("LayerContent")!.IsEffectivelyVisible);
        });
        Assert.True(state.ExpandedVisible); Assert.True(state.ExpandedHeight >= 160); Assert.True(state.Collapsed);
    }

    [Fact]
    public void Layer_action_icons_materialize_as_visible_vector_paths()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var paths = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.ToggleEditorMode();
            vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor); vm.AddLayer();
            var panel = new LayerPanel { DataContext = vm }; host.Show(panel, 360, 720); panel.UpdateLayout();
            return UiRuntimeTestHost.Descendants<AvaloniaPath>(panel)
                .Where(x => x.Classes.Contains("layerIcon"))
                .Select(x => (HasData: x.Data is not null, HasStroke: x.Stroke is not null, Visible: x.IsVisible,
                    Width: x.Bounds.Width, Height: x.Bounds.Height)).ToArray();
        });
        Assert.NotEmpty(paths); Assert.All(paths, path => { Assert.True(path.HasData); Assert.True(path.HasStroke); if (path.Visible) { Assert.True(path.Width >= 12); Assert.True(path.Height >= 12); } });
    }
}

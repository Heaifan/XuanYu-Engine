using Avalonia.Controls;
using XYUI.Avalonia.Controls;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class AreaDR2CorrectionInstanceRuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public AreaDR2CorrectionInstanceRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Right_has_one_shared_inspector_tree_and_persistent_layer_dock()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.AddCubeEntity(); vm.ToggleEditorMode();
            var right = new Right { DataContext = vm }; host.Show(right, 480, 720); right.UpdateLayout();
            return (EditorTabs: Count<EditorRightTabs>(right), Map: Count<MapEditorPanel>(right),
                Entity: Count<EntityInspectorPanel>(right), Dock: Count<EditorLayerDock>(right),
                VisibleEntity: Visible<EntityInspectorPanel>(right), VisibleDock: Visible<EditorLayerDock>(right));
        });

        Assert.Equal(1, state.EditorTabs); Assert.Equal(1, state.Map); Assert.Equal(1, state.Entity);
        Assert.Equal(1, state.Dock); Assert.Equal(1, state.VisibleEntity); Assert.Equal(1, state.VisibleDock);
    }

    [Fact]
    public void Layer_dock_collapse_state_survives_inspector_owner_switch()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.ToggleEditorMode();
            var right = new Right { DataContext = vm }; host.Show(right, 480, 720); right.UpdateLayout();
            var dock = UiRuntimeTestHost.Descendants<EditorLayerDock>(right).Single();
            var pane = dock.FindControl<XYCollapsiblePane>("Pane")!;
            pane.IsCollapsed = true;
            vm.AddCubeEntity(); right.UpdateLayout();
            return (Content: dock.FindControl<Grid>("LayerContent")!.IsVisible, Collapsed: pane.IsCollapsed);
        });
        Assert.False(state.Content); Assert.True(state.Collapsed);
    }

    static int Count<T>(Right right) where T : Control => UiRuntimeTestHost.Descendants<T>(right).Count();
    static int Visible<T>(Right right) where T : Control => UiRuntimeTestHost.Descendants<T>(right).Count(x => x.IsEffectivelyVisible);
}

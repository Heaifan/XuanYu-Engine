using Avalonia.Controls;
using Avalonia.Threading;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class AreaBLeftWorkspaceRuntimeTests
{
    [Fact]
    public void Right_rehomes_map_region_and_layer_workspace_content()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var evidence = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var right = new Right { DataContext = vm };
            host.Show(right, 360, 860);
            right.UpdateLayout();
            vm.ToggleEditorMode(); Dispatcher.UIThread.RunJobs(); right.UpdateLayout();
            var inspector = UiRuntimeTestHost.Descendants<InspectorPanel>(right)
                .Single(x => x.IsEffectivelyVisible);
            var map = UiRuntimeTestHost.Descendants<MapEditorPanel>(inspector).Single();
            var layer = right.FindControl<EditorLayerDock>("LayerWorkspace")!;
            var mapVisible = map.IsVisible;
            var layerVisible = layer.IsVisible;
            vm.SwitchWorkspaceCommand.Execute("RegionEditor"); Dispatcher.UIThread.RunJobs(); right.UpdateLayout();
            var region = UiRuntimeTestHost.Descendants<RegionalAuthoringPanel>(inspector).Single();
            return (mapVisible, region.IsVisible, layerVisible, map.SelectedTabId, region.SelectedTabId,
                vm.IsMapEditMode, vm.IsRegionEditMode);
        });

        Assert.True(evidence.Item1, $"mapVisible={evidence.Item1}, mapMode={evidence.Item6}");
        Assert.True(evidence.Item2);
        Assert.True(evidence.Item3);
        Assert.Equal("base", evidence.Item4);
        Assert.Equal("region", evidence.Item5);
        Assert.False(evidence.Item6);
        Assert.True(evidence.Item7);
    }

}

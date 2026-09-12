using System.Linq;
using Avalonia.Controls;
using Xunit;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Map;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class FeatureEditCR1InspectorRuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public FeatureEditCR1InspectorRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Inspector_geometry_editing_activation_contract()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var vm = new UiVm(null, () => true, seedInitialScene: false);
            var panel = new InspectorPanel { DataContext = vm };
            host.Show(panel, 300, 800); panel.UpdateLayout();
            
            var togglesBefore = UiRuntimeTestHost.Descendants<XYToggleButton>(panel)
                .Where(x => x.Command == vm.ToggleGeometryEditingCommand).ToArray();
            var isVisibleBefore = togglesBefore.Any() && togglesBefore.First().IsEffectivelyVisible;

            var layerId = vm.MapSession.ActiveRegionLayerId;
            var marker = new MapMarker(MapMarkerId.New(), layerId, "T", new(0, 0));
            vm.MapSession.CreateMarker(marker);
            vm.SelectMapGeometry(new MapGeometrySelection(MapGeometryFeatureKind.Marker, marker.MarkerId.ToString()));
            
            panel.UpdateLayout();
            
            var togglesAfter = UiRuntimeTestHost.Descendants<XYToggleButton>(panel)
                .Where(x => x.Command == vm.ToggleGeometryEditingCommand).ToArray();
            var toggle = togglesAfter.FirstOrDefault();
            
            bool isVisibleAfter = toggle?.IsEffectivelyVisible ?? false;
            bool isChecked = toggle?.IsChecked ?? true;
            
            vm.ToggleGeometryEditingCommand.Execute(null); 
            bool isCheckedAfterClick = toggle?.IsChecked ?? false;
            
            vm.ClearMapGeometrySelection(); 
            panel.UpdateLayout();
            bool isVisibleAfterClear = toggle?.IsEffectivelyVisible ?? true;

            return (isVisibleBefore, isVisibleAfter, isChecked, isCheckedAfterClick, isVisibleAfterClear);
        });

        Assert.False(state.isVisibleBefore);
        Assert.True(state.isVisibleAfter);
        Assert.False(state.isChecked);
        Assert.True(state.isCheckedAfterClick);
        Assert.False(state.isVisibleAfterClear);
    }
}

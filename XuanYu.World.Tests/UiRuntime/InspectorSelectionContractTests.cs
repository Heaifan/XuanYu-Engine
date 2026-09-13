using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class InspectorSelectionContractTests
{
    [Fact]
    public void None_selection_resolves_to_empty_even_in_map_edit_mode()
    {
        var vm = NewVm();
        vm.MapSession.ClearSelection();
        vm.ToggleEditorMode();

        Assert.Equal(InspectorObjectKind.Empty, vm.InspectorIdentity);
        Assert.True(vm.IsInspectorEmpty);
        Assert.False(vm.IsMapInspector);
        Assert.False(vm.IsMarkerInspector);
        Assert.False(vm.IsRoadInspector);
        Assert.False(vm.IsRegionInspector);
    }

    [Fact]
    public void Explicit_map_selection_resolves_to_map()
    {
        var vm = NewVm();
        vm.MapSession.SelectMap();

        Assert.Equal(InspectorObjectKind.Map, vm.InspectorIdentity);
        Assert.True(vm.IsMapInspector);
    }

    [Theory]
    [InlineData(MapGeometryFeatureKind.Marker, InspectorObjectKind.Marker)]
    [InlineData(MapGeometryFeatureKind.Road, InspectorObjectKind.Road)]
    [InlineData(MapGeometryFeatureKind.Region, InspectorObjectKind.Region)]
    public void Geometry_selection_resolves_to_its_object_kind(
        MapGeometryFeatureKind geometry, InspectorObjectKind expected)
    {
        var vm = NewVm();
        vm.SelectMapGeometry(new(geometry, "feature-1"));

        Assert.Equal(expected, vm.InspectorIdentity);
    }

    [Fact]
    public void Road_identity_survives_mode_and_geometry_tool_changes()
    {
        var vm = NewVm();
        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Road, "road-1"));
        vm.ToggleEditorMode();
        vm.IsGeometryEditingActive = true;

        Assert.Equal(InspectorObjectKind.Road, vm.InspectorIdentity);
        Assert.True(vm.IsRoadInspector);
    }

    [Fact]
    public void Clear_selection_resolves_to_empty_without_map_fallback()
    {
        var vm = NewVm();
        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Road, "road-1"));
        vm.ClearMapGeometrySelection();
        vm.MapSession.ClearSelection();

        Assert.Equal(InspectorObjectKind.Empty, vm.InspectorIdentity);
    }

    static UiVm NewVm() => new(null, () => true, seedInitialScene: false);
}

using XuanYu.Editor.App;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class AppCompositionRootMapContextTests
{
    [Fact]
    public async Task App_composition_provides_valid_map_identity_for_marker_activation()
    {
        var state = EditorCompositionRoot.CreateEditorState(() => true, seedInitialScene: false);
        Assert.True(state.Map.CurrentMap.MapId.IsValid);

        var vm = new UiVm(null, state.Scene, state.Map, () => true);
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        vm.ToggleEditorModeCommand.Execute(null);

        vm.SelectRegionAuthoringMode("地图标记");

        Assert.True(await vm.BeginContextDrawingAsync("地图标记"));
        Assert.True(vm.IsUnifiedMarkerDrawingActive);
        Assert.Equal(global::XuanYu.Editor.Drawing.DrawingPrimitiveKind.Point,
            vm.UnifiedDrawingSession!.PrimitiveKind);
    }
}

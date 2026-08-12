using XuanYu.Editor.Workspace;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiTokens;

public sealed class LayerAUiCompositionTests
{
    static string Read(string path) => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", path));

    [Fact]
    public void Right_dock_is_edit_only_and_map_context_has_no_layer_page()
    {
        var right = Read("Right/Right.axaml");
        var map = Read("Right/MapEditorPanel.axaml");
        Assert.Contains("EditorLayerDock", right);
        Assert.Contains("IsVisible=\"{Binding IsEditMode}\"", right);
        Assert.Contains("GridSplitter", right);
        Assert.DoesNotContain("Header=\"图层\"", map);
        Assert.DoesNotContain("LayerPanel", map);
    }

    [Fact]
    public void Inspector_owns_layer_attributes_and_map_is_empty()
    {
        var inspector = Read("Right/InspectorPanel.axaml");
        var map = Read("Right/MapPagePanel.axaml");
        var layer = Read("Right/LayerPanel.axaml");
        Assert.Contains("LayerInspectorPanel", inspector);
        Assert.Contains("HasCurrentLayerSelection", inspector);
        Assert.Contains("CurrentLayerItems", layer);
        Assert.DoesNotContain("区域绘制", map);
    }

    [Fact]
    public void Providers_filter_map_and_region_layers_by_workspace()
    {
        var vm = new UiVm(null, isWriteThread: () => true, seedInitialScene: false);
        Assert.Null(vm.CurrentLayerProvider);
        vm.ToggleEditorMode();
        Assert.Empty(vm.CurrentLayerProvider!.Items);
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        Assert.NotEmpty(vm.CurrentLayerProvider!.Items);
        Assert.All(vm.CurrentLayerProvider.Items, item => Assert.Equal("区域面", item.Kind));
        Assert.DoesNotContain(vm.CurrentLayerProvider.Items, item => item.Name is "地面" or "边界");
        Assert.NotEmpty(vm.CurrentLayerItems);
        vm.SelectedLayer = vm.CurrentLayerItems[0];
        Assert.True(vm.HasCurrentLayerSelection);
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.MapEditor);
        Assert.Empty(vm.CurrentLayerItems);
        Assert.Null(vm.SelectedLayer);
    }
}

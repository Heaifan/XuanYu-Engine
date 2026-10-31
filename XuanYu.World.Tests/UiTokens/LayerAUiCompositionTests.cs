using XuanYu.Editor.Workspace;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiTokens;

public sealed class LayerAUiCompositionTests
{
    static string Read(string path) => File.ReadAllText(Path.Combine(TestSourceRoot.Root,  "XuanYu.Editor.UI", path));

    [Fact]
    public void Right_dock_is_edit_only_and_map_context_has_no_layer_page()
    {
        var right = Read("Panels/Inspector/RightPanel/Right.axaml");
        var map = Read("MapUI/Presentation/EditorPanels/MapEditorPanel.axaml");
        Assert.Contains("EditorLayerDock", right);
        Assert.Contains("IsVisible=\"{Binding IsEditMode}\"", right);
        Assert.Contains("GridSplitter", right);
        Assert.DoesNotContain("<local:MapEditorPanel", right);
        Assert.Equal(1, Read("Panels/Inspector/InspectorPanels/InspectorPanel.axaml").Split("<local:MapEditorPanel", StringSplitOptions.None).Length - 1);
        Assert.DoesNotContain("Header=\"图层\"", map);
        Assert.DoesNotContain("LayerPanel", map);
    }

    [Fact]
    public void Inspector_owns_layer_attributes_and_map_is_empty()
    {
        var inspector = Read("Panels/Inspector/InspectorPanels/InspectorPanel.axaml");
        var map = Read("MapUI/Presentation/MapToolPanels/MapPagePanel.axaml");
        var layer = Read("MapUI/Editing/LayerPanels/LayerPanel.axaml");
        Assert.DoesNotContain("LayerInspectorPanel", inspector);
        Assert.DoesNotContain("HasCurrentLayerSelection", inspector);
        Assert.Contains("InspectorPropertyRow", inspector);
        Assert.Contains("VisibleItems", layer);
        Assert.DoesNotContain("区域绘制", map);
    }

    [Fact]
    public void Providers_filter_map_and_region_layers_by_workspace()
    {
        var vm = new UiVm(null, isWriteThread: () => true, seedInitialScene: false);
        Assert.Null(vm.CurrentLayerProvider);
        vm.ToggleEditorMode();
        Assert.Equal(3, vm.CurrentLayerProvider!.Items.Count);
        Assert.Contains(vm.CurrentLayerProvider.Items, item => item.Name == "地面");
        Assert.Contains(vm.CurrentLayerProvider.Items, item => item.Name == "边界");
        Assert.Contains(vm.CurrentLayerProvider.Items, item => item.Name == "区域 1");
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        Assert.NotEmpty(vm.CurrentLayerProvider!.Items);
        Assert.All(vm.CurrentLayerProvider.Items, item => Assert.Equal("区域面", item.Kind));
        Assert.DoesNotContain(vm.CurrentLayerProvider.Items, item => item.Name is "地面" or "边界");
        Assert.NotEmpty(vm.CurrentLayerItems);
        vm.SelectedLayer = vm.CurrentLayerItems[0];
        Assert.True(vm.HasCurrentLayerSelection);
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.MapEditor);
        Assert.Equal(3, vm.CurrentLayerItems.Count);
        Assert.False(vm.HasCurrentLayerSelection);
    }
}

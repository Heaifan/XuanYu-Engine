using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class PointFeatureEntryRuntimeTests
{
    [Fact]
    public void Point_feature_entry_enters_marker_authoring_in_edit_mode()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);

        vm.OpenPointFeatureEditorCommand.Execute(null);

        Assert.Equal(EditorWorkspaceId.RegionEditor, vm.CurrentWorkspace.Id);
        Assert.True(vm.IsEditMode);
        Assert.True(vm.IsRegionEditMode);
        Assert.True(vm.IsMarkerAuthoringMode);
    }
}

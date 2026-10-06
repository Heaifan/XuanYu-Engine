using XuanYu.Editor.Mode;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;

namespace XuanYu.World.Tests.Mode;

public sealed class WorkspaceAuthoritySceneResetTests
{
    [Fact]
    public void New_blank_scene_preserves_workspace_authority()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        vm.ToggleEditorMode();
        var footer = vm.FooterMode;

        vm.NewBlankScene();

        Assert.Equal(EditorModeId.Edit, vm.CurrentMode);
        Assert.Equal(EditorWorkspaceId.RegionEditor, vm.CurrentWorkspace.Id);
        Assert.Equal(footer, vm.FooterMode);
    }
}

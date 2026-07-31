using XuanYu.Core.Space;
using XuanYu.Editor.UI;
using XuanYu.World;

namespace XuanYu.World.Tests.World;

public sealed class WorldCR2UiHistoryTests
{
    [Fact]
    public void Production_start_is_blank_and_add_undo_redo_restores_id_and_selection()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.Empty(vm.RenderSnapshot.Entities);

        vm.RunCommand.Execute("添加立方体");
        var id = vm.RenderSnapshot.Entity.EntityKey;
        Assert.Equal("立方体", vm.SelectionTitle);
        Assert.True(vm.IsSceneDirty);

        vm.TryUndoFromShortcut();
        Assert.Empty(vm.RenderSnapshot.Entities);
        Assert.False(vm.HasSelection);
        Assert.False(vm.IsSceneDirty);

        vm.TryRedoFromShortcut();
        Assert.Equal(id, vm.RenderSnapshot.Entity.EntityKey);
        Assert.True(vm.HasSelection);
        Assert.True(vm.IsSceneDirty);
    }

    [Fact]
    public void Delete_and_rename_share_history_and_preserve_identity()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.AddCubeEntity();
        var id = vm.RenderSnapshot.Entity.EntityKey;
        Assert.True(vm.BeginRenameSelectedEntity());
        Assert.True(vm.SelectedHierarchyItem!.IsRenaming);
        vm.CancelInlineRename(vm.SelectedHierarchyItem);
        Assert.True(vm.RenameSelectedEntity("主方块"));
        Assert.Equal("主方块", vm.RenderSnapshot.Entity.Name);

        Assert.True(vm.DeleteSelectedEntity());
        Assert.Empty(vm.RenderSnapshot.Entities);
        vm.TryUndoFromShortcut();
        Assert.Equal(id, vm.RenderSnapshot.Entity.EntityKey);
        Assert.Equal("主方块", vm.RenderSnapshot.Entity.Name);
        vm.TryUndoFromShortcut();
        Assert.Equal("立方体", vm.RenderSnapshot.Entity.Name);
        Assert.Equal(id, vm.RenderSnapshot.Entity.EntityKey);
    }

    [Fact]
    public void New_scene_resets_default_camera_and_clears_selection()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.AddCubeEntity();
        vm.DollyCamera(120);

        vm.NewBlankScene();

        Assert.Empty(vm.RenderSnapshot.Entities);
        Assert.False(vm.HasSelection);
        Assert.Equal(DefaultEditorCamera.Position, vm.RenderSnapshot.CameraState.Position);
        Assert.Equal(DefaultEditorCamera.Target, vm.ObservationCenter);
    }

    [Fact]
    public async Task Save_add_undo_is_clean_and_redo_is_dirty()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.xyscene");
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveSceneAsync(path, saveAs: true));

        vm.AddCubeEntity();
        Assert.True(vm.IsSceneDirty);
        vm.TryUndoFromShortcut();
        Assert.False(vm.IsSceneDirty);
        vm.TryRedoFromShortcut();
        Assert.True(vm.IsSceneDirty);
    }
}

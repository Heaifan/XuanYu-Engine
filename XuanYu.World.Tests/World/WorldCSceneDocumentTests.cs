using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Editor.SceneDocument;
using XuanYu.Editor.UI;
using XuanYu.World;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.World;

public sealed class WorldCSceneDocumentTests
{
    [Fact]
    public async Task Save_and_load_preserves_entity_identity_name_and_transform()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.xyscene");
        var scene = new SceneStateOwner(null, seedInitialEntity: false);
        scene.CreateEntity("战役测试实体", "MinimalSceneEntity",
            new CommittedTransform(new Vector3d(1, 2, 3), new Vector3d(4, 5, 6), new Vector3d(1, 2, 3)));
        var storage = new SceneStorageService();
        var snapshot = SceneDocumentWorldBridge.Capture(scene, "scene-1", "战役测试场景");

        var saved = await storage.SaveAsync(path, snapshot);
        var loaded = await storage.LoadAsync(path);

        Assert.True(saved.Succeeded);
        Assert.True(loaded.Succeeded);
        var entity = Assert.Single(loaded.Value!.Entities);
        Assert.Equal(snapshot.Entities[0].Id, entity.Id);
        Assert.Equal("战役测试实体", entity.Name);
        Assert.Equal(snapshot.Entities[0].Transform, entity.Transform);
    }

    [Fact]
    public async Task Broken_json_load_fails_without_replacing_current_scene()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.xyscene");
        await File.WriteAllTextAsync(path, "{ broken");
        var vm = new UiVm(null, () => true, seedInitialScene: false);

        var opened = await vm.OpenSceneAsync(path);

        Assert.False(opened);
        Assert.Empty(vm.HierarchyItems);
        Assert.False(vm.IsSceneDirty);
    }

    [Fact]
    public async Task Saving_after_transform_marks_clean_and_undo_returns_clean()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.xyscene");
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.NewBlankScene();
        await vm.OpenSceneAsync(await CreateOneEntityScene(path));
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(n => n.Key == "EntityId(1)");
        Assert.True(vm.TryCommitInspectorTransformValue("位置", "X", "2"));
        Assert.True(vm.IsSceneDirty);

        Assert.True(await vm.SaveSceneAsync(path));
        Assert.False(vm.IsSceneDirty);
        vm.TryUndoFromShortcut();

        Assert.True(vm.IsSceneDirty);
    }

    static async Task<string> CreateOneEntityScene(string path)
    {
        var scene = new SceneStateOwner(null, seedInitialEntity: false);
        scene.CreateEntity("测试实体", "MinimalSceneEntity");
        var storage = new SceneStorageService();
        var snapshot = SceneDocumentWorldBridge.Capture(scene, "scene-1", "测试场景");
        Assert.True((await storage.SaveAsync(path, snapshot)).Succeeded);
        return path;
    }
}

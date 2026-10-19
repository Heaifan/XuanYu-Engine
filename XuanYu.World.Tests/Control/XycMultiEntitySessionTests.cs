using XuanYu.Editor.UI;
using XuanYu.Editor.UI.XycControl;
using XuanYu.World;

namespace XuanYu.World.Tests.Xyc;

public sealed class XycMultiEntitySessionTests
{
    [Fact]
    public void Session_creates_distinct_cubes_and_reads_complete_transforms()
    {
        var vm = NewVm();
        var a = vm.CreateXycTestCube(vm.SceneId, vm.SceneRevision);
        var b = vm.CreateXycTestCube(vm.SceneId, a.SceneRevision);
        Assert.Equal(2, b.SessionEntityIds!.Length);
        Assert.NotEqual(b.SessionEntityIds[0], b.SessionEntityIds[1]);
        var entity = vm.InspectXycSceneEntity(b.SessionEntityIds[0]);
        Assert.Equal("Cube", entity.Type);
        Assert.Equal(new Vector3(0, 0, 0), entity.Transform.Position);
        Assert.Equal(new Vector3(0, 0, 0), entity.Transform.Rotation);
        Assert.Equal(new Vector3(1, 1, 1), entity.Transform.Scale);
    }

    [Fact]
    public void Non_session_entity_cannot_be_moved()
    {
        var vm = NewVm();
        var state = vm.CreateXycTestCube(vm.SceneId, vm.SceneRevision);
        var before = vm.InspectXycTestScene();
        var error = Assert.Throws<XycSceneOperationException>(() => vm.SetXycTestPosition(
            vm.SceneId, before.SceneRevision, "EntityId(987654)", new Vector3(5, 0, 0)));
        Assert.Equal("UNAUTHORIZED_ENTITY", error.Code);
        var after = vm.InspectXycTestScene();
        Assert.Equal(before.SceneRevision, after.SceneRevision);
        Assert.Equal(before.EntityIds, after.EntityIds);
        Assert.Equal(before.SessionEntityIds, after.SessionEntityIds);
        Assert.Equal(before.UndoTop, after.UndoTop);
        Assert.Equal(before.UndoCount, after.UndoCount);
        Assert.Equal(before.RedoCount, after.RedoCount);
        Assert.Single(state.SessionEntityIds!);
    }

    [Fact]
    public void Scene_id_and_revision_mismatch_do_not_mutate_session()
    {
        var vm = NewVm();
        var state = vm.CreateXycTestCube(vm.SceneId, vm.SceneRevision);
        var before = vm.InspectXycTestScene();
        Assert.Equal("SCENE_MISMATCH", Assert.Throws<XycSceneOperationException>(() =>
            vm.CreateXycTestCube("wrong", before.SceneRevision)).Code);
        Assert.Equal("STALE_REVISION", Assert.Throws<XycSceneOperationException>(() =>
            vm.SetXycTestPosition(vm.SceneId, before.SceneRevision - 1,
                state.SessionEntityIds![0], new Vector3(1, 2, 3))).Code);
        var after = vm.InspectXycTestScene();
        Assert.Equal(before.SceneId, after.SceneId);
        Assert.Equal(before.SceneRevision, after.SceneRevision);
        Assert.Equal(before.EntityCount, after.EntityCount);
        Assert.Equal(before.EntityIds, after.EntityIds);
        Assert.Equal(before.HierarchyEntityIds, after.HierarchyEntityIds);
        Assert.Equal(before.RenderEntityIds, after.RenderEntityIds);
        Assert.Equal(before.SessionEntityIds, after.SessionEntityIds);
        Assert.Equal(before.UndoTop, after.UndoTop);
        Assert.Equal(before.RedoTop, after.RedoTop);
        Assert.Equal(before.UndoCount, after.UndoCount);
        Assert.Equal(before.RedoCount, after.RedoCount);
        Assert.Equal(before.CommitStatus, after.CommitStatus);
    }

    static UiVm NewVm() => new(null, () => true, seedInitialScene: false);
}

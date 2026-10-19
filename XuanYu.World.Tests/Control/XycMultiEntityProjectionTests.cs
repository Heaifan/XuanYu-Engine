using System.Reflection;
using XuanYu.Core.Identity;
using XuanYu.Core.Scene;
using XuanYu.Editor.UI;
using XuanYu.Editor.UI.XycControl;
using XuanYu.World;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.Xyc;

public sealed class XycMultiEntityProjectionTests
{
    [Fact]
    public void Create_undo_keeps_world_hierarchy_and_render_snapshot_in_sync()
    {
        var vm = NewVm();
        var state = vm.CreateXycTestCube(vm.SceneId, vm.SceneRevision);
        state = vm.CreateXycTestCube(vm.SceneId, state.SceneRevision);
        var id = state.SessionEntityIds![1];
        state = vm.UndoXycTestOperation(vm.SceneId, state.SceneRevision, id);
        var scene = GetScene(vm);
        Assert.DoesNotContain(id, state.EntityIds);
        Assert.DoesNotContain(id, state.HierarchyEntityIds);
        Assert.DoesNotContain(id, state.RenderEntityIds);
        Assert.DoesNotContain(scene.Entities, e => e.EntityKey.ToString() == id);
        Assert.Equal(state.EntityIds.Order(), state.HierarchyEntityIds.Order());
        Assert.Equal(state.EntityIds.Order(), state.RenderEntityIds.Order());
    }

    [Fact]
    public void Inspector_fields_change_identity_with_explicit_selection()
    {
        var vm = NewVm();
        var state = vm.CreateXycTestCube(vm.SceneId, vm.SceneRevision);
        state = vm.CreateXycTestCube(vm.SceneId, state.SceneRevision);
        var ids = state.SessionEntityIds!;
        state = vm.SetXycTestPosition(vm.SceneId, state.SceneRevision, ids[0], new Vector3(11, 2, 3));
        state = vm.SetXycTestPosition(vm.SceneId, state.SceneRevision, ids[1], new Vector3(-7, 4, 5));
        Select(vm, ids[0]);
        var first = vm.InspectXycTestScene();
        Select(vm, ids[1]);
        var second = vm.InspectXycTestScene();
        Assert.Equal(ids[0], first.InspectorEntityId);
        Assert.Equal(ids[1], second.InspectorEntityId);
        Assert.Contains("X 11", Assert.Single(first.InspectorFields, field => field.Label == "位置").Value);
        Assert.Contains("Y 2", Assert.Single(first.InspectorFields, field => field.Label == "位置").Value);
        Assert.Contains("X -7", Assert.Single(second.InspectorFields, field => field.Label == "位置").Value);
        Assert.Contains("Y 4", Assert.Single(second.InspectorFields, field => field.Label == "位置").Value);
    }

    [Fact]
    public void Undo_create_can_be_redone_as_the_same_session_entity()
    {
        var vm = NewVm();
        var state = vm.CreateXycTestCube(vm.SceneId, vm.SceneRevision);
        var id = state.SessionEntityIds![0];
        state = vm.UndoXycTestOperation(vm.SceneId, state.SceneRevision, id);
        Assert.DoesNotContain(id, state.EntityIds);
        Assert.Equal(id, state.RedoTop!.EntityId);
        state = vm.RedoXycTestOperation(vm.SceneId, state.SceneRevision, id);
        Assert.Contains(id, state.EntityIds);
        Assert.Contains(id, state.HierarchyEntityIds);
        Assert.Contains(id, state.RenderEntityIds);
    }

    static void Select(UiVm vm, string id) => typeof(UiVm)
        .GetMethod("SelectEntity", BindingFlags.Instance | BindingFlags.NonPublic)!
        .Invoke(vm, [EntityId.FromInt(int.Parse(id[9..^1])), "XYC test"]);
    static UiVm NewVm() => new(null, () => true, seedInitialScene: false);
    static SceneStateOwner GetScene(UiVm vm) => (SceneStateOwner)typeof(UiVm)
        .GetField("_sceneState", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(vm)!;
}

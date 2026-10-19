using System.Reflection;
using XuanYu.Core.Identity;
using XuanYu.Editor.UI;
using XuanYu.Editor.UI.XycControl;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.Xyc;

public sealed class XycHistoryApplyFailureTests
{
    [Fact]
    public void Generic_undo_apply_failure_keeps_undo_and_redo_stacks_unchanged()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var state = vm.CreateXycTestCube(vm.SceneId, vm.SceneRevision);
        var id = state.SessionEntityIds![0];
        state = vm.SetXycTestPosition(vm.SceneId, state.SceneRevision, id, new Vector3(9, 0, 0));
        var scene = (SceneStateOwner)typeof(UiVm)
            .GetField("_sceneState", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(vm)!;
        Assert.True(scene.DestroyEntity(EntityId.FromInt(int.Parse(id[9..^1]))));
        var before = vm.InspectXycTestScene();

        vm.TryUndoFromShortcut();

        var after = vm.InspectXycTestScene();
        Assert.Equal(before.UndoCount, after.UndoCount);
        Assert.Equal(before.RedoCount, after.RedoCount);
        Assert.Equal(before.UndoTop, after.UndoTop);
        Assert.Equal(before.RedoTop, after.RedoTop);
        Assert.DoesNotContain(id, after.EntityIds);
    }
}

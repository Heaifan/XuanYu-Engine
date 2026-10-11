using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Editor.Composition;
using XuanYu.Editor.UI;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.Camera;

public sealed class SceneFarPlaneRefreshTests
{
    [Fact]
    public void Scene_bounds_changes_refresh_far_without_moving_camera()
    {
        var scene = new SceneStateOwner(null, false);
        var entity = scene.AddCubeEntity();
        var map = EditorRuntimeComposition.CreateMapSession(() => true);
        var vm = new UiVm(null, scene, map, () => true);
        vm.UpdateViewportFrame(800, 600);
        var before = vm.RenderSnapshot.CameraState;
        var farPosition = before.Position + before.Forward * 500_000;

        scene.CommitTransformWithResult(entity.EntityKey,
            new CommittedTransform(farPosition));

        var after = vm.RenderSnapshot.CameraState;
        Assert.Equal(before.Position, after.Position);
        Assert.Equal(before.Forward, after.Forward);
        Assert.True(after.FarPlane > 500_000,
            $"Scene bounds moved to {farPosition} but FarPlane stayed {after.FarPlane:R}.");
    }
}

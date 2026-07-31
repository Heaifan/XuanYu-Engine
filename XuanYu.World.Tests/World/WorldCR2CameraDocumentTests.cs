using XuanYu.Core.Space;
using XuanYu.Editor.SceneDocument;
using XuanYu.Editor.UI;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.World;

public sealed class WorldCR2CameraDocumentTests
{
    [Fact]
    public async Task Successful_open_resets_default_camera()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.xyscene");
        var scene = new SceneStateOwner(null, false);
        scene.AddCubeEntity();
        var snapshot = SceneDocumentWorldBridge.Capture(scene, "camera", "Camera");
        Assert.True((await new SceneStorageService().SaveAsync(path, snapshot)).Succeeded);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.DollyCamera(120);

        Assert.True(await vm.OpenSceneAsync(path));

        Assert.Equal(DefaultEditorCamera.Position, vm.RenderSnapshot.CameraState.Position);
        Assert.Equal(DefaultEditorCamera.Target, vm.ObservationCenter);
    }

    [Fact]
    public async Task Failed_open_preserves_camera()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.xyscene");
        await File.WriteAllTextAsync(path, "{ broken");
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.DollyCamera(120);
        var before = vm.RenderSnapshot.CameraState;

        Assert.False(await vm.OpenSceneAsync(path));

        Assert.Equal(before, vm.RenderSnapshot.CameraState);
    }
}

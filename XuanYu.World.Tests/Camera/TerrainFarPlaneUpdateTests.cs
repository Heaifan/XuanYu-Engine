using System.Buffers.Binary;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Camera;

public sealed class TerrainFarPlaneUpdateTests
{
    [Fact]
    public async Task Vertical_exaggeration_refreshes_far_plane_without_camera_motion()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.UpdateViewportFrame(800, 600);
        var path = WriteDem();
        try { Assert.True(await vm.ImportTerrainSourceAsync(path), vm.TerrainImportMessage); }
        finally { File.Delete(path); }
        var before = vm.RenderSnapshot.CameraState;

        vm.VerticalExaggeration = 20;

        var after = vm.RenderSnapshot.CameraState;
        Assert.Equal(before.Position, after.Position);
        Assert.Equal(before.Forward, after.Forward);
        Assert.Equal(after.FarPlane, TerrainFarPlanePolicy.Resolve(after,
            vm.CurrentViewport, TerrainFarPlaneTestBounds.For(vm)));
    }

    static string WriteDem()
    {
        var path = Path.Combine(Path.GetTempPath(), $"n23e121-xye-pr2-far-{Guid.NewGuid():N}.hgt");
        using var stream = File.Create(path);
        Span<byte> bytes = stackalloc byte[2];
        for (var index = 0; index < 9; index++)
        {
            BinaryPrimitives.WriteInt16BigEndian(bytes, index == 4 ? (short)100 : (short)0);
            stream.Write(bytes);
        }
        return path;
    }
}

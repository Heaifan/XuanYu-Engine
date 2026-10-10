using System.Buffers.Binary;
using Xunit.Abstractions;
using XuanYu.Editor.Camera;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World;

namespace XuanYu.World.Tests.Camera;

public sealed class XYEPR2NavigationSafetyFarPlaneTests(ITestOutputHelper output)
{
    [Fact(DisplayName = "XYEPR2-NAV-SAFETY-FAR-001")]
    public async Task Constrained_dolly_far_plane_covers_observation_center()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.UpdateViewportFrame(800, 600);
        var path = WriteDem([0, 0, 0, 0, 100, 0, 0, 0, 0]);
        try { Assert.True(await vm.ImportTerrainSourceAsync(path)); }
        finally { File.Delete(path); }

        var center = vm.ObservationCenter;
        var candidate = default(CameraFrameResult);
        for (var step = 0; step < 100; step++)
        {
            var before = vm.RenderSnapshot.CameraState;
            Assert.True(CameraNavigation.TryDolly(before, center, 1,
                before.Revision + 1, out candidate, out var reason), reason);
            Assert.True(vm.DollyCamera(1));
        }

        var final = vm.RenderSnapshot.CameraState;
        var targetDepth = (center - final.Position).Dot(final.Forward);
        var expectedFarPlane = Math.Max(final.NearPlane * 10.0,
            final.Position.DistanceTo(center) * 4.0);
        output.WriteLine($"candidate={candidate.Camera.Position}; candidateFar={candidate.Camera.FarPlane:R}");
        output.WriteLine($"lastSafe={final.Position}; center={vm.ObservationCenter}; forward={final.Forward}");
        output.WriteLine($"near={final.NearPlane:R}; far={final.FarPlane:R}; " +
            $"expectedFar={expectedFarPlane:R}; targetDepth={targetDepth:R}");

        Assert.Equal(center, vm.ObservationCenter);
        Assert.True(targetDepth > 0, $"ObservationCenter is behind the camera: {targetDepth:R}m.");
        Assert.Equal(expectedFarPlane, final.FarPlane);
    }

    static string WriteDem(short[] samples)
    {
        var path = Path.Combine(Path.GetTempPath(), $"n23e121-xye-pr2-far-{Guid.NewGuid():N}.hgt");
        using var stream = File.Create(path);
        Span<byte> bytes = stackalloc byte[2];
        foreach (var sample in samples)
        {
            BinaryPrimitives.WriteInt16BigEndian(bytes, sample);
            stream.Write(bytes);
        }
        return path;
    }
}

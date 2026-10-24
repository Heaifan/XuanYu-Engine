using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Tests.Space;

public sealed class CameraStateTests
{
    [Fact]
    public void NormalizesAxes()
    {
        var camera = new CameraState(Vector3d.Zero, new Vector3d(0, 0, 5), Vector3d.UnitY, 60, 0.1, 100, 3);

        SpaceAssert.Near(Vector3d.UnitZ, camera.Forward);
        SpaceAssert.Near(-Vector3d.UnitX, camera.Right);
        SpaceAssert.Near(Vector3d.UnitY, camera.Up);
        Assert.Equal(3, camera.Revision);
    }

    [Fact]
    public void OrthonormalizesUp()
    {
        var camera = new CameraState(Vector3d.Zero, Vector3d.UnitZ, new Vector3d(0, 2, 1), 60, 0.1, 100, 0);

        SpaceAssert.Near(-Vector3d.UnitX, camera.Right);
        SpaceAssert.Near(Vector3d.UnitY, camera.Up);
        SpaceAssert.Near(0, camera.Forward.Dot(camera.Up));
    }

    [Fact]
    public void RejectsDegenerateAxes()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CameraState(Vector3d.Zero, Vector3d.Zero, Vector3d.UnitY, 60, 0.1, 100, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new CameraState(Vector3d.Zero, Vector3d.UnitZ, Vector3d.UnitZ, 60, 0.1, 100, 0));
    }

    [Fact]
    public void RejectsInvalidParameters()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CameraState(Vector3d.Zero, Vector3d.UnitZ, Vector3d.UnitY, 0, 0.1, 100, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new CameraState(Vector3d.Zero, Vector3d.UnitZ, Vector3d.UnitY, 60, 0, 100, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new CameraState(Vector3d.Zero, Vector3d.UnitZ, Vector3d.UnitY, 60, 1, 1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new CameraState(new Vector3d(double.NaN, 0, 0), Vector3d.UnitZ, Vector3d.UnitY, 60, 0.1, 100, 0));
    }
}

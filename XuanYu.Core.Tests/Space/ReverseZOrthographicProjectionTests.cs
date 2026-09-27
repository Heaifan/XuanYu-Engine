using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Tests.Space;

public sealed class ReverseZOrthographicProjectionTests
{
    const double Near = 0.05;
    const double Far = 800_000;

    [Fact] public void OrthographicNearMapsToOneTest() => Assert.Equal(1, Depth(Near), 5);
    [Fact] public void OrthographicFarMapsToZeroTest() => Assert.Equal(0, Depth(Far), 5);

    [Fact]
    public void OrthographicDepthLinearTest()
    {
        var a = Depth(100_000);
        var b = Depth(200_000);
        Assert.Equal(2.0 * a - Depth(0.0 + Near), b, 5);
    }

    [Fact]
    public void OrthographicScaleAndAspectArePreservedTest()
    {
        var state = State(800, 400);
        Assert.Equal(2.0 / 4.0, state.Projection.M11, 5);
        Assert.Equal(2.0 / 2.0, state.Projection.M22, 5);
        Assert.Equal(2.0, state.Projection.M22 / state.Projection.M11, 5);
    }

    [Fact]
    public void OrthographicInverseRoundTripTest()
    {
        var state = State(800, 600);
        var expected = new Vector3d(0.75, 1.25, 200_000);
        var clip = System.Numerics.Vector4.Transform(
            new System.Numerics.Vector4((float)expected.X, (float)expected.Y, (float)expected.Z, 1), state.ViewProjection);
        var actual = state.TransformPointToWorld(clip.X / clip.W, clip.Y / clip.W, clip.Z / clip.W);
        Assert.True(expected.DistanceTo(actual) < 0.5);
    }

    static double Depth(double distance)
    {
        var state = State(800, 600);
        var clip = System.Numerics.Vector4.Transform(new System.Numerics.Vector4(0, 0, (float)distance, 1), state.ViewProjection);
        return clip.Z / clip.W;
    }

    static ViewProjectionState State(int width, int height) =>
        ViewProjectionState.Create(new CameraState(Vector3d.Zero, Vector3d.UnitZ, Vector3d.UnitY, 60, Near, Far, 0,
            ProjectionMode.Orthographic, 2.0), new ViewportState(0, 0, width, height, width, height, 1, 0));
}

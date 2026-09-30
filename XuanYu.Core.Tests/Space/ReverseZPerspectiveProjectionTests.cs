using System.Numerics;
using XuanYu.Core.Gizmo;
using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Tests.Space;

public sealed class ReverseZPerspectiveProjectionTests
{
    const double Near = 0.05;
    const double Far = 800_000;

    [Fact] public void PerspectiveNearMapsToOneTest() => Assert.Equal(1, Depth(Near), 5);
    [Fact] public void PerspectiveFarMapsToZeroTest() => Assert.Equal(0, Depth(Far), 5);

    [Fact]
    public void PerspectiveDepthMonotonicallyDecreasesTest()
    {
        var values = new[] { 1.0, 1_000.0, 20_000.0, 100_000.0, Far };
        for (var i = 1; i < values.Length; i++) Assert.True(Depth(values[i]) < Depth(values[i - 1]));
    }

    [Fact]
    public void PerspectiveFovAndAspectArePreservedTest()
    {
        var matrix = State(800, 400).Projection;
        var expectedY = 1.0 / global::System.Math.Tan(60.0 * global::System.Math.PI / 360.0);
        Assert.Equal(expectedY, matrix.M22, 5);
        Assert.Equal(2.0, matrix.M22 / matrix.M11, 5);
    }

    [Fact]
    public void PerspectiveCenterRayPreservedTest()
    {
        var state = State(800, 600);
        var near = state.TransformPointToWorld(0, 0, 1);
        var far = state.TransformPointToWorld(0, 0, 0);
        Assert.True((far - near).Normalize().DistanceTo(Vector3d.UnitZ) < 0.0001);
    }

    [Fact]
    public void PerspectiveNdcDepthUsesKnownWorldDistancesTest()
    {
        var state = State(800, 600);
        Assert.True(state.TransformPointToWorld(0, 0, 1).DistanceTo(new Vector3d(0, 0, Near)) < 0.0001);
        Assert.True(state.TransformPointToWorld(0, 0, 0).DistanceTo(new Vector3d(0, 0, Far)) < 0.5);

        var distance = 100.0;
        var edgeX = distance * global::System.Math.Tan(global::System.Math.PI / 6) * (800.0 / 600.0);
        Assert.Equal(new ScreenPoint(800, 300), state.ProjectWorldPoint(new Vector3d(-edgeX, 0, distance)));
    }

    [Fact]
    public void PerspectiveExtremeRatioFiniteTest()
    {
        foreach (var far in new[] { 80_000.0, 800_000.0, 2_000_000.0 })
        {
            var state = State(800, 600, far);
            Assert.True(IsFinite(state.Projection));
            Assert.True(IsFinite(state.ViewProjection));
            Assert.True(IsFinite(state.InverseViewProjection));
        }
    }

    static double Depth(double distance) =>
        Clip(State(800, 600), new Vector3d(0, 0, distance));

    static double Clip(ViewProjectionState state, Vector3d point)
    {
        var clip = Vector4.Transform(new Vector4((float)point.X, (float)point.Y, (float)point.Z, 1), state.ViewProjection);
        return clip.Z / clip.W;
    }

    static ViewProjectionState State(int width, int height, double far = Far) =>
        ViewProjectionState.Create(new CameraState(Vector3d.Zero, Vector3d.UnitZ, Vector3d.UnitY, 60, Near, far, 0),
            new ViewportState(0, 0, width, height, width, height, 1, 0));

    static bool IsFinite(Matrix4x4 m) =>
        float.IsFinite(m.M11) && float.IsFinite(m.M12) && float.IsFinite(m.M13) && float.IsFinite(m.M14) &&
        float.IsFinite(m.M21) && float.IsFinite(m.M22) && float.IsFinite(m.M23) && float.IsFinite(m.M24) &&
        float.IsFinite(m.M31) && float.IsFinite(m.M32) && float.IsFinite(m.M33) && float.IsFinite(m.M34) &&
        float.IsFinite(m.M41) && float.IsFinite(m.M42) && float.IsFinite(m.M43) && float.IsFinite(m.M44);
}

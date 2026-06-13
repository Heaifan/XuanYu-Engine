using FluidWarfare.Core.Math;
using FluidWarfare.Render.Selection;
using FluidWarfare.Render.Selection.Ground;

namespace FluidWarfare.Tests.Render.Selection.Ground;

public sealed class SceneRayGroundIntersectionTests
{
    private static readonly SceneGroundPlane DefaultGround = SceneGroundPlane.Default;
    private const double Epsilon = 1e-10;

    [Fact]
    public void DownwardZRay_HitsGround()
    {
        // Ray pointing down along -Z, starting at Z=10
        var ray = new SceneRay(new Vector3d(0, 0, 10), new Vector3d(0, 0, -1));
        var hit = SceneRayGroundIntersection.Intersect(ray, DefaultGround);

        Assert.True(hit.IsHit);
        Assert.Equal(10, hit.Distance, Epsilon);
        Assert.NotNull(hit.WorldPosition);
        Assert.Equal(0, hit.WorldPosition.Value.X, Epsilon);
        Assert.Equal(0, hit.WorldPosition.Value.Y, Epsilon);
        Assert.Equal(0, hit.WorldPosition.Value.Z, Epsilon);
    }

    [Fact]
    public void ParallelToXYGround_ReturnsNoHit()
    {
        // Ray along XY plane (Direction.Z = 0) — parallel to ground
        var ray = new SceneRay(new Vector3d(0, 0, 10), new Vector3d(1, 0, 0));
        var hit = SceneRayGroundIntersection.Intersect(ray, DefaultGround);

        Assert.False(hit.IsHit);
    }

    [Fact]
    public void BackwardIntersection_ReturnsNoHit()
    {
        // Ray starts below ground and goes further down (-Z)
        var ray = new SceneRay(new Vector3d(0, 0, -5), new Vector3d(0, 0, -1));
        var hit = SceneRayGroundIntersection.Intersect(ray, DefaultGround);

        Assert.False(hit.IsHit);
    }

    [Fact]
    public void RayStartingOnGround_ReturnsZeroDistance()
    {
        var ray = new SceneRay(new Vector3d(2, 3, 0), new Vector3d(0, 0, -1));
        var hit = SceneRayGroundIntersection.Intersect(ray, DefaultGround);

        Assert.True(hit.IsHit);
        Assert.Equal(0, hit.Distance, Epsilon);
        Assert.NotNull(hit.WorldPosition);
        Assert.Equal(2, hit.WorldPosition.Value.X, Epsilon);
        Assert.Equal(3, hit.WorldPosition.Value.Y, Epsilon);
        Assert.Equal(0, hit.WorldPosition.Value.Z, Epsilon);
    }

    [Fact]
    public void CustomGroundElevation_ReturnsExpectedPosition()
    {
        var ground = new SceneGroundPlane(5.0);
        var ray = new SceneRay(new Vector3d(0, 0, 10), new Vector3d(0, 0, -1));
        var hit = SceneRayGroundIntersection.Intersect(ray, ground);

        Assert.True(hit.IsHit);
        Assert.Equal(5, hit.Distance, Epsilon);
        Assert.NotNull(hit.WorldPosition);
        Assert.Equal(0, hit.WorldPosition.Value.X, Epsilon);
        Assert.Equal(0, hit.WorldPosition.Value.Y, Epsilon);
        Assert.Equal(5, hit.WorldPosition.Value.Z, Epsilon);
    }

    [Fact]
    public void HitPosition_SatisfiesRayEquation()
    {
        var origin = new Vector3d(4, -3, 8);
        var dir = new Vector3d(0.3, 0.5, -0.8);
        dir = Normalize(dir);
        var ray = new SceneRay(origin, dir);

        var hit = SceneRayGroundIntersection.Intersect(ray, DefaultGround);
        Assert.True(hit.IsHit);

        // P(t) = Origin + Direction * t should equal HitPosition
        var expected = new Vector3d(
            origin.X + dir.X * hit.Distance,
            origin.Y + dir.Y * hit.Distance,
            origin.Z + dir.Z * hit.Distance);

        Assert.Equal(expected.X, hit.WorldPosition!.Value.X, Epsilon);
        Assert.Equal(expected.Y, hit.WorldPosition.Value.Y, Epsilon);
        Assert.Equal(expected.Z, hit.WorldPosition.Value.Z, Epsilon);
    }

    [Fact]
    public void UpwardZRayFromBelow_HitsGround()
    {
        // Ray starts at Z=-10 and points up (+Z), intersection at Z=0
        var ray = new SceneRay(new Vector3d(0, 0, -10), new Vector3d(0, 0, 1));
        var hit = SceneRayGroundIntersection.Intersect(ray, DefaultGround);

        Assert.True(hit.IsHit);
        Assert.Equal(10, hit.Distance, Epsilon);
    }

    [Fact]
    public void DiagonalDownwardRay_HitsGroundAtCorrectPosition()
    {
        // From (10, 0, 20) at 45 degrees downward along -Z and +X
        var dir = new Vector3d(1, 0, -1);
        dir = Normalize(dir);
        var ray = new SceneRay(new Vector3d(10, 0, 20), dir);

        var hit = SceneRayGroundIntersection.Intersect(ray, DefaultGround);
        Assert.True(hit.IsHit);

        // t = (0 - 20) / (-1/√2) = 20√2 ≈ 28.284
        // Hit = (10 + 28.284/√2, 0, 0) = (30, 0, 0)
        var expectedX = 30.0;
        Assert.NotNull(hit.WorldPosition);
        Assert.Equal(expectedX, hit.WorldPosition.Value.X, 0.01);
        Assert.Equal(0, hit.WorldPosition.Value.Y, Epsilon);
        Assert.Equal(0, hit.WorldPosition.Value.Z, Epsilon);
    }

    private static Vector3d Normalize(Vector3d v)
    {
        var len = Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
        return new Vector3d(v.X / len, v.Y / len, v.Z / len);
    }
}

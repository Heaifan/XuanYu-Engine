using XuanYu.Core.Math;
using XuanYu.Core.Scene;

namespace XuanYu.Core.Tests.Scene.Transform;

public sealed class CommittedTests
{
    [Fact]
    public void IdentityDefaults()
    {
        var t = CommittedTransform.Identity;

        Assert.Equal(Vector3d.Zero, t.Position);
        Assert.Equal(Vector3d.Zero, t.Rotation);
        Assert.Equal(new Vector3d(1, 1, 1), t.Scale);
        Assert.Equal(0.0001, CommittedTransform.MinimumScale);
    }

    [Fact]
    public void PositionDefaults()
    {
        var t = new CommittedTransform(Vector3d.UnitX);

        Assert.Equal(Vector3d.UnitX, t.Position);
        Assert.Equal(Vector3d.Zero, t.Rotation);
        Assert.Equal(new Vector3d(1, 1, 1), t.Scale);
    }

    [Fact]
    public void WithMethodsAreIsolated()
    {
        var original = CommittedTransform.Identity;
        var moved = original.WithPosition(Vector3d.UnitX);
        var rotated = original.WithRotation(Vector3d.UnitY);
        var scaled = original.WithScale(new Vector3d(2, 3, 4));

        Assert.Equal(Vector3d.UnitX, moved.Position);
        Assert.Equal(original.Rotation, moved.Rotation);
        Assert.Equal(original.Scale, moved.Scale);
        Assert.Equal(original.Position, rotated.Position);
        Assert.Equal(Vector3d.UnitY, rotated.Rotation);
        Assert.Equal(original.Scale, rotated.Scale);
        Assert.Equal(original.Position, scaled.Position);
        Assert.Equal(original.Rotation, scaled.Rotation);
        Assert.Equal(new Vector3d(2, 3, 4), scaled.Scale);
        Assert.Equal(CommittedTransform.Identity, original);
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void RejectsNonFinite(double invalid)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new CommittedTransform(new Vector3d(invalid, 0, 0)));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            CommittedTransform.Identity.WithRotation(new Vector3d(0, invalid, 0)));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            CommittedTransform.Identity.WithScale(new Vector3d(1, 1, invalid)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(0.000099)]
    public void RejectsSmallScale(double scale)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            CommittedTransform.Identity.WithScale(new Vector3d(scale, 1, 1)));
    }

    [Fact]
    public void AcceptsMinScale()
    {
        var scale = new Vector3d(CommittedTransform.MinimumScale, 1, 1);

        Assert.Equal(scale, CommittedTransform.Identity.WithScale(scale).Scale);
    }
}

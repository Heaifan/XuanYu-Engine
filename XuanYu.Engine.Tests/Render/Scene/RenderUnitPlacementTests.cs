using FluidWarfare.Core.Math;
using FluidWarfare.Render.Scene;
using FluidWarfare.Render.Selection;

namespace FluidWarfare.Tests.Render.Scene;

public sealed class RenderUnitPlacementTests
{
    [Fact]
    public void VisualCenter_EqualsBoundsCenter()
    {
        var placement = new RenderUnitPlacement(new Vector3d(-4, -1, 0));
        Assert.Equal(placement.VisualCenter, placement.SelectionBounds.Center);
    }

    [Fact]
    public void HalfExtent_IsCorrect()
    {
        // HalfExtent = LocalSize * Scale / 2 = 1.0 * 1.25 / 2 = 0.625
        Assert.Equal(0.625, RenderUnitPlacement.HalfExtent, 5);
    }

    [Fact]
    public void Scale_IsCorrect()
    {
        Assert.Equal(1.25, RenderUnitPlacement.Scale, 5);
    }

    [Fact]
    public void GroundAnchor_AtOrigin_PlacesVisualCenterAtHalfExtent()
    {
        var placement = new RenderUnitPlacement(Vector3d.Zero);
        Assert.Equal(new Vector3d(0, 0, RenderUnitPlacement.HalfExtent), placement.VisualCenter);
    }

    [Fact]
    public void GroundAnchor_AtNegativeZ_SetsVisualCenterCorrectly()
    {
        var placement = new RenderUnitPlacement(new Vector3d(10, 20, -5));
        Assert.Equal(new Vector3d(10, 20, -5 + RenderUnitPlacement.HalfExtent), placement.VisualCenter);
    }

    [Fact]
    public void SelectionBounds_CenterMatchesVisualCenter()
    {
        var placement = new RenderUnitPlacement(new Vector3d(1, 3, 0));
        Assert.Equal(placement.VisualCenter.X, placement.SelectionBounds.Center.X);
        Assert.Equal(placement.VisualCenter.Y, placement.SelectionBounds.Center.Y);
        Assert.Equal(placement.VisualCenter.Z, placement.SelectionBounds.Center.Z);
    }

    [Fact]
    public void SelectionBounds_HalfExtentMatchesRenderScale()
    {
        var placement = new RenderUnitPlacement(Vector3d.Zero);
        Assert.Equal(RenderUnitPlacement.HalfExtent, placement.SelectionBounds.HalfExtents.X, 5);
        Assert.Equal(RenderUnitPlacement.HalfExtent, placement.SelectionBounds.HalfExtents.Y, 5);
        Assert.Equal(RenderUnitPlacement.HalfExtent, placement.SelectionBounds.HalfExtents.Z, 5);
    }

    [Fact]
    public void IsConsistentWithDraw_CorrectValues_ReturnsTrue()
    {
        var placement = new RenderUnitPlacement(new Vector3d(-4, -1, 0));
        Assert.True(placement.IsConsistentWithDraw(
            (float)placement.VisualCenter.X,
            (float)placement.VisualCenter.Y,
            (float)placement.VisualCenter.Z,
            (float)RenderUnitPlacement.Scale));
    }

    [Fact]
    public void IsConsistentWithDraw_WrongValues_ReturnsFalse()
    {
        var placement = new RenderUnitPlacement(Vector3d.Zero);
        Assert.False(placement.IsConsistentWithDraw(0, 0, 999, 1.25f));
    }

    [Fact]
    public void BoundsContains_IntersectionPoint()
    {
        // A ray from above through the visual center should hit the bounds
        var placement = new RenderUnitPlacement(Vector3d.Zero);
        var ray = new SceneRay(new Vector3d(0, 0, 10), new Vector3d(0, 0, -1));
        var hit = SceneRayBoundsIntersection.Test(ray, placement.SelectionBounds, out var dist);
        Assert.True(hit);
        Assert.True(dist > 0);
    }

    [Fact]
    public void BoundsMisses_WideOffset()
    {
        var placement = new RenderUnitPlacement(new Vector3d(100, 0, 0));
        var ray = new SceneRay(new Vector3d(0, 0, 10), new Vector3d(0, 0, -1));
        var hit = SceneRayBoundsIntersection.Test(ray, placement.SelectionBounds, out _);
        Assert.False(hit);
    }
}

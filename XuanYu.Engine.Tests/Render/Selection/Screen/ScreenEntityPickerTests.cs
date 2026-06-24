using FluidWarfare.Core.Math;
using FluidWarfare.Render.Selection;
using FluidWarfare.Render.Selection.Presented;
using FluidWarfare.Render.Selection.Screen;

namespace FluidWarfare.Tests.Render.Selection.Screen;

public sealed class ScreenEntityPickerTests
{
    private static readonly float[] VP = UnitVP();
    private const int VW = 800, VH = 600;

    private static float[] UnitVP()
    {
        var vp = new float[16];
        vp[0] = 2f / VW; vp[12] = -1;
        vp[5] = 2f / VH; vp[13] = -1;
        vp[10] = 1; vp[15] = 1;
        return vp;
    }

    private static PresentedEntityBounds E(int id, double cx, double cy, double cz, double half)
        => new(id, new SceneAxisAlignedBounds(
            new Vector3d(cx, cy, cz), new Vector3d(half, half, half)), (float)cz, 0);

    [Fact]
    public void FarEntity_ProjectedCenterHits()
    {
        var entities = new[] { E(1, 400, 300, 10, 1.0) };
        var span = new ReadOnlySpan<PresentedEntityBounds>(entities);
        var ok = ScreenBoundsProjection.TryPoint(400, 300, 10, VP, VW, VH, out var sx, out var sy);
        Assert.True(ok);
        Assert.Equal(400, sx, 0);
        Assert.Equal(300, sy, 0);

        var hit = ScreenEntityPicker.Pick(sx, sy, VP, VW, VH, span);
        Assert.NotNull(hit);
        Assert.Equal(1, hit.Value.EntityId);
    }

    [Fact]
    public void FarEntity_WithinFivePixelsHits()
    {
        var entities = new[] { E(1, 400, 300, 10, 1.0) };
        var span = new ReadOnlySpan<PresentedEntityBounds>(entities);
        var hit = ScreenEntityPicker.Pick(403, 302, VP, VW, VH, span);
        Assert.NotNull(hit);
    }

    [Fact]
    public void FarEntity_OutsideFivePixelsMisses()
    {
        var entities = new[] { E(1, 400, 300, 10, 0.001) };
        var span = new ReadOnlySpan<PresentedEntityBounds>(entities);
        var hit = ScreenEntityPicker.Pick(0, 0, VP, VW, VH, span);
        Assert.Null(hit);
    }

    [Fact]
    public void OverlappingCandidates_SelectNearestDepth()
    {
        var entities = new[]
        {
            E(1, 400, 300, 10, 2.0),  // Z=10 (far)
            E(2, 400, 300, 2, 2.0),   // Z=2  (near - higher priority)
        };
        var span = new ReadOnlySpan<PresentedEntityBounds>(entities);
        var hit = ScreenEntityPicker.Pick(400, 300, VP, VW, VH, span);
        Assert.NotNull(hit);
        Assert.Equal(2, hit.Value.EntityId);
    }

    [Fact]
    public void EmptyEntities_ReturnsNull()
    {
        var span = ReadOnlySpan<PresentedEntityBounds>.Empty;
        var hit = ScreenEntityPicker.Pick(400, 300, VP, VW, VH, span);
        Assert.Null(hit);
    }

    [Fact]
    public void Projection_IdentityVP_ReturnsFinite()
    {
        var ok = ScreenBoundsProjection.TryPoint(100, 200, 0, VP, 800, 600, out var sx, out var sy);
        Assert.True(ok);
        Assert.InRange(sx, 0, 800);
        Assert.InRange(sy, 0, 600);
    }
}
